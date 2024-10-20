﻿using System;

namespace GbaMonoGame.Engine2d;

public class MechModel
{
    public MechModel()
    {
        InitActions = new[] 
        {
            Reset,
            UseConstantSpeed,
            None,
            SetSpeedXY,
            SetSpeedX_ResetSpeedY,
            SetSpeedY_ResetSpeedX,
            SetSpeedX,
            SetSpeedY,
            SetAccelerationXY_SetTargetSpeedXY,
            SetAccelerationX_SetTargetSpeedX_ResetSpeedY,
            SetAccelerationY_SetTargetSpeedY_ResetSpeedX,
            SetAccelerationX_SetTargetSpeedX,
            SetAccelerationY_SetTargetSpeedY,
            SetSpeedXY_SetAccelerationXY_SetTargetSpeedXY,
            SetSpeedX_SetAccelerationX_SetTargetSpeedX_ResetSpeedY,
            SetSpeedY_SetAccelerationY_SetTargetSpeedY_ResetSpeedX,
            SetSpeedX_SetAccelerationX_SetTargetSpeedX,
            SetSpeedY_SetAccelerationY_SetTargetSpeedY,

            // TODO: Implement these
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
            Dummy,
        };
    }

    private Action<float[], int>[] InitActions { get; }

    public Vector2 Speed { get; set; }
    public Vector2 Acceleration { get; set; }
    public Vector2 TargetSpeed { get; set; }
    public Func<Vector2> UpdateSpeedAction { get; set; }

    private Vector2 SetConstSpeedXY()
    {
        return Speed;
    }

    private Vector2 SetAcceleratedSpeedX()
    {
        Speed = Speed with { X = Speed.X + Acceleration.X };

        if (Acceleration.X <= 0)
        {
            if (Speed.X <= TargetSpeed.X)
            {
                Speed = Speed with { X = TargetSpeed.X };
                UpdateSpeedAction = SetConstSpeedXY;
            }
        }
        else
        {
            if (Speed.X >= TargetSpeed.X)
            {
                Speed = Speed with { X = TargetSpeed.X };
                UpdateSpeedAction = SetConstSpeedXY;
            }
        }

        return SetConstSpeedXY();
    }

    private Vector2 SetAcceleratedSpeedY()
    {
        Speed = Speed with { Y = Speed.Y + Acceleration.Y };

        if (Acceleration.Y <= 0)
        {
            if (Speed.Y <= TargetSpeed.Y)
            {
                Speed = Speed with { Y = TargetSpeed.Y };
                UpdateSpeedAction = SetConstSpeedXY;
            }
        }
        else
        {
            if (Speed.Y >= TargetSpeed.Y)
            {
                Speed = Speed with { Y = TargetSpeed.Y };
                UpdateSpeedAction = SetConstSpeedXY;
            }
        }

        return SetConstSpeedXY();
    }

    private Vector2 SetAcceleratedSpeedXY()
    {
        Speed += Acceleration;

        bool stopX = false;
        bool stopY = false;

        if (Acceleration.X <= 0)
        {
            if (Speed.X <= TargetSpeed.X)
            {
                Speed = Speed with { X = TargetSpeed.X };
                stopX = true;
            }
        }
        else
        {
            if (Speed.X >= TargetSpeed.X)
            {
                Speed = Speed with { X = TargetSpeed.X };
                stopX = true;
            }
        }

        if (Acceleration.Y <= 0)
        {
            if (Speed.Y <= TargetSpeed.Y)
            {
                Speed = Speed with { Y = TargetSpeed.Y };
                stopY = true;
            }
        }
        else
        {
            if (Speed.Y >= TargetSpeed.Y)
            {
                Speed = Speed with { Y = TargetSpeed.Y };
                stopY = true;
            }
        }

        if (stopX)
        {
            if (stopY)
            {
                UpdateSpeedAction = SetConstSpeedXY;
            }
            else
            {
                UpdateSpeedAction = SetAcceleratedSpeedY;
            }
        }
        else
        {
            if (stopY)
            {
                UpdateSpeedAction = SetAcceleratedSpeedX;
            }
        }

        return SetConstSpeedXY();
    }

    // Temporary
    private void Dummy(float[] mechParams, int offset)
    {
        throw new NotImplementedException();
    }

    private void Reset(float[] mechParams, int offset)
    {
        Reset();
    }

    private void UseConstantSpeed(float[] mechParams, int offset)
    {
        UpdateSpeedAction = SetConstSpeedXY;
    }

    private void None(float[] mechParams, int offset) { }

