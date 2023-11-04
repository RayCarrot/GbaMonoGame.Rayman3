using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace OnyxCs.Gba;

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

    public static bool Check(GbaInput gbaInput)
    {
        return Check(GbaButtonMapping[gbaInput]);
    }

    public static bool Check(Keys input)
    {
        return KeyboardState.IsKeyDown(input);
    }

    public static bool CheckSingle(Keys input)
    {
        return KeyboardState.IsKeyDown(input) && PreviousKeyboardState.IsKeyUp(input);
    }

    public static bool CheckSingle(GbaInput gbaInput)
    {
        return CheckSingle(GbaButtonMapping[gbaInput]);
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