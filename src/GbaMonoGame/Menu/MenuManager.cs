using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GbaMonoGame;

public class MenuManager
{
    #region Events

    public event EventHandler Closed;

    #endregion

    #region Private Properties

    private MenuCamera Camera { get; } = new(Engine.GameWindow);
    private List<Sprite> Sprites { get; } = new();
    private int Margin => 70;
    private int LineHeight => 40;
    private Color DisabledColor { get; } = new(0.4f, 0.4f, 0.4f);
    private Color Color { get; } = Color.White;
    private Color HighlightColor { get; } = new(218, 168, 9);

    private Menu CurrentMenu { get; set; }
    private MenuState NextMenuState { get; set; }
    private bool NewMenu { get; set; }

    private Box FullRenderBox { get; set; }
    private Box[] ColumnRenderBoxes { get; set; }
    private Vector2 Position { get; set; }
    private int ColumnsCount => ColumnRenderBoxes.Length;
    private int CurrentColumnIndex { get; set; }
    private int SelectableElementsCount { get; set; }
    private int CurrentSelectionIndex { get; set; }
    private HorizontalAlignment DefaultHorizontalAlignment { get; set; }

    private float TransitionValue { get; set; }

    private byte TransitionTextOutDelay { get; set; }
    private float TransitionTextValue { get; set; } = 1;

    private Stack<MenuState> MenuStack { get; } = new();

    #endregion

    #region Public Properties

    public bool IsTransitioningIn { get; private set; }
    public bool IsTransitioningOut { get; private set; }

    public bool IsTransitioningTextOut { get; private set; }
    public bool IsTransitioningTextIn { get; private set; }
    public bool IsTransitioningText => IsTransitioningTextOut || IsTransitioningTextIn;

    #endregion

    #region Private Methods

    private void OnClosed()
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

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

