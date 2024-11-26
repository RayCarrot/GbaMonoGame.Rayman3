﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public class Sprite
{
    public Texture2D Texture { get; set; }
    public Rectangle TextureRectangle { get; set; }
    public Vector2 Position { get; set; }
    public bool FlipX { get; set; }
    public bool FlipY { get; set; }
    public int Priority { get; set; }

    public bool Center { get; set; }
    public AffineMatrix? AffineMatrix { get; set; }
    public Color Color { get; set; } = Color.White;

    // TODO: There are multiple issues with how alpha is implemented here compared to on GBA. Most noticeably sprites should not effect each other. Very noticeable when setting it on Rayman and looking up - Rayman gets 4 eyes then! Although this might be how it is on N-Gage?
    public float? Alpha { get; set; }

    public Effect Shader { get; set; }

    public GfxCamera Camera { get; set; } = Engine.ScreenCamera;

    public void Draw(GfxRenderer renderer)
    {
        renderer.BeginRender(new RenderOptions(Alpha != null, Shader, Camera));

        Color color = Color;
        if (Alpha != null)
            color = new Color(color, Alpha.Value);

        Rectangle textureRectangle = TextureRectangle;
        if (textureRectangle == Rectangle.Empty)
            textureRectangle = Texture.Bounds;

        Vector2 origin = new(textureRectangle.Width / 2f, textureRectangle.Height / 2f);

        SpriteEffects effects = SpriteEffects.None;
        if ((AffineMatrix?.FlipX ?? false) ^ FlipX)
            effects |= SpriteEffects.FlipHorizontally;
        if ((AffineMatrix?.FlipY ?? false) ^ FlipY)
            effects |= SpriteEffects.FlipVertically;

        float rotation;
        if (AffineMatrix != null)
        {
            rotation = AffineMatrix.Value.Rotation;
            if (FlipX ^ FlipY)
                rotation = -rotation;
        }
        else
        {
            rotation = 0;
        }

        Vector2 scale = AffineMatrix?.Scale ?? Vector2.One;

        renderer.Draw(
            texture: Texture, 
            position: Center ? Position + origin : Position, 
            sourceRectangle: textureRectangle, 
            rotation: rotation, 
            origin: Center ? origin : Vector2.Zero, 
            scale: scale, 
            effects: effects, 
            color: color);
    }
}