using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public class GfxRenderer
{
    #region Constructor

    public GfxRenderer(SpriteBatch spriteBatch, GfxCamera camera)
    {
        SpriteBatch = spriteBatch;
        Camera = camera;

        Pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        Pixel.SetData(new[] { Color.White });
    }

    #endregion

    #region Private Properties

    private Texture2D Pixel { get; }
    private SpriteBatch SpriteBatch { get; }

    #endregion

    #region Public Properties

    public GfxCamera Camera { get; }

    #endregion

    #region Standard

    public void Begin()
    {
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.TransformMatrix);
    }

    public void End()
    {
        //DrawFilledRectangle(Camera.VisibleArea, new Color(Color.Red, 0.5f));
        SpriteBatch.End();
    }

    #endregion

    #region Alpha

    public void BeginAlpha()
    {
        SpriteBatch.End();
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.Additive, transformMatrix: Camera.TransformMatrix);
    }

    public void EndAlpha()
    {
        SpriteBatch.End();
        Begin();
    }

    #endregion

    #region Draw

    public void Draw(Texture2D texture, Vector2 position, Rectangle sourceRectangle, Color? color = null)
    {
        Rectangle destinationRectangle = new(position.ToPoint(), sourceRectangle.Size);

        if (!Camera.IsVisible(destinationRectangle))
            return;

        var visibleRect = Camera.GetVisibleRectangle(sourceRectangle, destinationRectangle);

        SpriteBatch.Draw(texture, visibleRect.Destination, visibleRect.Source, color ?? Color.White);
    }

    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, SpriteEffects effects, Color? color = null)
    {
        if (!Camera.IsVisible(destinationRectangle))
            return;

        sourceRectangle ??= texture.Bounds;
        var visibleRect = Camera.GetVisibleRectangle(sourceRectangle.Value, destinationRectangle);

        SpriteBatch.Draw(texture, visibleRect.Destination, visibleRect.Source, color ?? Color.White, 0, Vector2.Zero, effects, 0);
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, Color? color = null)
    {
        // TODO: Check camera for clipping

        SpriteBatch.Draw(texture, position, sourceRectangle, color ?? Color.White, rotation, origin, scale, effects, 0);
    }

    #endregion

    #region FillRectangle

    /// <summary>
    /// Draws a filled rectangle
    /// </summary>
    /// <param name="rect">The rectangle to draw</param>
    /// <param name="color">The color to draw the rectangle in</param>
    public void DrawFilledRectangle(Rectangle rect, Color color)
    {
        // Simply use the function already there
        SpriteBatch.Draw(Pixel, rect, color);
    }

    /// <summary>
    /// Draws a filled rectangle
    /// </summary>
    /// <param name="rect">The rectangle to draw</param>
    /// <param name="color">The color to draw the rectangle in</param>
    /// <param name="angle">The angle in radians to draw the rectangle at</param>
    public void DrawFilledRectangle(Rectangle rect, Color color, float angle)
    {
        SpriteBatch.Draw(Pixel, rect, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }

    /// <summary>
    /// Draws a filled rectangle
    /// </summary>
    /// <param name="location">Where to draw</param>
    /// <param name="size">The size of the rectangle</param>
    /// <param name="color">The color to draw the rectangle in</param>
    public void DrawFilledRectangle(Vector2 location, Vector2 size, Color color)
    {
        DrawFilledRectangle(location, size, color, 0.0f);
    }

    /// <summary>
    /// Draws a filled rectangle
    /// </summary>
    /// <param name="location">Where to draw</param>
    /// <param name="size">The size of the rectangle</param>
    /// <param name="angle">The angle in radians to draw the rectangle at</param>
    /// <param name="color">The color to draw the rectangle in</param>
    public void DrawFilledRectangle(Vector2 location, Vector2 size, Color color, float angle)
    {
        // stretch the pixel between the two vectors
        SpriteBatch.Draw(Pixel,
            location,
            null,
            color,
            angle,
            Vector2.Zero,
            size,
            SpriteEffects.None,
            0);
    }

    /// <summary>
    /// Draws a filled rectangle
    /// </summary>
    /// <param name="x">The X coordinate of the left side</param>
    /// <param name="y">The Y coordinate of the upper side</param>
    /// <param name="w">Width</param>
    /// <param name="h">Height</param>
    /// <param name="color">The color to draw the rectangle in</param>
    public void DrawFilledRectangle(float x, float y, float w, float h, Color color)
    {
        DrawFilledRectangle(new Vector2(x, y), new Vector2(w, h), color, 0.0f);
    }

    /// <summary>
    /// Draws a filled rectangle
    /// </summary>
    /// <param name="x">The X coordinate of the left side</param>
    /// <param name="y">The Y coordinate of the upper side</param>
    /// <param name="w">Width</param>
    /// <param name="h">Height</param>
    /// <param name="color">The color to draw the rectangle in</param>
    /// <param name="angle">The angle of the rectangle in radians</param>
    public void DrawFilledRectangle(float x, float y, float w, float h, Color color, float angle)
    {
        DrawFilledRectangle(new Vector2(x, y), new Vector2(w, h), color, angle);
    }

    #endregion

    #region DrawRectangle

    /// <summary>
    /// Draws a rectangle with the thickness provided
    /// </summary>
    /// <param name="rect">The rectangle to draw</param>
    /// <param name="color">The color to draw the rectangle in</param>
    public void DrawRectangle(Rectangle rect, Color color)
    {
        DrawRectangle(rect, color, 1.0f);
    }

    /// <summary>
    /// Draws a rectangle with the thickness provided
    /// </summary>
    /// <param name="rect">The rectangle to draw</param>
    /// <param name="color">The color to draw the rectangle in</param>
    /// <param name="thickness">The thickness of the lines</param>
    public void DrawRectangle(Rectangle rect, Color color, float thickness)
    {
        DrawLine(new Vector2(rect.X, rect.Y), new Vector2(rect.Right, rect.Y), color, thickness); // top
        DrawLine(new Vector2(rect.X, rect.Y), new Vector2(rect.X, rect.Bottom), color, thickness); // left
        DrawLine(new Vector2(rect.X, rect.Bottom), new Vector2(rect.Right, rect.Bottom), color, thickness); // bottom
        DrawLine(new Vector2(rect.Right, rect.Y), new Vector2(rect.Right, rect.Bottom), color, thickness); // right
    }

    /// <summary>
    /// Draws a rectangle with the thickness provided
    /// </summary>
    /// <param name="location">Where to draw</param>
    /// <param name="size">The size of the rectangle</param>
    /// <param name="color">The color to draw the rectangle in</param>
    public void DrawRectangle(Vector2 location, Vector2 size, Color color)
    {
        DrawRectangle(new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color, 1.0f);
    }

    /// <summary>
    /// Draws a rectangle with the thickness provided
    /// </summary>
    /// <param name="location">Where to draw</param>
    /// <param name="size">The size of the rectangle</param>
    /// <param name="color">The color to draw the rectangle in</param>
    /// <param name="thickness">The thickness of the line</param>
    public void DrawRectangle(Vector2 location, Vector2 size, Color color, float thickness)
    {
        DrawRectangle(new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color, thickness);
    }

    #endregion

    #region DrawLine

    /// <summary>
    /// Draws a line from point1 to point2 with an offset
    /// </summary>
    /// <param name="x1">The X coordinate of the first point</param>
    /// <param name="y1">The Y coordinate of the first point</param>
    /// <param name="x2">The X coordinate of the second point</param>
    /// <param name="y2">The Y coordinate of the second point</param>
    /// <param name="color">The color to use</param>
    public void DrawLine(float x1, float y1, float x2, float y2, Color color)
    {
        DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, 1.0f);
    }

    /// <summary>
    /// Draws a line from point1 to point2 with an offset
    /// </summary>
    /// <param name="x1">The X coordinate of the first point</param>
    /// <param name="y1">The Y coordinate of the first point</param>
    /// <param name="x2">The X coordinate of the second point</param>
    /// <param name="y2">The Y coordinate of the second point</param>
    /// <param name="color">The color to use</param>
    /// <param name="thickness">The thickness of the line</param>
    public void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness)
    {
        DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
    }

    /// <summary>
    /// Draws a line from point1 to point2 with an offset
    /// </summary>
    /// <param name="point1">The first point</param>
    /// <param name="point2">The second point</param>
    /// <param name="color">The color to use</param>
    public void DrawLine(Vector2 point1, Vector2 point2, Color color)
    {
        DrawLine(point1, point2, color, 1.0f);
    }

    /// <summary>
    /// Draws a line from point1 to point2 with an offset
    /// </summary>
    /// <param name="point1">The first point</param>
    /// <param name="point2">The second point</param>
    /// <param name="color">The color to use</param>
    /// <param name="thickness">The thickness of the line</param>
    public void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness)
    {
        // calculate the distance between the two vectors
        float distance = Vector2.Distance(point1, point2);

        // calculate the angle between the two vectors
        float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

        DrawLine(point1, distance, angle, color, thickness);
    }

    /// <summary>
    /// Draws a line from point1 to point2 with an offset
    /// </summary>
    /// <param name="point">The starting point</param>
    /// <param name="length">The length of the line</param>
    /// <param name="angle">The angle of this line from the starting point in radians</param>
    /// <param name="color">The color to use</param>
    public void DrawLine(Vector2 point, float length, float angle, Color color)
    {
        DrawLine(point, length, angle, color, 1.0f);
    }

    /// <summary>
    /// Draws a line from point1 to point2 with an offset
    /// </summary>
    /// <param name="point">The starting point</param>
    /// <param name="length">The length of the line</param>
    /// <param name="angle">The angle of this line from the starting point</param>
    /// <param name="color">The color to use</param>
    /// <param name="thickness">The thickness of the line</param>
    public void DrawLine(Vector2 point, float length, float angle, Color color, float thickness)
    {
        // stretch the pixel between the two vectors
        SpriteBatch.Draw(Pixel,
            point,
            null,
            color,
            angle,
            new Vector2(0, (float)Pixel.Height / 2),
            new Vector2(length, thickness),
            SpriteEffects.None,
            0);
    }

    #endregion

    #region DrawPixel

    public void DrawPixel(float x, float y, Color color)
    {
        DrawPixel(new Vector2(x, y), color);
    }

    public void DrawPixel(Vector2 position, Color color)
    {
        SpriteBatch.Draw(Pixel, position, color);
    }

    #endregion
}