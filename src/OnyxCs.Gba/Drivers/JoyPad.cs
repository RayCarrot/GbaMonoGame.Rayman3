using Microsoft.Xna.Framework.Input;

namespace OnyxCs.Gba;

public static class JoyPad
{
    private static KeyboardState PreviousState { get; set; }
    private static KeyboardState State { get; set; }

    public static bool Check(GbaInput gbaInput)
    {
        // TODO: Add all inputs and read key bindings from config. Also support a controller.

        if ((gbaInput & GbaInput.Start) != 0 && State.IsKeyDown(Keys.V))
            return true;

        return false;
    }

    public static bool Check(Keys input)
    {
        return State.IsKeyDown(input);
    }

    public static bool CheckSingle(Keys input)
    {
        return State.IsKeyDown(input) && PreviousState.IsKeyUp(input);
    }

    public static void Scan()
    {
        PreviousState = State;
        State = Keyboard.GetState();
    }
}