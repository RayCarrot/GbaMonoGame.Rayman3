using Microsoft.Xna.Framework.Input;

namespace OnyxCs.Gba;

public static class JoyPad
{
    private static KeyboardState State { get; set; }

    public static bool Check(Input input)
    {
        // TODO: Add all inputs and read key bindings from config. Also support a controller.

        if ((input & Input.Start) != 0 && State.IsKeyDown(Keys.V))
            return true;

        return false;
    }

    public static void Scan()
    {
        State = Keyboard.GetState();
    }
}