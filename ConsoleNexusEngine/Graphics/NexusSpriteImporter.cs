namespace ConsoleNexusEngine.Graphics;

using System.IO.Compression;
using System.IO;
using System.Text;

/// <summary>
/// Allows for importing <see cref="NexusSpriteMap"/> from a file
/// </summary>
public static class NexusSpriteImporter
{
    /// <summary>
    /// Imports a compressed or uncompressed sprite
    /// </summary>
    /// <param name="spritePath">The path of the sprite</param>
    /// <returns><see cref="NexusSpriteMap"/></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static NexusSpriteMap Import(string spritePath)
    {
        var extension = Path.GetExtension(spritePath);
        if (extension is not (INexusSprite.FileExtension or INexusSprite.FileExtensionCompressed))
            throw new ArgumentException($"The file path is not a {INexusSprite.FileExtension} or {INexusSprite.FileExtensionCompressed} file", nameof(spritePath));

        using (var fileStream = new FileStream(spritePath, FileMode.Open, FileAccess.Read))
        {
            Stream stream = extension == INexusSprite.FileExtension ? fileStream : new GZipStream(fileStream, CompressionMode.Decompress, false);

            using (var reader = new BinaryReader(stream, Encoding.Unicode, false))
            {
                var version = reader.ReadInt32();
                return version switch
                {
                    1 => ReadVersion1(reader),
                    _ => throw new NotSupportedException($"This version ({version}) is not supported"),
                };
            }
        }
    }

    private static NexusSpriteMap ReadVersion1(BinaryReader reader)
    {
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();

        var size = new NexusSize(width, height);

        Span<CHARINFO> map = StackAlloc.Allow<CHARINFO>(size.Dimensions) ? stackalloc CHARINFO[size.Dimensions] : new CHARINFO[size.Dimensions];
        for (int i = 0; i < map.Length; i++)
        {
            ref var pixel = ref map[i];
            pixel.UnicodeChar = reader.ReadChar();
            pixel.Attributes = reader.ReadInt16();
        }

        return new NexusSpriteMap(map, size);
    }
}