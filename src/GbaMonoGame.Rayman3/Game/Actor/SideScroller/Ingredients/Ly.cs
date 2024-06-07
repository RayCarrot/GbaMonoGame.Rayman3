﻿using System;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Ly : MovableActor
{
    public Ly(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        State.SetTo(Fsm_Init);
    }

    public TextBoxDialog TextBox { get; set; }
    public ushort Timer { get; set; }

    private void SetText()
    {
        TextBox.SetText(GameInfo.MapId switch
        {
            MapId.WoodLight_M2 => 0,
            MapId.BossMachine => 1,
            MapId.EchoingCaves_M2 => 2,
            MapId.SanctuaryOfStoneAndFire_M3 => 3,
            MapId.BossRockAndLava => 4,
            MapId.BossScaleMan => 5,
            _ => throw new Exception("Ly is not set to be used in the current map")
        });
    }

    private void SetPowerAndReplayData()
    {
        Rayman rayman = (Rayman)Scene.MainActor;

        switch (GameInfo.MapId)
        {
            case MapId.WoodLight_M2:
                JoyPad.SetReplayData(Engine.Loader.Rayman3_NewPower1Replay.Inputs);
                rayman.SetPowers(Power.DoubleFist);
                break;

            case MapId.BossMachine:
                JoyPad.SetReplayData(Engine.Loader.Rayman3_NewPower2Replay.Inputs);
                rayman.SetPowers(Power.Grab);
                break;

            case MapId.EchoingCaves_M2:
                JoyPad.SetReplayData(Engine.Loader.Rayman3_NewPower3Replay.Inputs);
                rayman.SetPowers(Power.Climb);
                break;

            case MapId.SanctuaryOfStoneAndFire_M3:
                JoyPad.SetReplayData(Engine.Loader.Rayman3_NewPower4Replay.Inputs);
                rayman.SetPowers(Power.SuperHelico);
                break;

            case MapId.BossRockAndLava:
                JoyPad.SetReplayData(Engine.Loader.Rayman3_NewPower5Replay.Inputs);
                rayman.SetPowers(Power.BodyShot);
                break;

            case MapId.BossScaleMan:
                JoyPad.SetReplayData(Engine.Loader.Rayman3_NewPower6Replay.Inputs);
                rayman.SetPowers(Power.SuperFist);
                break;

            default:
                throw new Exception("Ly is not set to be used in the current map");
        }
    }
}