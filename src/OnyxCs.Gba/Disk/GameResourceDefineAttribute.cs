using System;
using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class GameResourceDefineAttribute : Attribute
{
    public GameResourceDefineAttribute(Game game, Platform platform, int resourceId)
    {
        Game = game;
        Platform = platform;
        ResourceId = resourceId;
    }

    public Game Game { get; }
    public Platform Platform { get; }
    public int ResourceId { get; }
}