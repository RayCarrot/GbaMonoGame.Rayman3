using System;

namespace OnyxCs.Gba.Engine2d;

public class Mechanic
{
    public Mechanic()
    {
        InitActions = new[] 
        {
            Reset,
            Continue,
            None,
            SetSpeedXY,
            SetSpeedX_ResetSpeedY,
            SetSpeedY_ResetSpeedX,
            SetSpeedX,
            SetSpeedY,
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
    public Vector2 Max { get; set; }
    public Func<Vector2, Vector2> UpdateSpeedAction { get; set; }

    private Vector2 SetConstSpeedXY(Vector2 speed)
    {
        return Speed;
    }

    // Temporary
    private void Dummy(float[] mechParams, int offset)
    {

    }

    private void Reset(float[] mechParams, int offset)
    {
        Speed = Vector2.Zero;
        Acceleration = Vector2.Zero;
        Max = Vector2.Zero;
        UpdateSpeedAction = SetConstSpeedXY;
    }

    private void Continue(float[] mechParams, int offset)
    {
        UpdateSpeedAction = SetConstSpeedXY;
    }

    private void None(float[] mechParams, int offset) { }

    private void SetSpeedXY(float[] mechParams, int offset)
    {
        SetSpeedX(mechParams, offset);
        SetSpeedX(mechParams, offset + 1);
    }

    private void SetSpeedX_ResetSpeedY(float[] mechParams, int offset)
    {
        SetSpeedX(mechParams, offset);
        Speed = new Vector2(Speed.X, 0);
    }

    private void SetSpeedY_ResetSpeedX(float[] mechParams, int offset)
    {
        SetSpeedY(mechParams, offset);
        Speed = new Vector2(0, Speed.Y);
    }

    private void SetSpeedX(float[] mechParams, int offset)
    {
        Speed = new Vector2(mechParams[offset], Speed.Y);
        UpdateSpeedAction = SetConstSpeedXY;
    }

    private void SetSpeedY(float[] mechParams, int offset)
    {
        Speed = new Vector2(Speed.X, mechParams[offset]);
        UpdateSpeedAction = SetConstSpeedXY;
    }

    public void Init(int type, float[] mechParams)
    {
        InitActions[type](mechParams, 0);
    }
}