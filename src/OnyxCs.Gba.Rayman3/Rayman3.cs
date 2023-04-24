using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.Rayman3;

public class Rayman3 : Engine
{
    public Rayman3(Vram vram, JoyPad joyPad)
    {
        Vram = vram;
        JoyPad = joyPad;

        FrameMngr = new FrameMngr(new Intro());
        GameTime = new GameTime();
    }

    public override FrameMngr FrameMngr { get; }
    public override GameTime GameTime { get; }

    public override Vram Vram { get; }
    public override JoyPad JoyPad { get; }
}