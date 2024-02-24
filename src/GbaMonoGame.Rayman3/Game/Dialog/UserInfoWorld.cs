using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class UserInfoWorld : Dialog
{
    public UserInfoWorld(Scene2D scene, bool hasBlueLum)
    {
        LifeBar = new LifeBar(scene);
        LevelInfoBar = new LevelInfoBar();
        Lums1000Bar = new Lums1000Bar();
        Cages50Bar = new Cages50Bar();

        if (hasBlueLum)
        {
            // TODO: Blue lums bar
        }

        if (GameInfo.MapId == MapId.WorldMap)
        {
            // TODO: World name bar
        }

        if (LifeBar.Mode != 3)
            LifeBar.Mode = 2;

        Hide = false;
        ShouldPlayCurtainAnimation = true;
    }

    private LifeBar LifeBar { get; }
    private LevelInfoBar LevelInfoBar { get; }
    private Lums1000Bar Lums1000Bar { get; }
    private Cages50Bar Cages50Bar { get; }

    private bool ShouldPlayCurtainAnimation { get; set; }
    
    public bool Hide { get; set; }

    // The game only has a single animated object, but we split it into two in order
    // to support different screen resolutions (so it fills the width of the screen)
    public AnimatedObject CurtainsLeft { get; set; }
    public AnimatedObject CurtainsRight { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        // TODO: Implement
        return false;
    }

    public void SetLevelInfoBar(int levelCurtainId)
    {
        LevelInfoBar.SetLevel(levelCurtainId);
    }

    public override void Load()
    {
        LifeBar.Load();
        LevelInfoBar.Load();
        Lums1000Bar.Load();
        Cages50Bar.Load();

        LifeBar.Set();
        LevelInfoBar.Set();
        Lums1000Bar.Set();
        Cages50Bar.Set();

        if (Engine.Settings.Platform == Platform.GBA && GameInfo.MapId != MapId.WorldMap)
        {
            AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.WorldCurtainAnimations);

            CurtainsLeft = new AnimatedObject(resource, false)
            {
                IsFramed = true,
                CurrentAnimation = ShouldPlayCurtainAnimation ? 1 : 0,
                ScreenPos = new Vector2(120, 56),
            };
            CurtainsRight = new AnimatedObject(resource, false)
            {
                IsFramed = true,
                CurrentAnimation = ShouldPlayCurtainAnimation ? 1 : 0,
                ScreenPos = new Vector2(Engine.ScreenCamera.Resolution.X - 120, 56),
            };

            for (int i = 0; i < 6; i++)
            {
                CurtainsLeft.SetChannelInvisible(i);
                CurtainsRight.SetChannelInvisible(i + 6);
            }

            if (ShouldPlayCurtainAnimation)
                ShouldPlayCurtainAnimation = false;
        }
    }

    public override void Init() { }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        if (Engine.Settings.Platform == Platform.GBA && GameInfo.MapId != MapId.WorldMap)
        {
            animationPlayer.PlayFront(CurtainsLeft);

            CurtainsRight.ScreenPos = new Vector2(Engine.ScreenCamera.Resolution.X - 120, CurtainsRight.ScreenPos.Y);
            animationPlayer.PlayFront(CurtainsRight);
        }

        if (!Hide)
        {
            LifeBar.Draw(animationPlayer);
            LevelInfoBar.Draw(animationPlayer);
            Lums1000Bar.Draw(animationPlayer);
            Cages50Bar.Draw(animationPlayer);
        }
    }
}