namespace ConsoleNexusEngine.Graphics;

using System.IO;
using System.IO.Compression;
using System.Text;

/// <summary>
/// Allows for exporting <see cref="INexusSprite"/> and <see cref="NexusSpriteMap"/> to a file
/// </summary>
public static class NexusSpriteExporter
{
    /// <summary>
    /// The current version of the sprite exporter
    /// </summary>
    public const int CurrentVersion = 1;

    /// <summary>
    /// The file extension of a uncompressed exported sprite
    /// </summary>
    public const string Extension = INexusSprite.FileExtension;
    /// <summary>
    /// The file extension of a compressed exported sprite
    /// </summary>
    public const string ExtensionCompressed = INexusSprite.FileExtensionCompressed;

    /// <summary>
    /// Exports the sprite to the specified <paramref name="directory"/> with the specified <paramref name="spriteName"/><br/>
    /// Returns the full path of the exported sprite
    /// </summary>
    /// <param name="directory">The directory where the file should be saved</param>
    /// <param name="spriteName">The name of the exported sprite</param>
    /// <param name="sprite">The sprite to export</param>
    /// <param name="compressed"><see langword="false"/> for faster export but bigger file size.</param>
    /// <returns><see cref="string"/></returns>
    public static string Export(string directory, string spriteName, INexusSprite sprite, bool compressed = true)
        => Export(directory, spriteName, sprite.Map, compressed);

    /// <summary>
    /// Exports the sprite to the specified <paramref name="directory"/> with the specified <paramref name="spriteName"/><br/>
    /// Returns the full path of the exported sprite
    /// </summary>
    /// <param name="directory">The directory where the file should be saved</param>
    /// <param name="spriteName">The name of the exported sprite</param>
    /// <param name="spriteMap">The sprite map to export</param>
    /// <param name="compressed"><see langword="false"/> for faster export but bigger file size.</param>
    /// <returns><see cref="string"/></returns>
    public static string Export(string directory, string spriteName, in NexusSpriteMap spriteMap, bool compressed = true)
    {
        var extension = compressed ? ExtensionCompressed : Extension;

        var basePath = Path.Combine(directory, Path.ChangeExtension(spriteName, extension));
        var fullPath = basePath;
        for (int i = 1; File.Exists(fullPath); i++)
        {
            fullPath = $"{Path.ChangeExtension(basePath, null)}({i}){extension}";
        }

        var spriteSpan = spriteMap._spriteMap.Span;
        using (var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
        {
            Stream stream = compressed ? new GZipStream(fileStream, CompressionLevel.Optimal, true) : fileStream;

            using (var writer = new BinaryWriter(stream, Encoding.Unicode, true))
            {
                writer.Write(CurrentVersion);
                writer.Write(spriteMap.Size.Width);
                writer.Write(spriteMap.Size.Height);

                for (int i = 0; i < spriteSpan.Length; i++)
                {
                    writer.Write(spriteSpan[i].UnicodeChar);
                    writer.Write(spriteSpan[i].Attributes);
                }

                writer.Flush();
            }

            fileStream.Flush();
        }

        return fullPath;
    }
}