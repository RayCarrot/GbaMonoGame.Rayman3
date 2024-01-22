using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

public class MenuRenderer
{
    #region Private Properties

    private MenuCamera Camera { get; } = new(Engine.GameWindow);
    private List<Sprite> Sprites { get; } = new();
    private int Margin => 80;
    private int LineHeight => 40;
    private Color Color { get; } = Color.White;
    private Color HighlightColor { get; } = new(218, 168, 9);

    private MenuFunction CurrentMenu { get; set; }
    private MenuState NextMenuState { get; set; }
    private bool NewMenu { get; set; }

    private Box RenderBox { get; set; }
    private Vector2 Position { get; set; }
    private int VerticalButtonsCount { get; set; }
    private int CurrentVerticalIndex { get; set; }

    private byte TransitionTextOutDelay { get; set; }
    private float TextTransitionValue { get; set; } = 1;

    private Stack<MenuState> MenuStack { get; } = new();

    #endregion

    #region Public Properties

    public bool IsTransitioningTextOut { get; private set; }
    public bool IsTransitioningTextIn { get; private set; }
    public bool IsTransitioning => IsTransitioningTextOut || IsTransitioningTextIn;

    #endregion

    #region Private Methods

    private void DrawText(string text, ref Vector2 position, HorizontalAlignment horizontalAlignment, FontSize fontSize, Color color, bool animate) =>
        DrawText(FontManager.GetTextBytes(text), ref position, horizontalAlignment, fontSize, color, animate);

    private void DrawText(byte[] text, ref Vector2 position, HorizontalAlignment horizontalAlignment, FontSize fontSize, Color color, bool animate)
    {
        if (horizontalAlignment == HorizontalAlignment.Center)
        {
            int width = FontManager.GetStringWidth(fontSize, text);
            position -= new Vector2(width / 2f, 0);
        }
        else if (horizontalAlignment == HorizontalAlignment.Right)
        {
            int width = FontManager.GetStringWidth(fontSize, text);
            position -= new Vector2(width, 0);
        }

        AffineMatrix? matrix = animate ? new AffineMatrix(1, 0, 0, TextTransitionValue) : null;
        foreach (byte b in text)
        {
            Sprite sprite = FontManager.GetCharacterSprite(b, fontSize, ref position, 0, matrix, color, Camera);
            Sprites.Add(sprite);
        }
    }

    private void DrawWrappedText(string text, ref Vector2 position, HorizontalAlignment horizontalAlignment, FontSize fontSize, Color color)
    {
        float posX = position.X;

        float availableWidth = horizontalAlignment switch
        {
            HorizontalAlignment.Left => RenderBox.MaxX - position.X,
            HorizontalAlignment.Right => position.X - RenderBox.MinX,
            HorizontalAlignment.Center => (RenderBox.MaxX - position.X < position.X - RenderBox.MinX ? RenderBox.MaxX - position.X : position.X - RenderBox.MinX) * 2,
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null)
        };

        byte[][] lines = FontManager.GetWrappedStringLines(fontSize, text, availableWidth);
        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            DrawText(lines[lineIndex], ref position, horizontalAlignment, fontSize, color, true);

