namespace OnyxCs.Gba.Rayman3;

public static class GameInfo
{
    public static MapId NextMapId { get; set; }
    public static MapId MapId { get; set; }
    public static int World { get; set; }
    public static int LoadedYellowLums { get; set; }
    public static int GreenLums { get; set; }
    public static int LastGreenLumAlive { get; set; }

    public static LevelInfo Level => Levels[(int)MapId];

    public static void SetNextMapId(MapId mapId)
    {
        LastGreenLumAlive = 0;
        NextMapId = mapId;
        GreenLums = 0;
        // TODO: More setup...

    }

    public static LevelInfo[] Levels { get; } =
    {
        new LevelInfo(
            levelMusic: 0x98,
            finishLevelMusic: 0x99,
            globalLumsIndex: 0,
            globalCagesIndex: 0,
            yellowLumsCount: 25,
            cagesCount: 1,
            hasBlueLum: false,
            frameType: typeof(WoodLight),
            nextLevelId: MapId.WoodsOfLight_M1,
            levelId: MapId.WoodsOfLight_M1),
        new LevelInfo(
            levelMusic: 0x98,
            finishLevelMusic: 0x99,
            globalLumsIndex: 25,
            globalCagesIndex: 1,
            yellowLumsCount: 30,
            cagesCount: 2,
            hasBlueLum: false,
            frameType: typeof(WoodLight), // TODO: Different type
            nextLevelId: (MapId)54,
            levelId: MapId.WoodsOfLight_M2),
    };
}