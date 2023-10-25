using System;

namespace OnyxCs.Gba.Rayman3;

public class LevelInfo
{
    public LevelInfo(
        int levelMusic, 
        int finishLevelMusic, 
        int globalLumsIndex, 
        int globalCagesIndex, 
        int yellowLumsCount, 
        int cagesCount, 
        bool hasBlueLum, 
        Type frameType,
        MapId nextLevelId,
        MapId levelId)
    {
        LevelMusic = levelMusic;
        FinishLevelMusic = finishLevelMusic;
        GlobalLumsIndex = globalLumsIndex;
        GlobalCagesIndex = globalCagesIndex;
        YellowLumsCount = yellowLumsCount;
        CagesCount = cagesCount;
        HasBlueLum = hasBlueLum;
        FrameType = frameType;
        NextLevelId = nextLevelId;
        LevelId = levelId;
    }

    public int LevelMusic { get; }
    public int FinishLevelMusic { get; }
    public int GlobalLumsIndex { get; }
    public int GlobalCagesIndex { get; }
    public int YellowLumsCount { get; }
    public int CagesCount { get; }
    public bool HasBlueLum { get; }
    public Type FrameType { get; }
    public MapId NextLevelId { get; }
    public MapId LevelId { get; }
}