    private void SetSpeedXY(float[] mechParams, int offset)
    {
        SetSpeedX(mechParams, offset + 0);
        SetSpeedY(mechParams, offset + 1);
    }

    private void SetSpeedX_ResetSpeedY(float[] mechParams, int offset)
    {
        SetSpeedX(mechParams, offset);
        Speed = Speed with { Y = 0 };
    }

    private void SetSpeedY_ResetSpeedX(float[] mechParams, int offset)
    {
        SetSpeedY(mechParams, offset);
        Speed = Speed with { X = 0 };
    }

    private void SetSpeedX(float[] mechParams, int offset)
    {
        Speed = Speed with { X = mechParams[offset] };
        UpdateSpeedAction = SetConstSpeedXY;
    }

    private void SetSpeedY(float[] mechParams, int offset)
    {
        Speed = Speed with { Y = mechParams[offset] };
        UpdateSpeedAction = SetConstSpeedXY;
    }

    private void SetAccelerationXY_SetTargetSpeedXY(float[] mechParams, int offset)
    {
        SetAccelerationX_SetTargetSpeedX(mechParams, offset + 0);
        SetAccelerationY_SetTargetSpeedY(mechParams, offset + 2);
        UpdateSpeedAction = SetAcceleratedSpeedXY;
    }

    private void SetAccelerationX_SetTargetSpeedX_ResetSpeedY(float[] mechParams, int offset)
    {
        SetAccelerationX_SetTargetSpeedX(mechParams, offset);
        Speed = Speed with { Y = 0 };
    }

    private void SetAccelerationY_SetTargetSpeedY_ResetSpeedX(float[] mechParams, int offset)
    {
        SetAccelerationY_SetTargetSpeedY(mechParams, offset);
        Speed = Speed with { X = 0 };
    }

    private void SetAccelerationX_SetTargetSpeedX(float[] mechParams, int offset)
    {
        Acceleration = Acceleration with { X = mechParams[offset + 0] };
        TargetSpeed = TargetSpeed with { X = mechParams[offset + 1] };
        UpdateSpeedAction = SetAcceleratedSpeedX;
    }

    private void SetAccelerationY_SetTargetSpeedY(float[] mechParams, int offset)
    {
        Acceleration = Acceleration with { Y = mechParams[offset + 0] };
        TargetSpeed = TargetSpeed with { Y = mechParams[offset + 1] };
        UpdateSpeedAction = SetAcceleratedSpeedY;
    }

    private void SetSpeedXY_SetAccelerationXY_SetTargetSpeedXY(float[] mechParams, int offset)
    {
        SetSpeedX_SetAccelerationX_SetTargetSpeedX(mechParams, offset + 0);
        SetSpeedY_SetAccelerationY_SetTargetSpeedY(mechParams, offset + 3);
        UpdateSpeedAction = SetAcceleratedSpeedXY;
    }

    private void SetSpeedX_SetAccelerationX_SetTargetSpeedX_ResetSpeedY(float[] mechParams, int offset)
    {
        SetSpeedX_SetAccelerationX_SetTargetSpeedX(mechParams, offset);
        Speed = Speed with { Y = 0 };
    }

    private void SetSpeedY_SetAccelerationY_SetTargetSpeedY_ResetSpeedX(float[] mechParams, int offset)
    {
        SetSpeedY_SetAccelerationY_SetTargetSpeedY(mechParams, offset);
        Speed = Speed with { X = 0 };
    }

    private void SetSpeedX_SetAccelerationX_SetTargetSpeedX(float[] mechParams, int offset)
    {
        Speed = Speed with { X = mechParams[offset + 0] };
        Acceleration = Acceleration with { X = mechParams[offset + 1] };
        TargetSpeed = TargetSpeed with { X = mechParams[offset + 2] };
        UpdateSpeedAction = SetAcceleratedSpeedX;
    }

    private void SetSpeedY_SetAccelerationY_SetTargetSpeedY(float[] mechParams, int offset)
    {
        Speed = Speed with { Y = mechParams[offset + 0] };
        Acceleration = Acceleration with { Y = mechParams[offset + 1] };
        TargetSpeed = TargetSpeed with { Y = mechParams[offset + 2] };
        UpdateSpeedAction = SetAcceleratedSpeedY;
    }

    public void Init(int type, float[] mechParams)
    {
        InitActions[type](mechParams, 0);
    }

    public void Reset()
    {
        Speed = Vector2.Zero;
        Acceleration = Vector2.Zero;
        TargetSpeed = Vector2.Zero;
        UpdateSpeedAction = SetConstSpeedXY;
    }
}