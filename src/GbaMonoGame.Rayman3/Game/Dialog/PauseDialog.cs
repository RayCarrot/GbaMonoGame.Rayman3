using System;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class PauseDialog : Dialog
{
    protected override bool ProcessMessageImpl(object sender, Message message, object param) => false;

    public override void Load()
    {
        throw new NotImplementedException();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        throw new NotImplementedException();
    }
}