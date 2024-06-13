using Microsoft.Xna.Framework.Input;

namespace GbaMonoGame;

// TODO: Rename Check methods to these? Same in JoyPad. This matches how Unity does it and might be less confusing.
// GetInputDown
// GetInputUp
// GetInput

// TODO: Check usage of this. We don't want to hard-code key inputs. Use some abstraction to allow remapping.
public static class InputManager
{
    private static KeyboardState PreviousKeyboardState { get; set; }
    private static KeyboardState KeyboardState { get; set; }
    private static MouseState PreviousMouseState { get; set; }
    private static MouseState MouseState { get; set; }

    public static Vector2 MouseOffset { get; set; }

    public static bool IsButtonPressed(Keys input) => KeyboardState.IsKeyDown(input);
    public static bool IsButtonReleased(Keys input) => KeyboardState.IsKeyUp(input);
    public static bool IsButtonJustPressed(Keys input) => KeyboardState.IsKeyDown(input) && PreviousKeyboardState.IsKeyUp(input);
    public static bool IsButtonJustReleased(Keys input) => KeyboardState.IsKeyUp(input) && PreviousKeyboardState.IsKeyDown(input);

    public static bool IsMouseOnScreen(GfxCamera camera) => camera.IsVisible(GetMousePosition(camera));
    public static Vector2 GetMousePosition(GfxCamera camera) => camera.ToWorldPosition(MouseState.Position.ToVector2() + MouseOffset);
    public static Vector2 GetMousePositionDelta(GfxCamera camera) => camera.ToWorldPosition(MouseState.Position.ToVector2()) -
                                                                     camera.ToWorldPosition(PreviousMouseState.Position.ToVector2());
    public static int GetMouseWheelDelta() => MouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue;
    public static MouseState GetMouseState() => MouseState;

    public static void Update()
    {
        PreviousKeyboardState = KeyboardState;
        KeyboardState = Keyboard.GetState();

        PreviousMouseState = MouseState;
        MouseState = Mouse.GetState();
    }
}