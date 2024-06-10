using System;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public partial class CameraSideScroller
{
    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (LinkedObject != null)
                {
                    PreviousLinkedObjectPosition = LinkedObject.Position;
                    IsFacingRight = LinkedObject.IsFacingRight;

                    UpdateTargetX();
                    Speed = new Vector2(TargetX < LinkedObject.ScreenPosition.X ? 1 : -1, Speed.Y);
                }

                Timer = 0;
                break;

            case FsmAction.Step:
                // Reset speed y
                Speed = new Vector2(Speed.X, 0);

                UpdateTargetX();

                float linkedObjDeltaX = LinkedObject.Position.X - PreviousLinkedObjectPosition.X;

                // If we're within 4 pixels of the target...
                if (Math.Abs(LinkedObject.ScreenPosition.X - TargetX) <= 4)
                {
                    // Follow the linked object's movement
                    Speed = new Vector2(linkedObjDeltaX, Speed.Y);
                }
                // If far away from the target...
                else
                {
                    Timer++;

                    // Reset speed x if we're switching direction to move
                    if ((LinkedObject.ScreenPosition.X < TargetX && Speed.X > 0) ||
                        (LinkedObject.ScreenPosition.X > TargetX && Speed.X < 0))
                    {
                        Speed = new Vector2(0, Speed.Y);
                    }

                    float dir = LinkedObject.ScreenPosition.X > TargetX ? 1 : -1;

                    // If the linked object is moving faster than 2...
                    if (Math.Abs(linkedObjDeltaX) > 2)
                    {
                        // Move the camera alongside the linked object with a speed of 6
                        Speed = new Vector2(dir * 6, Speed.Y);
                    }
                    // If the linked object is moving and
                    // the timer is greater than or equal to 3 and
                    // the absolute camera speed is less than 4...
                    else if (LinkedObject.Speed.X != 0 && Timer >= 3 && Math.Abs(Speed.X) < 4)
                    {
                        // Move the camera with a speed of 0.5 and reset the timer
                        Speed += new Vector2(dir * 0.5f, 0);
                        Timer = 0;
                    }
                    // If the linked object is not moving...
                    else if (LinkedObject.Speed.X == 0)
                    {
                        // If the linked object is within 40 pixels of the horizontal offset...
                        if ((LinkedObject.IsFacingRight &&
                             ScaledHorizontalOffset + 40 > LinkedObject.ScreenPosition.X &&
                             ScaledHorizontalOffset <= LinkedObject.ScreenPosition.X) ||
                          (LinkedObject.IsFacingLeft &&
                           Scene.Resolution.X - 40 - ScaledHorizontalOffset < LinkedObject.ScreenPosition.X &&
                           Scene.Resolution.X - ScaledHorizontalOffset > LinkedObject.ScreenPosition.X))
                        {
                            // If timer is greater than 2, slow down the speed if it's absolute greater than 1
                            if (Timer > 2 && Math.Abs(Speed.X) > 1)
                            {
                                Speed -= new Vector2(dir * 0.5f, 0);
                                Timer = 0;
                            }
                        }
                        else
                        {
                            // If the timer is greater than 5, increase the speed if it's absolute less than 4
                            if (Timer > 5 && Math.Abs(Speed.X) < 4)
                            {
                                Speed += new Vector2(dir * 0.5f, 0);
                                Timer = 0;
                            }
                        }
                    }
                }

                float linkedObjDeltaY = LinkedObject.Position.Y - PreviousLinkedObjectPosition.Y;

                // Do not follow Y (unless near the edge). Used when jumping for example.
                if (FollowYMode == FollowMode.DoNotFollow)
                {
                    float yOff = Scene.Resolution.Y - Engine.GameViewPort.OriginalGameResolution.Y;

                    if ((LinkedObject.ScreenPosition.Y < 70 + yOff / 2 && linkedObjDeltaY < 0) ||
                        (LinkedObject.ScreenPosition.Y > 130 + yOff && linkedObjDeltaY > 0))
                    {
                        Speed = new Vector2(Speed.X, linkedObjDeltaY);
                    }

                }
                // Follow Y, the default
                else
                {
                    if (Math.Abs(LinkedObject.ScreenPosition.Y - ScaledTargetY) <= 4)
                    {
                        Speed = new Vector2(Speed.X, linkedObjDeltaY);

                        if (FollowYMode == FollowMode.FollowUntilNearby)
                            FollowYMode = FollowMode.DoNotFollow;
                    }
                    else
                    {
                        if (ScaledTargetY < LinkedObject.ScreenPosition.Y)
                        {
                            if (linkedObjDeltaY >= 2)
                            {
                                Speed = new Vector2(Speed.X, 5);
                            }
                            else if (LinkedObject.ScreenPosition.Y - ScaledTargetY >= 21)
                            {
                                Speed = new Vector2(Speed.X, 3);
                            }
                            else if (linkedObjDeltaY >= 1)
                            {
                                Speed = new Vector2(Speed.X, 2);
                            }
                            else
                            {
                                Speed = new Vector2(Speed.X, 1);
                            }
                        }
                        else
                        {
                            if (linkedObjDeltaY <= -2)
                            {
                                Speed = new Vector2(Speed.X, -5);
                            }
                            else if (LinkedObject.ScreenPosition.Y - ScaledTargetY <= -21)
                            {
                                Speed = new Vector2(Speed.X, -3);
                            }
                            else if (linkedObjDeltaY <= -1)
                            {
                                Speed = new Vector2(Speed.X, -2);
                            }
                            else
                            {
                                Speed = new Vector2(Speed.X, -1);
                            }
                        }
                    }
                }

                Speed = VerticalShake(Speed);

                // Clamp speed
                Speed = new Vector2(Math.Clamp(Speed.X, -7, 7), Math.Clamp(Speed.Y, -7, 7));

                TgxCamera2D tgxCam = ((TgxPlayfield2D)Scene.Playfield).Camera;
                TgxCluster mainCluster = tgxCam.GetMainCluster();

                tgxCam.Position += Speed;

                PreviousLinkedObjectPosition = LinkedObject.Position;

                // Reset if changed direction
                if (!mainCluster.IsOnLimit(Edge.Left) &&
                    !mainCluster.IsOnLimit(Edge.Right) &&
                    LinkedObject.IsFacingRight != IsFacingRight)
                    State.MoveTo(Fsm_Default);

                if (field16_0x2e == 4)
                {
                    HorizontalOffset = CameraOffset.Default;
                    field16_0x2e = 1;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    // TODO: Implement
    private void Fsm_MoveToTarget(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:

                break;

            case FsmAction.Step:

                break;

            case FsmAction.UnInit:

                break;
        }
    }
}