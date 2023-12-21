using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public class MenuRenderer
{
    #region Private Properties

    private List<Sprite> Sprites { get; } = new();
    private int Margin => 20;
    private int LineHeight => 20;
    private Color Color { get; } = Color.White;
    private Color HighlightColor { get; } = new(218, 168, 9);

    private Action<MenuRenderer> CurrentMenu { get; set; }
    private bool NewMenu { get; set; }

    private Box RenderBox { get; set; }
    private Vector2 Position { get; set; }
    private int VerticalButtonsCount { get; set; }
    private int CurrentVerticalIndex { get; set; }

    #endregion

    #region Private Methods

    private void DrawText(string text, ref Vector2 position, HorizontalAlignment horizontalAlignment, FontSize fontSize, Color color) =>
        DrawText(FontManager.GetTextBytes(text), ref position, horizontalAlignment, fontSize, color);

    private void DrawText(byte[] text, ref Vector2 position, HorizontalAlignment horizontalAlignment, FontSize fontSize, Color color)
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

        foreach (byte b in text)
        {
            Sprite sprite = FontManager.GetCharacterSprite(b, fontSize, ref position, 0, null, color);
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
            DrawText(lines[lineIndex], ref position, horizontalAlignment, fontSize, color);

            if (lineIndex < lines.Length - 1)
                position = new Vector2(posX, position.Y + FontManager.GetFontHeight(fontSize));
        }
    }

    #endregion

    #region Public Methods

    public void Update()
    {
        RenderBox = new Box(Margin, Margin, Engine.GameWindow.GameResolution.X - Margin, Engine.GameWindow.GameResolution.Y - Margin);
        Position = new Vector2(RenderBox.MinX, RenderBox.MinY);

        Sprites.Clear();
        VerticalButtonsCount = 0;

        if (NewMenu)
        {
            CurrentVerticalIndex = 0;
            NewMenu = false;
        }

        CurrentMenu(this);

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

        // Draw engine version in the corner
        Vector2 versionPos = new(Engine.GameWindow.GameResolution.X - 2, Engine.GameWindow.GameResolution.Y - 10);
        DrawText(
            text: Engine.Version.ToString(3),
            position: ref versionPos,
            horizontalAlignment: HorizontalAlignment.Right,
            fontSize: FontSize.Font8,
            color: Color);

    }

    public void ChangeMenu(Action<MenuRenderer> newMenu)
    {
        CurrentMenu = newMenu;
        NewMenu = true;
    }

    public void Text(string text)
    {
        Vector2 pos = new(Engine.GameWindow.GameResolution.X / 2f, Position.Y);

        DrawWrappedText(
            text: text,
            position: ref pos,
            horizontalAlignment: HorizontalAlignment.Center,
            fontSize: FontSize.Font16,
            color: Color);

        Position = new Vector2(RenderBox.MinX, pos.Y + LineHeight);
    }

    public bool Button(string text)
    {
        Vector2 pos = new(Engine.GameWindow.GameResolution.X / 2f, Position.Y);
        int index = VerticalButtonsCount;

        DrawWrappedText(
            text: text,
            position: ref pos,
            horizontalAlignment: HorizontalAlignment.Center,
            fontSize: FontSize.Font16,
            color: CurrentVerticalIndex == index ? HighlightColor : Color);

        VerticalButtonsCount++;
        Position = new Vector2(RenderBox.MinX, pos.Y + LineHeight);

        return CurrentVerticalIndex == index && JoyPad.CheckSingle(GbaInput.A);
    }

    public void Draw(GfxRenderer renderer)
    {
        foreach (Sprite sprite in Sprites)
            sprite.Draw(renderer);
    }

    #endregion

    #region Data Types

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right,
    }

    #endregion
}