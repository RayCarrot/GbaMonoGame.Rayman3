using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

// Temp code for testing a level
public class LevelTest : Frame
{
    private TgxPlayfield2D Playfield { get; set; }

    public override void Init()
    {
        Gfx.Clear();

        Scene2D scene = Storage.LoadResource<Scene2D>(0x00);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(scene.Playfield);
    }

    public override void Step()
    {
        // TODO: Implement
    }
}