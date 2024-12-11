﻿using System;
using BinarySerializer.Ubisoft.GbaEngine;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace GbaMonoGame;

public static class InputManager
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

    private static KeyboardState PreviousKeyboardState { get; set; }
    private static KeyboardState KeyboardState { get; set; }
    private static MouseState PreviousMouseState { get; set; }
    private static MouseState MouseState { get; set; }

    public static Vector2 MouseOffset { get; set; }

    public static Keys GetDefaultKey(Input input)
    {
        return input switch
        {
            // GBA
            Input.Gba_A => Keys.Space,
            Input.Gba_B => Keys.S,
            Input.Gba_Select => Keys.C,
            Input.Gba_Start => Keys.V,
            Input.Gba_Right => Keys.Right,
            Input.Gba_Left => Keys.Left,
            Input.Gba_Up => Keys.Up,
            Input.Gba_Down => Keys.Down,
            Input.Gba_R => Keys.W,
            Input.Gba_L => Keys.Q,

            // Menu
            Input.Menu_Right => Keys.Right,
            Input.Menu_Left => Keys.Left,
            Input.Menu_Up => Keys.Up,
            Input.Menu_Down => Keys.Down,
            Input.Menu_Confirm => Keys.Space,
            Input.Menu_Back => Keys.Back,

            // Debug
            Input.Debug_ToggleBoxes => Keys.B,
            Input.Debug_ToggleCollision => Keys.T,
            Input.Debug_ToggleNoClip => Keys.Z,
            
            _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
        };
    }
    public static Keys GetKey(Input input) => Engine.Config.Controls[input];

    public static bool IsButtonPressed(Keys input) => KeyboardState.IsKeyDown(input);
    public static bool IsButtonReleased(Keys input) => KeyboardState.IsKeyUp(input);
    public static bool IsButtonJustPressed(Keys input) => KeyboardState.IsKeyDown(input) && PreviousKeyboardState.IsKeyUp(input);
    public static bool IsButtonJustReleased(Keys input) => KeyboardState.IsKeyUp(input) && PreviousKeyboardState.IsKeyDown(input);

    public static bool IsButtonPressed(Input input) => IsButtonPressed(GetKey(input));
    public static bool IsButtonReleased(Input input) => IsButtonReleased(GetKey(input));
    public static bool IsButtonJustPressed(Input input) => IsButtonJustPressed(GetKey(input));
    public static bool IsButtonJustReleased(Input input) => IsButtonJustReleased(GetKey(input));

    public static GbaInput GetGbaInputs()
    {
        GbaInput inputs = GbaInput.Valid;

        foreach (KeyValuePair<GbaInput, Input> input in GbaInputMapping)
        {
            if (IsButtonPressed(input.Value))
                inputs |= input.Key;
        }

        // Cancel out if opposite directions are pressed
        if ((inputs & (GbaInput.Right | GbaInput.Left)) == (GbaInput.Right | GbaInput.Left))
            inputs &= ~(GbaInput.Right | GbaInput.Left);
        if ((inputs & (GbaInput.Up | GbaInput.Down)) == (GbaInput.Up | GbaInput.Down))
            inputs &= ~(GbaInput.Up | GbaInput.Down);

        return inputs;
    }

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