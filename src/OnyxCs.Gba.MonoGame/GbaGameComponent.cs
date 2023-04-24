using BinarySerializer;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.Sdk;
using GameTime = Microsoft.Xna.Framework.GameTime;

namespace OnyxCs.Gba.MonoGame.Rayman3;

public class GbaGameComponent : GameComponent
{
    public GbaGameComponent(Game game, Context context, Engine engine, MonoGameVram vram) : base(game)
    {
        Context = context;
        Engine = engine;
        MonoGameVram = vram;
    }

    public Context Context { get; }
    public Engine Engine { get; }
    public MonoGameVram MonoGameVram { get; }

    public override void Initialize()
    {
        Storage.Load(Context);
    }

    public override void Update(GameTime gameTime)
    {
        // Update game engine
        Engine.Step();

        // Update screen
        MonoGameVram.Update();
    }
}