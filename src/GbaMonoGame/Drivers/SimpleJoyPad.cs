using System.Collections.Generic;
using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame;

public class SimpleJoyPad
{
    private static Dictionary<GbaInput, Input> GbaInputMapping { get; } = new()
    {
        [GbaInput.A] = Input.Gba_A,
        [GbaInput.B] = Input.Gba_B,
        [GbaInput.Select] = Input.Gba_Select,
        [GbaInput.Start] = Input.Gba_Start,
        [GbaInput.Right] = Input.Gba_Right,
        [GbaInput.Left] = Input.Gba_Left,
        [GbaInput.Up] = Input.Gba_Up,
        [GbaInput.Down] = Input.Gba_Down,
        [GbaInput.R] = Input.Gba_R,
        [GbaInput.L] = Input.Gba_L,
    };

    public GbaInput KeyStatus { get; set; }
    public GbaInput KeyTriggers { get; set; } // Just pressed

    public GbaInput[] ReplayData { get; set; }
    public int ReplayDataIndex { get; set; }

    public bool IsReplayFinished => KeyStatus == GbaInput.None;

    public void SetReplayData(GbaInput[] replayData)
    {
        ReplayData = replayData;
        ReplayDataIndex = 0;
    }

    public void Scan()
    {
        GbaInput inputs;

        if (ReplayData == null)
        {
            inputs = GbaInput.Valid;

            foreach (KeyValuePair<GbaInput, Input> input in GbaInputMapping)
            {
                if (InputManager.IsButtonPressed(input.Value))
                    inputs |= input.Key;
            }
        }
        else
        {
            inputs = ReplayData[ReplayDataIndex];

            if (inputs == GbaInput.None)
                ReplayData = null;
            else
                ReplayDataIndex++;
        }

        // Cancel out if opposite directions are pressed
        if ((inputs & (GbaInput.Right | GbaInput.Left)) == (GbaInput.Right | GbaInput.Left))
            inputs &= ~(GbaInput.Right | GbaInput.Left);
        if ((inputs & (GbaInput.Up | GbaInput.Down)) == (GbaInput.Up | GbaInput.Down))
            inputs &= ~(GbaInput.Up | GbaInput.Down);

        KeyTriggers = inputs ^ KeyStatus;
        KeyStatus = inputs;
    }

    public bool IsButtonPressed(GbaInput gbaInput) => (KeyStatus & gbaInput) != 0;
    public bool IsButtonReleased(GbaInput gbaInput) => (KeyStatus & gbaInput) == 0;
    public bool IsButtonJustPressed(GbaInput gbaInput) => (gbaInput & KeyStatus & KeyTriggers) != 0;
    public bool IsButtonJustReleased(GbaInput gbaInput) => (gbaInput & ~KeyStatus & KeyTriggers) != 0;
}