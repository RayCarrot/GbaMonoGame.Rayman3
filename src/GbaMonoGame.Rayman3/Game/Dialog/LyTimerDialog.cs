﻿using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class LyTimerDialog : Dialog
{
    private TimerBar TimerBar { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param) => false;

    public override void Load()
    {
        TimerBar = new TimerBar();
        TimerBar.Load();
        TimerBar.Set();
    }

    public override void Init() { }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        TimerBar.DrawTime(animationPlayer, GameInfo.RemainingTime);
    }
}