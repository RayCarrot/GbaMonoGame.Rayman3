using System;
using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class PurpleLum
{
    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                Box viewBox = GetViewBox();

                if (RSMultiplayer.IsActive && Engine.Settings.Platform == Platform.NGage)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    Rayman rayman = (Rayman)Scene.MainActor;

                    // Why is this code so weird with how the view box is handled?
                    foreach (RaymanBody fist in rayman.GetActiveFists())
                    {
                        Box detectionBox = fist.GetDetectionBox();
                        if (detectionBox.Intersects(viewBox))
                        {
                            viewBox = new Box(viewBox.MinX + 16, viewBox.MinY + 8, viewBox.MaxX - 16, viewBox.MaxY + 4);

                            if (detectionBox.Intersects(viewBox))
                            {
                                rayman.ProcessMessage(Message.Main_BeginSwing, this);
                                fist.ProcessMessage(Message.RaymanBody_FinishedAttack);
                                break;
                            }
                        }
                    }
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}