using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OnyxCs.Gba;

public static class JoyPad
{
    private static KeyboardState PreviousKeyboardState { get; set; }
    private static KeyboardState KeyboardState { get; set; }
    private static MouseState PreviousMouseState { get; set; }
    private static MouseState MouseState { get; set; }

    public static bool Check(GbaInput gbaInput)
    {
        // TODO: Add all inputs and read key bindings from config. Also support a controller.

        if ((gbaInput & GbaInput.Start) != 0 && KeyboardState.IsKeyDown(Keys.V))
            return true;

        return false;
    }

    public static bool Check(Keys input)
    {
        return KeyboardState.IsKeyDown(input);
    }

    public static bool CheckSingle(Keys input)
    {
        return KeyboardState.IsKeyDown(input) && PreviousKeyboardState.IsKeyUp(input);
    }

    public static Vector2 GetMousePosition()
    {
        return Gfx.GfxCamera.ToGamePosition(MouseState.Position.ToVector2());
    }

    public static Vector2 GetMousePositionDelta()
    {
        return Gfx.GfxCamera.ToGamePosition(MouseState.Position.ToVector2()) - 
               Gfx.GfxCamera.ToGamePosition(PreviousMouseState.Position.ToVector2());
    }

    public static int GetMouseWheelDelta()
    {
        return MouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue;
    }

    public static MouseState GetMouseState() => MouseState;

    public static void Scan()
    {
        PreviousKeyboardState = KeyboardState;
        KeyboardState = Keyboard.GetState();

        PreviousMouseState = MouseState;
        MouseState = Mouse.GetState();
    }
}