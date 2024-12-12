using System;
using System.Linq;
using GbaMonoGame.Editor;
using ImGuiNET;

namespace GbaMonoGame.Rayman3;

public class FramesDebugMenu : DebugMenu
{
    private FrameMenuItem[] Menu { get; } =
    {
        new("Intro", () => new Intro()),
        new("GameCubeMenu", () => new GameCubeMenu()),
        new("Game Over", () => new GameOver()),
        new("Level Select", () => new LevelSelect()),
        new("Menu", null, new FrameMenuItem[]
        {
            new("Language", () => new MenuAll(MenuAll.Page.SelectLanguage)),
            new("Game Mode", () => new MenuAll(MenuAll.Page.SelectGameMode)),
            new("Options", () => new MenuAll(MenuAll.Page.Options)),
            new("Multiplayer", () => new MenuAll(MenuAll.Page.Multiplayer)),
            new("Multiplayer Lost Connection", () => new MenuAll(MenuAll.Page.MultiplayerLostConnection)),
        }),
        new("Story", null, new FrameMenuItem[]
        {
            new("NGage Splash Screens", () => new NGageSplashScreensAct()),
            new("Act #1", () => new Act1()),
            new("Act #2", () => new Act2()),
            new("Act #3", () => new Act3()),
            new("Act #4", () => new Act4()),
            new("Act #5", () => new Act5()),
            new("Act #6", () => new Act6()),
        }),
        new("Levels", null, 
            GameInfo.Levels.
            Select((_, i) => new FrameMenuItem(((MapId)i).ToString(), () =>
            {
                // New power levels have to have the previous map id set before loading
                GameInfo.MapId = (MapId)i switch
                {
                    MapId.Power1 => MapId.WoodLight_M2,
                    MapId.Power2 => MapId.BossMachine,
                    MapId.Power3 => MapId.EchoingCaves_M2,
                    MapId.Power4 => MapId.BossRockAndLava,
                    MapId.Power5 => MapId.SanctuaryOfStoneAndFire_M3,
                    MapId.Power6 => MapId.BossScaleMan,
                    _ => GameInfo.MapId
                };

                // Create the level frame
                Frame frame = LevelFactory.Create((MapId)i);

                // Set the powers
                GameInfo.SetPowerBasedOnMap((MapId)i);

                return frame;
            }, EndWithSeparator: (MapId)i switch
            {
                MapId.SanctuaryOfBigTree_M2 => true,
                MapId.MarshAwakening2 => true,
                MapId.SanctuaryOfRockAndLava_M3 => true,
                MapId.BossFinal_M2 => true,
                MapId._1000Lums => true,
                MapId.ChallengeLyGCN => true,
                MapId.Power6 => true,
                MapId.WorldMap => true,
                _ => false
            })).
            ToArray()),
        new("Level Editor", null, 
            GameInfo.Levels.
            Select((_, i) => new FrameMenuItem(((MapId)i).ToString(), () => new LevelEditor(i), EndWithSeparator: (MapId)i switch
            {
                MapId.SanctuaryOfBigTree_M2 => true,
                MapId.MarshAwakening2 => true,
                MapId.SanctuaryOfRockAndLava_M3 => true,
                MapId.BossFinal_M2 => true,
                MapId._1000Lums => true,
                MapId.ChallengeLyGCN => true,
                MapId.Power6 => true,
                MapId.WorldMap => true,
                _ => false
            })).
            ToArray()),
    };

    public override string Name => "Frames";

    private void DrawMenu(FrameMenuItem[] items)
    {
        foreach (FrameMenuItem menuItem in items)
        {
            if (menuItem.SubMenu != null)
            {
                if (ImGui.BeginMenu(menuItem.Name))
                {
                    DrawMenu(menuItem.SubMenu);
                    ImGui.EndMenu();
                }
            }
            else if (ImGui.MenuItem(menuItem.Name))
            {
                FrameManager.SetNextFrame(menuItem.CreateFrame());
            }

            if (menuItem.EndWithSeparator)
                ImGui.Separator();
        }
    }

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        DrawMenu(Menu);
    }

    private record FrameMenuItem(string Name, Func<Frame> CreateFrame, FrameMenuItem[] SubMenu = null, bool EndWithSeparator = false);
}