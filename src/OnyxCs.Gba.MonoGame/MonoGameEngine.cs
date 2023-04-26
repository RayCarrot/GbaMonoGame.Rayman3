using Microsoft.Xna.Framework.Graphics;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.MonoGame;

public class MonoGameEngine : Engine
{
    public MonoGameEngine(Frame initialFrame, GraphicsDevice graphicsDevice, int width, int height)
    {
        FrameManager = new FrameManager(initialFrame);
        GameTime = new OnyxGameTime();
        SoundManager = new MonoGameSoundManager();
        Vram = new MonoGameVram(graphicsDevice, width, height);
        JoyPad = new MonoGameJoyPad();
    }

    public override FrameManager FrameManager { get; }
    public override OnyxGameTime GameTime { get; }
    public override MonoGameSoundManager SoundManager { get; }
    public override MonoGameVram Vram { get; }
    public override MonoGameJoyPad JoyPad { get; }
}