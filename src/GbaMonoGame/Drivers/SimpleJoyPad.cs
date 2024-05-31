using System.Collections.Generic;
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

    private GbaInput Inputs { get; set; }
    private GbaInput NewInputs { get; set; }

    private GbaInput[] ReplayData { get; set; }
    private int ReplayDataIndex { get; set; }

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
                if (InputManager.Check(input.Value))
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

        NewInputs = inputs ^ Inputs;
        Inputs = inputs;
    }

    public bool Check(GbaInput gbaInput) => (Inputs & gbaInput) != 0;
    public bool CheckSingle(GbaInput gbaInput) => (gbaInput & Inputs & NewInputs) != 0;
    public bool CheckSingleReleased(GbaInput gbaInput) => (gbaInput & ~Inputs & NewInputs) != 0;
}