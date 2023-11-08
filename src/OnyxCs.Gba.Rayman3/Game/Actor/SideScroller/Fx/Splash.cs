﻿using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed partial class Splash : BaseActor
{
    public Splash(int id, ActorResource actorResource) : base(id, actorResource)
    {
        Fsm.ChangeAction(Fsm_Default);
    }
}