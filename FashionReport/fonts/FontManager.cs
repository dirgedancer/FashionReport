using System.IO;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.GameFonts;

internal static class FontManager
{
    internal static ImFontPtr CustomFont { get; private set; }
    internal static ImFontPtr FixedTableFont { get; private set; }

    internal static void BuildFonts(IFontAtlasBuildToolkitPreBuild toolkit)
    {
        SafeFontConfig baseConfig = new SafeFontConfig
        {
            SizePx = 16f,
            MergeFont = default
        };

        using (Stream fontStream = File.OpenRead("Fonts/MyFont.ttf"))
        {
            CustomFont = toolkit.AddFontFromMemory(fontStream.ReadAllBytes(), baseConfig, "MyFont");
        }

        GameFontStyle gameFont = new GameFontStyle
        {
            SizePt = 12f,
            Bold = false,
            Italic = false
        };

        FixedTableFont = toolkit.AddGameGlyphs(gameFont, null, CustomFont);
    }

    private static byte[] ReadAllBytes(this Stream stream)
    {
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