            if (lineIndex < lines.Length - 1)
                position = new Vector2(posX, position.Y + FontManager.GetFontHeight(fontSize));
        }
    }

    #endregion

    #region Public Methods

    public void Update()
    {
        RenderBox = new Box(Margin, Margin, Camera.Resolution.X - Margin, Camera.Resolution.Y - Margin);
        Position = new Vector2(RenderBox.MinX, RenderBox.MinY);

        Sprites.Clear();
        VerticalButtonsCount = 0;

        if (IsTransitioningTextOut)
        {
            TextTransitionValue++;

            if (TextTransitionValue > 8)
            {
                IsTransitioningTextOut = false;
                CurrentMenu = NextMenuState?.Menu;

                if (CurrentMenu != null)
                {
                    IsTransitioningTextIn = NextMenuState != null;
                    TransitionTextOutDelay = 2;
                    NewMenu = true;
                }
            }
        }
        else if (IsTransitioningTextIn)
        {
            if (TransitionTextOutDelay == 0)
            {
                TextTransitionValue--;

                if (TextTransitionValue < 1)
                {
                    TextTransitionValue = 1;
                    IsTransitioningTextOut = false;
                    IsTransitioningTextIn = false;
                }
            }
            else
            {
                TransitionTextOutDelay--;
            }
        }

        if (NewMenu)
        {
            CurrentVerticalIndex = NextMenuState?.CurrentVerticalIndex ?? 0;
            NewMenu = false;
        }

        CurrentMenu?.Invoke(this);

        if (!IsTransitioning)
        {
            // Run standard controls
            if (JoyPad.CheckSingle(GbaInput.Down))
            {
                if (CurrentVerticalIndex >= VerticalButtonsCount - 1)
                    CurrentVerticalIndex = 0;
                else
                    CurrentVerticalIndex++;
            }
            else if (JoyPad.CheckSingle(GbaInput.Up))
            {
                if (CurrentVerticalIndex <= 0)
                    CurrentVerticalIndex = VerticalButtonsCount - 1;
                else
                    CurrentVerticalIndex--;
            }
        }

        // Draw engine version in the corner
        Vector2 versionPos = new(Camera.Resolution.X - 12, Camera.Resolution.Y - 25);
        DrawText(
            text: $"Version {Engine.Version.ToString(3)}",
            position: ref versionPos,
            horizontalAlignment: HorizontalAlignment.Right,
            fontSize: FontSize.Font16,
            color: Color,
            animate: false);
    }

    public void Init(MenuFunction mainMenu)
    {
        if (IsTransitioning)
            return;

        CurrentMenu = mainMenu;
        NewMenu = true;

        IsTransitioningTextOut = false;
        IsTransitioningTextIn = true;
        TransitionTextOutDelay = 2;
        TextTransitionValue = 8;
    }

    public void UnInit()
    {
        if (IsTransitioning)
            return;

        NextMenuState = null;
        IsTransitioningTextOut = true;
        IsTransitioningTextIn = false;
        TextTransitionValue = 1;
    }

    public void GoBackFromMenu()
    {
        if (IsTransitioning)
            return;

        NextMenuState = MenuStack.Pop();
        IsTransitioningTextOut = true;
        IsTransitioningTextIn = true;
        TextTransitionValue = 1;
    }

    public void ChangeMenu(MenuFunction newMenu)
    {
        if (IsTransitioning)
            return;

        NextMenuState = new MenuState(newMenu, 0);
        IsTransitioningTextOut = true;
        IsTransitioningTextIn = true;
        TextTransitionValue = 1;
        MenuStack.Push(new MenuState(CurrentMenu, CurrentVerticalIndex));
    }

    public void Text(string text)
    {
        Vector2 pos = new(Camera.Resolution.X / 2f, Position.Y);

        DrawWrappedText(
            text: text,
            position: ref pos,
            horizontalAlignment: HorizontalAlignment.Center,
            fontSize: FontSize.Font32,
            color: Color);

        Position = new Vector2(RenderBox.MinX, pos.Y + LineHeight);
    }

    public bool Button(string text)
    {
        Vector2 pos = new(Camera.Resolution.X / 2f, Position.Y);
        int index = VerticalButtonsCount;

        DrawWrappedText(
            text: text,
            position: ref pos,
            horizontalAlignment: HorizontalAlignment.Center,
            fontSize: FontSize.Font32,
            color: CurrentVerticalIndex == index ? HighlightColor : Color);

        VerticalButtonsCount++;
        Position = new Vector2(RenderBox.MinX, pos.Y + LineHeight);

        return CurrentVerticalIndex == index && !IsTransitioning && JoyPad.CheckSingle(GbaInput.A);
    }

    public void Draw(GfxRenderer renderer)
    {
        foreach (Sprite sprite in Sprites)
            sprite.Draw(renderer);
    }

    #endregion

    #region Data Types

    public delegate void MenuFunction(MenuRenderer renderer);

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right,
    }

    private class MenuCamera : GfxCamera
    {
        public MenuCamera(GameWindow gameWindow) : base(gameWindow) { }

        protected override Vector2 GetResolution(GameWindow gameWindow)
        {
            // Scale by 3 to fit more text on screen
            return gameWindow.GameResolution * 3;
        }
    }

    private class MenuState
    {
        public MenuState(MenuFunction menu, int currentVerticalIndex)
        {
            Menu = menu;
            CurrentVerticalIndex = currentVerticalIndex;
        }

        public MenuFunction Menu { get; }
        public int CurrentVerticalIndex { get; }
    }

    #endregion
}