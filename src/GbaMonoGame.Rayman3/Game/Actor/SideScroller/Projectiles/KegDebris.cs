﻿using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class KegDebris : ActionActor
{
    public KegDebris(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        Timer = 0;
        Fsm.ChangeAction(Fsm_Default);
    }

    private byte Timer { get; set; }
}