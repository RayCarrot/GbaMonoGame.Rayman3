using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public class FogDialog : Dialog
{
    public FogDialog(Scene2D scene) : base(scene)
    {
        Playfield = scene.Playfield;
        ShouldDraw = true;
    }

    private TgxPlayfield Playfield { get; }
    private AObjectFog Fog { get; set; }
    private float ScrollX { get; set; }
    private int ScrollSpeed { get; set; }

    public bool ShouldDraw { get; set; }

    protected override bool ProcessMessageImpl(object sender, Message message, object param) => false;

    public override void Load()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.FogAnimations);

        Fog = new AObjectFog(resource)
        {
            SpritePriority = 0,
            YPriority = 63,
            IsAlphaBlendEnabled = true,
            GbaAlpha = 6,
            Camera = Playfield.Camera
        };

        ScrollX = 0;
        ScrollSpeed = 1;
    }

    public override void Init() { }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        if (!ShouldDraw) 
            return;
        
        Vector2 camPos = Playfield.Camera.Position;
        int height = Playfield.PhysicalLayer.PixelHeight;

        if (height - 32 < camPos.Y + Playfield.Camera.Resolution.Y)
        {
            // What the game does:
            // Fog.ScreenPos = new Vector2(512 - (camPos.X + ScrollX) % 512, height - camPos.Y - 32);
            Fog.ScreenPos = new Vector2(-(camPos.X + ScrollX) % AObjectFog.Width, height - camPos.Y - 32);
            
            animationPlayer.Play(Fog);
        }

        ScrollX += ScrollSpeed / 8f; // NOTE: Game scrolls every 8 frames
        
        // What the game does:
        // if (ScrollX > 512)
        //     ScrollX = 0;
        ScrollX %= AObjectFog.Width;
    }
}