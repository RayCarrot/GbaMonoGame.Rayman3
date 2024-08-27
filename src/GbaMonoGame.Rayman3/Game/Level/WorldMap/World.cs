using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;
using Action = System.Action;

namespace GbaMonoGame.Rayman3;

public class World : FrameWorldSideScroller
{
    public World(MapId mapId) : base(mapId) { }

    private Action CurrentExStepAction { get; set; }
    private Action NextExStepAction { get; set; }
    
    private uint MurfyTimer { get; set; }
    private TextBoxDialog TextBox { get; set; }
    private byte MurfyLevelCurtainTargetId { get; set; }
    private byte MurfyId { get; set; }

    private PaletteFadeEffectObject PaletteFade { get; set; }
    private int PaletteFadeTimer { get; set; }
    private bool FinishedTransitioningOut { get; set; }

    public void InitEntering()
    {
        FinishedTransitioningOut = false;
        PaletteFade.Fade = 1;
        UserInfo.Hide = true;
        CurrentExStepAction = StepEx_MoveInCurtains;
        PaletteFadeTimer = PaletteFade.MinFadeTime;
        Gfx.FadeControl = FadeControl.None;
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Curtain_YoyoMove_Mix02);
    }

    public void InitExiting()
    {
        CurrentExStepAction = StepEx_FadeOut;
        PaletteFadeTimer = PaletteFade.MaxFadeTime;
        FinishedTransitioningOut = false;
        UserInfo.Hide = true;
        BlockPause = true;
    }

    public bool IsTransitioningOut() => !FinishedTransitioningOut;

    public override void Init()
    {
        base.Init();

        UserInfo = new UserInfoWorld(Scene, GameInfo.Level.HasBlueLum);
        Scene.AddDialog(UserInfo, false, false);

        BlockPause = true;

        TextBox = new TextBoxDialog(Scene);
        Scene.AddDialog(TextBox, false, false);

        CurrentExStepAction = StepEx_MoveInCurtains;
        NextExStepAction = null;

        Vector2 camLockOffset = Engine.Settings.Platform switch
        {
            Platform.GBA => new Vector2(110, 120),
            Platform.NGage => new Vector2(75, 120),
            _ => throw new UnsupportedPlatformException()
        };

        if (GameInfo.MapId == MapId.World1 && 
            !GameInfo.PersistentInfo.PlayedMurfyWorldHelp)
        {
            MurfyLevelCurtainTargetId = 16;
            MurfyId = 41;

            // Lock the camera on N-Gage, otherwise Murfy is off-screen
            if (Engine.Settings.Platform == Platform.NGage)
                Scene.Camera.ProcessMessage(this, Message.Cam_Lock, Scene.GetGameObject(MurfyLevelCurtainTargetId).Position - camLockOffset);

            Scene.MainActor.ProcessMessage(this, Message.Main_EnterCutscene);
            NextExStepAction = StepEx_SpawnMurfy;
            
            GameInfo.PersistentInfo.PlayedMurfyWorldHelp = true;
            GameInfo.Save(GameInfo.CurrentSlot);

            Murfy murfy = Scene.GetGameObject<Murfy>(MurfyId);
            murfy.Position = murfy.Position with { Y = Scene.Playfield.Camera.Position.Y };
            murfy.IsForBonusInWorld1 = false;
        }
        else if (GameInfo.MapId == MapId.World1 && 
                 GameInfo.World1LumsCompleted() &&
                 !GameInfo.PersistentInfo.UnlockedBonus1)
        {
            MurfyLevelCurtainTargetId = 20;
            MurfyId = 41;

            Scene.Camera.ProcessMessage(this, Message.Cam_Lock, Scene.GetGameObject(MurfyLevelCurtainTargetId).Position - camLockOffset);
            Scene.MainActor.ProcessMessage(this, Message.Main_EnterCutscene);
            NextExStepAction = StepEx_SpawnMurfy;

            GameInfo.PersistentInfo.UnlockedBonus1 = true;
            GameInfo.Save(GameInfo.CurrentSlot);

            UserInfo.Hide = true;
        }
        else if (GameInfo.MapId == MapId.World2 && 
                 GameInfo.World2LumsCompleted() &&
                 !GameInfo.PersistentInfo.UnlockedBonus2)
        {
            MurfyLevelCurtainTargetId = Engine.Settings.Platform switch
            {
                Platform.GBA => 9,
                Platform.NGage => 10,
                _ => throw new UnsupportedPlatformException()
            };
            MurfyId = 22;

            Scene.Camera.ProcessMessage(this, Message.Cam_Lock, Scene.GetGameObject(MurfyLevelCurtainTargetId).Position - camLockOffset);
            Scene.MainActor.ProcessMessage(this, Message.Main_EnterCutscene);
            NextExStepAction = StepEx_SpawnMurfy;

            GameInfo.PersistentInfo.UnlockedBonus2 = true;
            GameInfo.Save(GameInfo.CurrentSlot);

            UserInfo.Hide = true;
        }
        else if (GameInfo.MapId == MapId.World3 && 
                 GameInfo.World3LumsCompleted() &&
                 !GameInfo.PersistentInfo.UnlockedBonus3)
        {
            MurfyLevelCurtainTargetId = Engine.Settings.Platform switch
            {
                Platform.GBA => 14,
                Platform.NGage => 18,
                _ => throw new UnsupportedPlatformException()
            };
            MurfyId = Engine.Settings.Platform switch
            {
                Platform.GBA => 22,
                Platform.NGage => 23,
                _ => throw new UnsupportedPlatformException()
            };

            Scene.Camera.ProcessMessage(this, Message.Cam_Lock, Scene.GetGameObject(MurfyLevelCurtainTargetId).Position - camLockOffset);
            Scene.MainActor.ProcessMessage(this, Message.Main_EnterCutscene);
            NextExStepAction = StepEx_SpawnMurfy;

            GameInfo.PersistentInfo.UnlockedBonus3 = true;
            GameInfo.Save(GameInfo.CurrentSlot);

            UserInfo.Hide = true;
        }
        else if (GameInfo.MapId == MapId.World4 && 
                 GameInfo.World4LumsCompleted() &&
                 !GameInfo.PersistentInfo.UnlockedBonus4)
        {
            MurfyLevelCurtainTargetId = 11;
            MurfyId = Engine.Settings.Platform switch
            {
                Platform.GBA => 22,
                Platform.NGage => 21,
                _ => throw new UnsupportedPlatformException()
            };

            Scene.Camera.ProcessMessage(this, Message.Cam_Lock, Scene.GetGameObject(MurfyLevelCurtainTargetId).Position - camLockOffset);
            Scene.MainActor.ProcessMessage(this, Message.Main_EnterCutscene);
            NextExStepAction = StepEx_SpawnMurfy;

            GameInfo.PersistentInfo.UnlockedBonus4 = true;
            GameInfo.Save(GameInfo.CurrentSlot);

            UserInfo.Hide = true;
        }

        PaletteFade = new PaletteFadeEffectObject()
        {
            SpritePriority = 0,
        };

        InitEntering();

        // We have to show the palette fade already now or we have one game frame with the level visible
        Scene.AnimationPlayer.PlayFront(PaletteFade);

        Scene.Playfield.Step();
        Scene.AnimationPlayer.Execute();
    }

    public override void Step()
    {
        Scene.AnimationPlayer.PlayFront(PaletteFade);

        base.Step();

        if (!IsBusy())
            CurrentExStepAction?.Invoke();
    }

    #region Steps

    private void StepEx_MoveInCurtains()
    {
        if (UserInfo.HasFinishedMovingInCurtains())
            CurrentExStepAction = StepEx_FadeIn;
    }

    private void StepEx_FadeIn()
    {
        // In the original code this function is actually two step functions
        // that it cycles between every second frame. The first one modifies
        // the background palette and second one the object palette.

        PaletteFade.SetFadeFromTimer(PaletteFadeTimer);

        PaletteFadeTimer++;
        if (PaletteFadeTimer > PaletteFade.MaxFadeTime)
        {
            if (NextExStepAction == null)
                UserInfo.Hide = false;

            BlockPause = false;
            CurrentExStepAction = NextExStepAction;
        }
    }

    private void StepEx_FadeOut()
    {
        // In the original code this function is actually two step functions
        // that it cycles between every second frame. The first one modifies
        // the background palette and second one the object palette.

        PaletteFade.SetFadeFromTimer(PaletteFadeTimer);

        if (PaletteFadeTimer == PaletteFade.MinFadeTime)
        {
            CurrentExStepAction = StepEx_MoveOutCurtains;
            UserInfo.MoveOutCurtains();
        }
        else
        {
            PaletteFadeTimer--;
        }
    }

    private void StepEx_MoveOutCurtains()
    {
        if (UserInfo.HasFinishedMovingOutCurtains())
            FinishedTransitioningOut = true;
    }

    private void StepEx_SpawnMurfy()
    {
        Murfy murfy = Scene.GetGameObject<Murfy>(MurfyId);
        murfy.TargetActor = Scene.GetGameObject<BaseActor>(MurfyLevelCurtainTargetId);
        murfy.ProcessMessage(this, Message.Murfy_Spawn);

        if (MurfyLevelCurtainTargetId == 16)
            CurrentExStepAction = StepEx_MurfyIntroCutscene;
        else
            CurrentExStepAction = StepEx_MurfyBonusLevelCutscene;

        MurfyTimer = 0;
    }

    private void StepEx_MurfyIntroCutscene()
    {
        if (MurfyTimer == 0)
        {
            if (TextBox.IsFinished)
                MurfyTimer = 1;
        }
        else
        {
            if (MurfyTimer > 90)
            {
                Scene.MainActor.ProcessMessage(this, Message.Main_ExitStopOrCutscene);
                
                if (Engine.Settings.Platform == Platform.NGage)
                    Scene.Camera.ProcessMessage(this, Message.Cam_Unlock);
                
                UserInfo.Hide = false;
                CurrentExStepAction = null;
                BlockPause = false;
            }

            MurfyTimer++;
        }
    }

    private void StepEx_MurfyBonusLevelCutscene()
    {
        if (MurfyTimer == 0)
        {
            if (TextBox.IsFinished)
            {
                // Unlock curtain
                LevelCurtain levelCurtain = Scene.GetGameObject<LevelCurtain>(MurfyLevelCurtainTargetId);
                levelCurtain.AnimatedObject.BasePaletteIndex = 0;
                levelCurtain.IsLocked = false;
                
                MurfyTimer = 1;
            }
        }
        else
        {
            if (MurfyTimer > 90)
            {
                CurrentExStepAction = StepEx_MurfyFadeOut;
                TransitionsFX.FadeOutInit(2 / 16f);
            }

            MurfyTimer++;
        }
    }

    private void StepEx_MurfyFadeOut()
    {
        if (TransitionsFX.IsFadeOutFinished)
        {
            Scene.Camera.ProcessMessage(this, Message.Cam_SetPosition, Scene.MainActor.Position - new Vector2(120, 120));
            Scene.Camera.ProcessMessage(this, Message.Cam_Unlock);
            UserInfo.Hide = false;
            TransitionsFX.FadeInInit(2 / 16f);
            CurrentExStepAction = StepEx_MurfyFadeIn;
        }
    }

    private void StepEx_MurfyFadeIn()
    {
        if (TransitionsFX.IsFadeInFinished)
        {
            Scene.MainActor.ProcessMessage(this, Message.Main_ExitStopOrCutscene);
            CurrentExStepAction = null;
        }
    }

    #endregion
}