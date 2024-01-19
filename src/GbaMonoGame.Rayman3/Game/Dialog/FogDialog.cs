using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

// TODO: Rendering currently doesn't work like it should. The game has sprites wrap around at 512 because of the x position
//       in the OAM being a 9-bit signed value. However, we want to allow resolutions wider than 512, so this should be
//       handled differently, probably with multiple fog objects filling the width of the screen.
public class FogDialog : Dialog
{
    public FogDialog(TgxPlayfield playfield)
    {
        Playfield = playfield;
        ShouldDraw = true;
    }

    private TgxPlayfield Playfield { get; }
    private AnimatedObject Fog { get; set; }
    private bool ShouldDraw { get; set; }
    private int ScrollX { get; set; }
    private int ScrollSpeed { get; set; }

    public override void ProcessMessage() { }

    public override void Load()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.FogAnimations);

        Fog = new AnimatedObject(resource, resource.IsDynamic)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(0, 128),
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
            Fog.ScreenPos = new Vector2(512 - (camPos.X + ScrollX) % 512, height - camPos.Y - 32);
            animationPlayer.Play(Fog);
        }

        // TODO: Smoother scrolling using floats? Do that throughout game? Optional setting?
        if ((GameTime.ElapsedFrames & 7) == 0)
        {
            ScrollX += ScrollSpeed;
            if (ScrollX > 512)
                ScrollX = 0;
        }
    }
}