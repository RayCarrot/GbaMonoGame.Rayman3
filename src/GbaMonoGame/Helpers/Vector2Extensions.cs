using System;
using Microsoft.Xna.Framework;

namespace GbaMonoGame;

public static class Vector2Extensions
{
    public static Point ToRoundedPoint(this Vector2 point) => new((int)Math.Round(point.X), (int)Math.Round(point.Y));
}