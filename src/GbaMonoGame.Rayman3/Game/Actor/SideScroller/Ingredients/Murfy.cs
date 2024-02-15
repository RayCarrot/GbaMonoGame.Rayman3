using System;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// TODO: Action id enum
public sealed partial class Murfy : MovableActor
{
    public Murfy(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        MainActor = Scene.MainActor;
        Byte_8D = 1;

        if (Engine.Settings.Platform == Platform.NGage)
            NGage_Byte_8E = 1;

        Fsm.ChangeAction(Fsm_PreInit);
    }

    private TextBoxDialog TextBox { get; set; }
    private Vector2 TargetPosition { get; set; }
    private Vector2 MainActorPosition { get; set; }
    private Vector2 InitialPosition { get; set; }
    private MovableActor MainActor { get; set; }
    private Vector2 SavedSpeed { get; set; }
    private byte Timer { get; set; }
    private bool MoveTextBoxIn { get; set; }
    private byte Byte_8A { get; set; }
    private int Byte_8B { get; set; }
    private bool ShouldSpawn { get; set; }
    private byte Byte_8D { get; set; }
    private byte NGage_Byte_8E { get; set; }

    private void SetText()
    {
        TextBox.SetCutsceneCharacter(TextBoxCutsceneCharacter.Murfy);

        switch (GameInfo.MapId)
        {
            case MapId.WoodLight_M1:
                TextBox.SetText(0);
                break;

            case MapId.WoodLight_M2:
                Vector2 mainActorPos = Scene.MainActor.Position;
                if (mainActorPos.X < 800)
                    TextBox.SetText(3);
                else if (mainActorPos.X < 2700)
                    TextBox.SetText(2);
                else
                    TextBox.SetText(1);
                break;

            case MapId.FairyGlade_M2:
                TextBox.SetText(5);
                break;

            case MapId.BossMachine:
                TextBox.SetText(13);
                break;

            case MapId.MenhirHills_M1:
                TextBox.SetText(16);
                break;

            case MapId.SanctuaryOfStoneAndFire_M1:
                TextBox.SetText(15);
                break;

            case MapId.ChallengeLy1:
                TextBox.SetText(11);
                break;

            case MapId.ChallengeLy2:
                TextBox.SetText(12);
                break;

            case MapId.World1:
                if (Byte_8D == 0)
                    TextBox.SetText(14);
                else
                    TextBox.SetText(7);
                break;

            case MapId.World2:
                TextBox.SetText(8);
                break;

            case MapId.World3:
                TextBox.SetText(9);
                break;

            case MapId.World4:
                TextBox.SetText(10);
                break;

            default:
                throw new Exception("Murfy is not set to be used in the current map");
        }
    }

    private bool FUN_08071fb0()
    {
        if (Byte_8A == 0 && GameInfo.MapId == MapId.WoodLight_M1)
        {
            if (GameInfo.LastGreenLumAlive == 0)
            {
                GameInfo.LastGreenLumAlive++;
                GameInfo.CheckpointPosition = new Vector2(130, 264);
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        if (base.ProcessMessageImpl(message, param))
            return false;

        // Handle messages
        switch (message)
        {
            case Message.Murfy_Spawn:
                ShouldSpawn = true;
                if (MainActorPosition == Vector2.Zero)
                    MainActorPosition = MainActor.Position;
                return false;

            default:
                return false;
        }
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (Fsm.EqualsAction(Fsm_Init))
        {
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__MurfHeli_Mix01);
        }
        else
        {
            base.Draw(animationPlayer, forceDraw);

            if (AnimatedObject.IsFramed)
            {
                if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__MurfHeli_Mix01))
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MurfHeli_Mix01);
            }
            else
            {
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__MurfHeli_Mix01);
            }
        }
    }
}