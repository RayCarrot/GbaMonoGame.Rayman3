using GbaMonoGame.AnimEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame.Rayman3
{
    /// <summary>
    /// A custom object base class for emulating GBA effects, such as fading and windows
    /// </summary>
    public abstract class EffectObject : AObject
    {
        static EffectObject()
        {
            Pixel = new Texture2D(Engine.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Pixel.SetData(new[] { Color.White });
        }

        protected static Texture2D Pixel { get; }
    }
}
