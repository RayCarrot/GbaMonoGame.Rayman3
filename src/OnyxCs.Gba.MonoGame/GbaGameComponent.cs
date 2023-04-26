using BinarySerializer;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.MonoGame;

public class GbaGameComponent : GameComponent
{
    public GbaGameComponent(Game game, Context context, Frame initialFrame, int width, int height) : base(game)
    {
        Context = context;
        Engine = new MonoGameEngine(initialFrame, game.GraphicsDevice, width, height);
    }

    public Context Context { get; }
    public MonoGameEngine Engine { get; }

    public override void Initialize()
    {
        Storage.Load(Context);
    }

    public override void Update(GameTime gameTime)
    {
        // Update game engine
        Engine.Step();

        // Update screen
        Engine.Vram.Update();
    }
}