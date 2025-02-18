namespace ConsoleNexusEngine.Internal;

using System.Threading.Tasks;
using System.Threading;
using System.Runtime.ExceptionServices;

internal static class TaskHelper
{
    public static async void RunInBackground<TException>(this Task task, Action<TException> onException, bool rethrow) where TException : Exception
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (TException ex)
        {
            onException(ex);

            if (rethrow) ExceptionDispatchInfo.Throw(ex);
        }
    }

    public static T? RunSync<T>(Func<Task<T>> task)
    {
        var oldContext = SynchronizationContext.Current;
        var syncContext = new ExclusiveSynchronizationContext();

        SynchronizationContext.SetSynchronizationContext(syncContext);

        T? result = default;
        syncContext.Post(async _ =>
        {
            try
            {
                result = await task();
            }
            catch (Exception ex)
            {
                syncContext.InnerException = ex;
                throw;
            }
            finally
            {
                syncContext.EndMessageLoop();
            }
        }, null);

        syncContext.BeginMessageLoop();

        SynchronizationContext.SetSynchronizationContext(oldContext);

        return result;
    }

    private sealed class ExclusiveSynchronizationContext : SynchronizationContext
    {
        private readonly AutoResetEvent _workItemsWaiting = new(false);
        private readonly Queue<Tuple<SendOrPostCallback, object?>> _items = new();
        private bool done;

        public Exception? InnerException { get; set; }

        public override void Send(SendOrPostCallback d, object? state)
            => throw new NotSupportedException("We cannot send to our same thread");

        public override void Post(SendOrPostCallback d, object? state)
        {
            lock (_items)
            {
                _items.Enqueue(Tuple.Create(d, state));
            }

            _workItemsWaiting.Set();
        }

        public void EndMessageLoop() => Post(_ => done = true, null);

        public void BeginMessageLoop()
        {
            while (!done)
            {
                Tuple<SendOrPostCallback, object?>? task = null;

                lock (_items)
                {
                    if (_items.Count > 0)
                    {
                        task = _items.Dequeue();
                    }
                }

                if (task is not null)
                {
                    task.Item1(task.Item2);

                    if (InnerException is not null)
                    {
                        throw new AggregateException($"{nameof(RunSync)} method threw an exception.", InnerException);
                    }
                }
                else
                {
                    _workItemsWaiting.WaitOne();
                }
            }
        }

        public override SynchronizationContext CreateCopy() => this;
    }
}