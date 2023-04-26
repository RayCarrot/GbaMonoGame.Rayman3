namespace OnyxCs.Gba.Sdk;

public class GfxScreen
{
    public GfxScreen()
    {
        // The screen always defaults to being enabled
        SetIsEnabled(true);
    }

    public GfxBgDriver? BackgroundDriver { get; set; }
    public Vec2Int Position { get; set; }
    public bool IsEnabled { get; set; }
    public int Priority { get; set; }
    public OverflowProcess OverflowProcess { get; set; }
    public bool Is8Bit { get; set; }

    public void SetBgDriver(int layerId)
    {
        BackgroundDriver = GfxBgDriver.GetBgDriver(layerId);
        BackgroundDriver.SetPriority(Priority);
        BackgroundDriver.SetOverflowProcess(OverflowProcess);
        BackgroundDriver.SetColorMode(Is8Bit);
        BackgroundDriver.SetOffset(Position);
        BackgroundDriver.SetOnOff(IsEnabled);
    }

    // TODO: Move methods to property setters?

    public void SetIsEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
        BackgroundDriver?.SetOnOff(IsEnabled);
    }

    public void SetPriority(int prio)
    {
        Priority = prio;
        BackgroundDriver?.SetPriority(Priority);
    }

    public void SetBgOverflowProcess(OverflowProcess overflowProcess)
    {
        OverflowProcess = overflowProcess;
        BackgroundDriver?.SetOverflowProcess(OverflowProcess);
    }

    public void SetColorMode(bool is8Bit)
    {
        Is8Bit = is8Bit;
        BackgroundDriver?.SetColorMode(Is8Bit);
    }

    public void SetOffset(Vec2Int offset)
    {
        Position = offset;
        BackgroundDriver?.SetOffset(offset);
    }
}