﻿using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public partial class Piranha : MovableActor
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        Move_Right = 0,
        Move_Left = 1,
        Dying1_Right = 2,
        Dying1_Left = 3,
        Dying2_Right = 4,
        Dying2_Left = 5,
    }
}