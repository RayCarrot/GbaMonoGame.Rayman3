using BinarySerializer.Nintendo.GBA;

namespace OnyxCs.Gba.Sdk;

public class GfxBgDriver
{
    private GfxBgDriver(int bgIndex)
    {
        _bg = Vram.Instance.GetBackground(bgIndex);
    }

    private readonly Background _bg;

    public void SetPriority(int priority) => _bg.Priority = priority;
    public void SetOverflowProcess(OverflowProcess overflowProcess) => _bg.OverflowProcess = overflowProcess;
    public void SetColorMode(bool is8bit) => _bg.Is8Bit = is8bit;
    public void SetOffset(Vec2Int pos) => _bg.Offset = pos;
    public void SetOnOff(bool isEnabled) => _bg.IsEnabled = isEnabled;

    // New methods
    public void SetTileMap(MapTile[] tileMap, int width, int height)
    {
        _bg.Map = tileMap;
        _bg.Width = width;
        _bg.Height = height;
    }
    public void SetTileSet(byte[] tileSet, Palette palette)
    {
        _bg.TileSet = tileSet;
        _bg.Palette = palette;
    }

    public static GfxBgDriver GetBgDriver(int layerId) => new(layerId);
}