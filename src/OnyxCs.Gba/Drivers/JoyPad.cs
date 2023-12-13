using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace OnyxCs.Gba;

// TODO: Rename Check methods to these? This matches how Unity does it and might be less confusing.
// GetInputDown
// GetInputUp
// GetInput

// TODO: Restructure this? In the game JoyPad is an abstract class with different implementations. For example the multiplayer
//       mode has its own JoyPad. We probably don't need that, although supporting multiple input methods such as controllers
//       and keyboard would be good. It also seems a bit weird having mouse support in a class named "JoyPad".
public static class JoyPad
{
    // TODO: Allow this to be configured
    private static Dictionary<GbaInput, Keys> GbaButtonMapping { get; } = new Dictionary<GbaInput, Keys>()
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

    private static KeyboardState PreviousKeyboardState { get; set; }
    private static KeyboardState KeyboardState { get; set; }
    private static MouseState PreviousMouseState { get; set; }
    private static MouseState MouseState { get; set; }

    public static Vector2 MouseOffset { get; set; }

    public static bool Check(GbaInput gbaInput)
    {
        // Cancel out if opposite directions are pressed
        if (gbaInput is GbaInput.Left or GbaInput.Right && Check(GbaButtonMapping[GbaInput.Left]) && Check(GbaButtonMapping[GbaInput.Right]))
            return false;
        if (gbaInput is GbaInput.Up or GbaInput.Down && Check(GbaButtonMapping[GbaInput.Up]) && Check(GbaButtonMapping[GbaInput.Down]))
            return false;

        return Check(GbaButtonMapping[gbaInput]);
    }

    public static bool Check(Keys input)
    {
        return KeyboardState.IsKeyDown(input);
    }

    public static bool CheckSingle(GbaInput gbaInput)
    {
        // Cancel out if opposite directions are pressed
        if (gbaInput is GbaInput.Left or GbaInput.Right && CheckSingle(GbaButtonMapping[GbaInput.Left]) && CheckSingle(GbaButtonMapping[GbaInput.Right]))
            return false;
        if (gbaInput is GbaInput.Up or GbaInput.Down && CheckSingle(GbaButtonMapping[GbaInput.Up]) && CheckSingle(GbaButtonMapping[GbaInput.Down]))
            return false;

        return CheckSingle(GbaButtonMapping[gbaInput]);
    }

    public static bool CheckSingle(Keys input)
    {
        return KeyboardState.IsKeyDown(input) && PreviousKeyboardState.IsKeyUp(input);
    }

    public static bool CheckSingleReleased(GbaInput gbaInput)
    {
        // Cancel out if opposite directions are pressed
        if (gbaInput is GbaInput.Left or GbaInput.Right && CheckSingleReleased(GbaButtonMapping[GbaInput.Left]) && CheckSingleReleased(GbaButtonMapping[GbaInput.Right]))
            return false;
        if (gbaInput is GbaInput.Up or GbaInput.Down && CheckSingleReleased(GbaButtonMapping[GbaInput.Up]) && CheckSingleReleased(GbaButtonMapping[GbaInput.Down]))
            return false;

        return CheckSingleReleased(GbaButtonMapping[gbaInput]);
    }

    public static bool CheckSingleReleased(Keys input)
    {
        return KeyboardState.IsKeyUp(input) && PreviousKeyboardState.IsKeyDown(input);
    }

    public static bool IsMouseOnScreen() => Engine.ScreenCamera.VisibleArea.Contains(GetMousePosition());

    public static Vector2 GetMousePosition()
    {
        return Engine.ScreenCamera.ToGamePosition(MouseState.Position.ToVector2() + MouseOffset);
    }

    public static Vector2 GetMousePositionDelta()
    {
        return Engine.ScreenCamera.ToGamePosition(MouseState.Position.ToVector2()) -
               Engine.ScreenCamera.ToGamePosition(PreviousMouseState.Position.ToVector2());
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