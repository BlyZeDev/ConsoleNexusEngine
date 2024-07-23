namespace ConsoleNexusEngine.Internal;

using System.Drawing.Imaging;

internal interface ILockablePixels
{
    public BitmapData LockBitsReadOnly();

    public void UnlockBits(BitmapData data);
}