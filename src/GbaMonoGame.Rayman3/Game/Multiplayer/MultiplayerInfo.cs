﻿using System;

namespace GbaMonoGame.Rayman3;

public static class MultiplayerInfo
{
    public static MultiplayerGameType GameType { get; set; }
    public static int? MapId { get; set; }
    public static uint InitialGameTime { get; set; }
    public static TagInfo TagInfo { get; set; }

    public static void Reset()
    {
        GameType = default;
        MapId = null;
        InitialGameTime = 0;
        TagInfo = null;
    }

    public static void SetGameType(MultiplayerGameType gameType)
    {
        if (MapId == null)
            throw new Exception("Can't set game type before setting map");

        GameType = gameType;

        if (GameType is MultiplayerGameType.RayTag or MultiplayerGameType.CatAndMouse or MultiplayerGameType.CaptureTheFlag)
            TagInfo = new TagInfo(GameType, MapId.Value);
    }
}