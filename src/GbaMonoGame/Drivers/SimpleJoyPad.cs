using System.Collections.Generic;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework.Input;

namespace GbaMonoGame;

public class SimpleJoyPad
{
    // TODO: Allow this to be configured
    private static Dictionary<GbaInput, Keys> GbaButtonMapping { get; } = new()
    {
        [GbaInput.A] = Keys.Space,
        [GbaInput.B] = Keys.S,
        [GbaInput.Select] = Keys.C,
        [GbaInput.Start] = Keys.V,
        [GbaInput.Right] = Keys.Right,
        [GbaInput.Left] = Keys.Left,
        [GbaInput.Up] = Keys.Up,
        [GbaInput.Down] = Keys.Down,
        [GbaInput.R] = Keys.W,
        [GbaInput.L] = Keys.Q,
    };

    private GbaInput KeyStatus { get; set; }
    private GbaInput KeyTriggers { get; set; } // Just pressed

    private GbaInput[] ReplayData { get; set; }
    private int ReplayDataIndex { get; set; }

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

            foreach (KeyValuePair<GbaInput, Keys> input in GbaButtonMapping)
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