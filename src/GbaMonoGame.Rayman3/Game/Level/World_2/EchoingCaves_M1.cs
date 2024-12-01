﻿using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;
using Action = System.Action;

namespace GbaMonoGame.Rayman3;

// TODO: Scale camera positions?
// NOTE: In the original game it calls message Cam_SetPosition (1062) on the camera instead of Cam_Lock (1090) like we do. The reason the
//       camera doesn't move back to Rayman in the original game is because Rayman's screen position doesn't update when he isn't framed.
//       However, we update the screen positions regardless, and also allow you to play in any resolution, so we need to make sure it locks.
public class EchoingCaves_M1 : FrameSideScroller
{
    #region Constructor

    public EchoingCaves_M1(MapId mapId) : base(mapId) { }

    #endregion

    #region Private Properties

    private const int GateActorId = 1;
    private int[] SwitchActorIds { get; } = Engine.Settings.Platform switch
    {
        Platform.GBA => [22, 23, 28, 50],
        Platform.NGage => [45, 46, 56, 67],
        _ => throw new UnsupportedPlatformException()
    };

    private bool ShouldInitCutscene { get; set; } = true;
    private Action CurrentCutsceneStepAction { get; set; }
    private uint Timer { get; set; }
    private int CameraTargetIndex { get; set; }

    #endregion

    #region Public Methods

    public override void Init()
    {
        base.Init();

        if (GameInfo.PersistentInfo.LastCompletedLevel < (int)MapId.EchoingCaves_M1 && ShouldInitCutscene)
        {
            // Move camera to the gate
            Gate gate = Scene.GetGameObject<Gate>(GateActorId);
            Vector2 gatePos = gate.Position;
            gatePos -= new Vector2(120, 120);
            Scene.Camera.ProcessMessage(this, Message.Cam_Lock, gatePos);
            
            // Change the position of the circle transition
            CircleTransitionScreenEffect.Init(CircleTransitionValue, new Vector2(120, 80));

            // NOTE: Game calls Vsync here on GBA and renders tiles
            Scene.Playfield.Step();

            // Stop the main actor
            Scene.MainActor.ProcessMessage(this, Message.Main_Stop);

            // Don't allow pausing
            CanPause = false;

            // Set the gate action to be open
            gate.ActionId = Gate.Action.Open_Left;
            gate.ChangeAction();

            // Turn on switches
            foreach (int switchActorId in SwitchActorIds)
            {
                Switch s = Scene.GetGameObject<Switch>(switchActorId);
                s.ActionId = Switch.Action.Activated;
                s.ChangeAction();
            }

            // Hide the level bars
            ((FrameSideScroller)Current).UserInfo.ForceHideLevelBars();

            // Set the switch bar to fully activated
            UserInfo.SwitchBar.Switches.CurrentAnimation = 4;
            UserInfo.SwitchBar.ActivatedSwitches = 4;

            CurrentCutsceneStepAction = Step_Cutscene_Init;
            Timer = 0;
        }
        else
        {
            UserInfo.SwitchBar.MoveIn();
            CurrentCutsceneStepAction = Step_Cutscene_Complete;
        }

        ShouldInitCutscene = false;
    }

    public override void UnInit()
    {
        base.UnInit();
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__barrel);
    }

    public override void Step()
    {
        CurrentCutsceneStepAction();
    }

    #endregion

    #region Steps

    public void Step_Cutscene_Complete()
    {
        base.Step();
    }

    public void Step_Cutscene_Init()
    {
        base.Step();

        // Wait with starting the timer until the transition has finished
        if (Timer == 0)
        {
            if (CircleTransitionMode == TransitionMode.None)
            {
                UserInfo.SwitchBar.MoveIn();
                Timer = 1;
            }
        }
        // Wait 2 seconds
        else if (Timer <= 120)
        {
            Timer++;
        }
        // Finished
        else
        {
            CurrentCutsceneStepAction = Step_Cutscene_ShowObject;
            TransitionsFX.FadeOutInit(2 / 16f);
            CameraTargetIndex = 1;
            Timer = 0;
        }
    }

    public void Step_Cutscene_ShowObject()
    {
        base.Step();

        // Wait until the fade out is finished
        if (!TransitionsFX.IsFadeOutFinished)
            return;

        Vector2 pos = Vector2.Zero;

        if (CameraTargetIndex < 5)
            pos = Scene.GetGameObject(SwitchActorIds[CameraTargetIndex - 1]).Position - new Vector2(120, 80);
        else if (CameraTargetIndex == 5)
            pos = Scene.GetGameObject(GateActorId).Position - new Vector2(120, 120);

        Scene.Camera.ProcessMessage(this, Message.Cam_Lock, pos);

        CurrentCutsceneStepAction = Step_Cutscene_AnimateObject;
        TransitionsFX.FadeInInit(2 / 16f);
    }

    public void Step_Cutscene_AnimateObject()
    {
        base.Step();

        // Wait until the fade in is finished
        if (!TransitionsFX.IsFadeInFinished)
            return;

        CurrentCutsceneStepAction = Step_Cutscene_Finish;
        Timer = 0;

        if (CameraTargetIndex == 0)
        {
            UserInfo.SwitchBar.MoveIn();
        }
        else if (CameraTargetIndex < 5)
        {
            Switch s = Scene.GetGameObject<Switch>(SwitchActorIds[CameraTargetIndex - 1]);
            s.ActionId = Switch.Action.Deactivating;
        }
        else if (CameraTargetIndex == 5)
        {
            Gate gate = Scene.GetGameObject<Gate>(GateActorId);
            gate.ActionId = Gate.Action.Closing_Left;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__GateClos_MetlGate_Mix01);
        }
    }

    public void Step_Cutscene_Finish()
    {
        base.Step();

        // NOTE: The original GBA code has a bug here where it doesn't check if the
        //       target is not 5, making it go out of bounds of the actor id array
        if (CameraTargetIndex != 5)
        {
            Switch s = Scene.GetGameObject<Switch>(SwitchActorIds[CameraTargetIndex - 1]);

            if (s.ActionId == Switch.Action.Deactivating && s.IsActionFinished)
            {
                s.ActionId = Switch.Action.Deactivated;

                UserInfo.SwitchBar.Switches.CurrentAnimation = 4 - CameraTargetIndex;
                UserInfo.SwitchBar.ActivatedSwitches = 4 - CameraTargetIndex;

                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LightFX1_Mix01);
            }
        }
        else
        {
            Gate gate = Scene.GetGameObject<Gate>(GateActorId);

            if (gate.IsActionFinished)
                gate.ActionId = Gate.Action.Init_4Switches_Left;
        }

        if ((Timer > 140 && CameraTargetIndex == 5) || 
            (Timer > 110 && CameraTargetIndex != 5))
        {
            CameraTargetIndex++;

            if (CameraTargetIndex <= 5)
            {
                CurrentCutsceneStepAction = Step_Cutscene_ShowObject;
                TransitionsFX.FadeOutInit(2 / 16f);
            }
            else
            {
                CurrentCutsceneStepAction = Step_Cutscene_Complete;
                Scene.Camera.ProcessMessage(this, Message.Cam_MoveToLinkedObject, false);
                CanPause = true;
            }
        }

        Timer++;
    }

    #endregion
}