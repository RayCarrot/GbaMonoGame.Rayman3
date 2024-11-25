using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

public class CircleWindowWipeEffectObject : EffectObject
{
    public bool IsEnabled { get; set; }
    public int Value { get; set; } // 0-255

    // NOTE: This math will be a bit different from the original game due to the usage of floats
    public override void Execute(Action<short> soundEventCallback)
    {
        // TODO: Add option to use this on N-Gage
        if (Engine.Settings.Platform == Platform.NGage)
            return;

        float scalingFactor = MathHelpers.FromFixedPoint(0x39b07); // Around 3.6
        Vector2 res = Camera.Resolution;
        Vector2 halfRes = res / 2;
        const int quarterCircle = 64; // 256/4

        // Bottom-right
        if (Value < quarterCircle * 1)
        {
            if (Value != quarterCircle * 0)
            {
                float horizontalScale = scalingFactor * MathHelpers.Cos256(quarterCircle * 1 - Value);
                float verticalScale = scalingFactor * MathHelpers.Sin256(quarterCircle * 1 - Value);
                float triangleLength = horizontalScale / verticalScale;

                for (float y = halfRes.Y; y < res.Y; y++)
                {
                    float length = (y - halfRes.Y) * triangleLength;

                    if (length > halfRes.X)
                        length = halfRes.X;

                    DrawLine(new Vector2(halfRes.X, y), new Vector2(halfRes.X + length, y), Color.Black);
                }
            }
        }
        // Top-right
        else if (Value < quarterCircle * 2)
        {
            // Fill bottom-right
            DrawRectangle(halfRes, halfRes, Color.Black);

            if (Value != quarterCircle * 1)
            {
                float horizontalScale = scalingFactor * MathHelpers.Cos256(quarterCircle * 1 - Value);
                float verticalScale = scalingFactor * MathHelpers.Sin256(quarterCircle * 3 - Value);
                float triangleLength = horizontalScale / verticalScale;

                for (int y = 0; y < halfRes.Y; y++)
                {
                    float length = (halfRes.Y - y) * triangleLength;

                    if (length > halfRes.X)
                        length = halfRes.X;

                    DrawLine(new Vector2(halfRes.X + length, y), new Vector2(res.X, y), Color.Black);
                }
            }
        }
        // Top-left
        else if (Value < quarterCircle * 3)
        {
            // Fill right
            DrawRectangle(new Vector2(halfRes.X, 0), new Vector2(halfRes.X, res.Y), Color.Black);

            if (Value != quarterCircle * 2)
            {
                float horizontalScale = scalingFactor * MathHelpers.Cos256(quarterCircle * 3 - Value);
                float verticalScale = scalingFactor * MathHelpers.Sin256(quarterCircle * 3 - Value);
                float triangleLength = horizontalScale / verticalScale;

                for (int y = 0; y < halfRes.Y; y++)
                {
                    float length = (halfRes.Y - y) * triangleLength;

                    if (length > halfRes.X)
                        length = halfRes.X;

                    DrawLine(new Vector2(halfRes.X - length, y), new Vector2(halfRes.X, y), Color.Black);
                }
            }
        }
        // Bottom-left
        else
        {
            // Fill top
            DrawRectangle(Vector2.Zero, new Vector2(res.X, halfRes.Y), Color.Black);

            // Fill bottom-right
            DrawRectangle(halfRes, halfRes, Color.Black);

            if (Value != quarterCircle * 3)
            {
                float horizontalScale = scalingFactor * MathHelpers.Cos256(quarterCircle * 3 - Value);
                float verticalScale = scalingFactor * MathHelpers.Sin256(quarterCircle * 5 - Value);
                float triangleLength = horizontalScale / verticalScale;

                for (float y = halfRes.Y; y < res.Y; y++)
                {
                    float length = (y - halfRes.Y) * triangleLength;

                    if (length > halfRes.X)
                        length = halfRes.X;

                    DrawLine(new Vector2(0, y), new Vector2(halfRes.X - length, y), Color.Black);
                }
            }
        }
    }
}