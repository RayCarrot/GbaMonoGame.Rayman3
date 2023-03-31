using BinarySerializer;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.Sdk;
using GameTime = Microsoft.Xna.Framework.GameTime;

namespace OnyxCs.Gba.MonoGame.Rayman3;

public class GbaGameComponent : GameComponent
{
    public GbaGameComponent(Game game, Context context, Frame firstFrame, int width, int height) : base(game)
    {
        Context = context;
        FrameManager = new FrameMngr(firstFrame);
        GbaScreen = new GbaScreen(game.GraphicsDevice, width, height, Vram.Instance);
    }

    public Context Context { get; }
    public FrameMngr FrameManager { get; }
    public GbaScreen GbaScreen { get; }

    public override void Initialize()
    {
        Storage.Load(Context);
    }

    public override void Update(GameTime gameTime)
    {
        // Update game engine
        FrameManager.Step();

        // Update screen
        GbaScreen.Update();
    }
}