﻿namespace OnyxCs.Gba.Rayman3;

public class Act1 : Act
{
    public override void Init()
    {
        Init(Engine.Loader.Act1);
    }

    public override void Step()
    {
        base.Step();

        if (IsFinished)
            GameInfo.LoadLevel(MapId.WoodLight_M1);
    }
}