        AffineMatrix? matrix = animate ? new AffineMatrix(1, 0, 0, TransitionTextValue) : null;
        foreach (byte b in text)
        {
            Sprite sprite = FontManager.GetCharacterSprite(b, fontSize, ref position, 0, matrix, color, Camera);
            Sprites.Add(sprite);
        }
    }

    private void DrawWrappedText(string text, ref Vector2 position, HorizontalAlignment horizontalAlignment, FontSize fontSize, Color color)
    {
        Box renderBox = ColumnRenderBoxes[CurrentColumnIndex];

        float posX = position.X;

        float availableWidth = horizontalAlignment switch
        {
            HorizontalAlignment.Left => renderBox.MaxX - position.X,
            HorizontalAlignment.Right => position.X - renderBox.MinX,
            HorizontalAlignment.Center => (renderBox.MaxX - position.X < position.X - renderBox.MinX ? renderBox.MaxX - position.X : position.X - renderBox.MinX) * 2,
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

    private Vector2 GetPosition()
    {
        return DefaultHorizontalAlignment switch
        {
            HorizontalAlignment.Left => new Vector2(ColumnRenderBoxes[CurrentColumnIndex].MinX, Position.Y),
            HorizontalAlignment.Center => new Vector2(ColumnRenderBoxes[CurrentColumnIndex].Center.X, Position.Y),
            HorizontalAlignment.Right => new Vector2(ColumnRenderBoxes[CurrentColumnIndex].MaxX, Position.Y),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void NextLine(Vector2 pos)
    {
        CurrentColumnIndex++;

        if (CurrentColumnIndex >= ColumnsCount)
        {
            CurrentColumnIndex = 0;
            Position = new Vector2(ColumnRenderBoxes[CurrentColumnIndex].MinX, pos.Y + LineHeight);
        }
        else
        {
            Position = new Vector2(ColumnRenderBoxes[CurrentColumnIndex].MinX, Position.Y);
        }
    }

    #endregion

    #region Public Methods

    public void Update()
    {
        FullRenderBox = new Box(Margin, Margin, Camera.Resolution.X - Margin, Camera.Resolution.Y - Margin);
        Position = new Vector2(FullRenderBox.MinX, FullRenderBox.MinY);
        CurrentColumnIndex = 0;

        Sprites.Clear();
        SelectableElementsCount = 0;

        if (IsTransitioningTextOut)
        {
            TransitionTextValue++;

            if (TransitionTextValue > 8)
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
                TransitionTextValue--;

                if (TransitionTextValue < 1)
                {
                    TransitionTextValue = 1;
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
            CurrentSelectionIndex = NextMenuState?.SelectedIndex ?? 0;
            NewMenu = false;
        }

        CurrentMenu?.Update(this);

        if (!IsTransitioningText)
        {
            // Run standard controls
            if (JoyPad.CheckSingle(GbaInput.Down))
            {
                if (CurrentSelectionIndex >= SelectableElementsCount - 1)
                    CurrentSelectionIndex = 0;
                else
                    CurrentSelectionIndex++;
            }
            else if (JoyPad.CheckSingle(GbaInput.Up))
            {
                if (CurrentSelectionIndex <= 0)
                    CurrentSelectionIndex = SelectableElementsCount - 1;
                else
                    CurrentSelectionIndex--;
            }
            else if (JoyPad.CheckSingle(GbaInput.B) || JoyPad.CheckSingle(Keys.Back))
            {
                GoBack();
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

    public void GoBack()
    {
        if (IsTransitioningIn || IsTransitioningOut || IsTransitioningText)
            return;

        if (MenuStack.Count == 0)
        {
            Close();
        }
        else
        {
            NextMenuState = MenuStack.Pop();
            IsTransitioningTextOut = true;
            IsTransitioningTextIn = true;
            TransitionTextValue = 1;
        }
    }

    public void ChangeMenu(Menu newMenu)
    {
        if (IsTransitioningIn || IsTransitioningOut || IsTransitioningText)
            return;

        NextMenuState = new MenuState(newMenu, 0);
        IsTransitioningTextOut = true;
        IsTransitioningTextIn = true;
        TransitionTextValue = 1;
        MenuStack.Push(new MenuState(CurrentMenu, CurrentSelectionIndex));
    }

    public void SetColumns(params float[] columns)
    {
        float total = columns.Sum();

        // Replace with widths
        for (int i = 0; i < columns.Length; i++)
        {
            columns[i] = FullRenderBox.Width * (columns[i] / total);
        }

        ColumnRenderBoxes = new Box[columns.Length];
        float xPos = FullRenderBox.MinX;
        for (int i = 0; i < columns.Length; i++)
        {
            float width = columns[i];
            ColumnRenderBoxes[i] = new Box(xPos, FullRenderBox.MinY, xPos + width, FullRenderBox.MaxY);
            xPos += width;
        }
        CurrentColumnIndex = 0;
    }

    public void SetHorizontalAlignment(HorizontalAlignment horizontalAlignment)
    {
        DefaultHorizontalAlignment = horizontalAlignment;
    }

    public void Text(string text)
    {
        Vector2 pos = GetPosition();

        DrawWrappedText(
            text: text,
            position: ref pos,
            horizontalAlignment: DefaultHorizontalAlignment,
            fontSize: FontSize.Font32,
            color: Color);

        NextLine(pos);
    }

    public void Empty()
    {
        NextLine(GetPosition());
    }

    public bool Button(string text, bool isEnabled = true)
    {
        Vector2 pos = GetPosition();
        int index = isEnabled ? SelectableElementsCount : -1;
        Color color = Color;

        if (!isEnabled)
            color = DisabledColor;
        else if (CurrentSelectionIndex == index)
            color = HighlightColor;

        DrawWrappedText(
            text: text,
            position: ref pos,
            horizontalAlignment: DefaultHorizontalAlignment,
            fontSize: FontSize.Font32,
            color: color);

        if (isEnabled)
            SelectableElementsCount++;
        NextLine(pos);

        return CurrentSelectionIndex == index && !IsTransitioningText && JoyPad.CheckSingle(GbaInput.A);
    }

    public int Selection(string[] options, int selectedOption)
    {
        Vector2 pos = GetPosition();
        int index = SelectableElementsCount;
        string text = options[selectedOption];

        if (CurrentSelectionIndex == index)
        {
            text = $"< {text} >";

            if (JoyPad.CheckSingle(Keys.Left))
            {
                selectedOption--;
                if (selectedOption < 0)
                    selectedOption = options.Length - 1;
            }
            else if (JoyPad.CheckSingle(Keys.Right))
            {
                selectedOption++;
                if (selectedOption >= options.Length)
                    selectedOption = 0;
            }
        }

        DrawWrappedText(
            text: text,
            position: ref pos,
            horizontalAlignment: DefaultHorizontalAlignment,
            fontSize: FontSize.Font32,
            color: CurrentSelectionIndex == index ? HighlightColor : Color);

        SelectableElementsCount++;
        NextLine(pos);

        return selectedOption;
    }

    public void Open(Menu menu)
    {
        if (IsTransitioningIn || IsTransitioningOut || IsTransitioningText)
            return;

        CurrentMenu = menu;
        NewMenu = true;

        IsTransitioningTextOut = false;
        IsTransitioningTextIn = true;
        TransitionTextOutDelay = 2;
        TransitionTextValue = 8;
        IsTransitioningOut = false;
        IsTransitioningIn = true;
        TransitionValue = 0;
    }

    public void Close()
    {
        if (IsTransitioningIn || IsTransitioningOut || IsTransitioningText)
            return;

        NextMenuState = null;
        IsTransitioningTextOut = true;
        IsTransitioningTextIn = false;
        TransitionTextValue = 1;
        IsTransitioningOut = true;
        IsTransitioningIn = false;
        TransitionValue = 1;
    }

    public void Draw(GfxRenderer renderer)
    {
        if (IsTransitioningIn)
        {
            TransitionValue += 1 / 10f;

            if (TransitionValue >= 1)
            {
                TransitionValue = 1;
                IsTransitioningIn = false;
            }
        }
        else if (IsTransitioningOut)
        {
            TransitionValue -= 1 / 10f;

            if (TransitionValue <= 0)
            {
                TransitionValue = 0;
                IsTransitioningOut = false;
                OnClosed();
                return;
            }
        }

        renderer.BeginRender(new RenderOptions(false, Engine.ScreenCamera));

        // Fade out the game
        renderer.DrawFilledRectangle(Vector2.Zero, Engine.GameWindow.GameResolution, Color.Black * MathHelper.Lerp(0.0f, 0.7f, TransitionValue));

        // Draw the sprites
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
        public MenuState(Menu menu, int selectedIndex)
        {
            Menu = menu;
            SelectedIndex = selectedIndex;
        }

        public Menu Menu { get; }
        public int SelectedIndex { get; }
    }

    #endregion
}