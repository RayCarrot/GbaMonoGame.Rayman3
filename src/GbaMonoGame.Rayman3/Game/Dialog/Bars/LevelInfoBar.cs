﻿using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public class LevelInfoBar : Bar
{
    public LevelInfoBar(Scene2D scene) : base(scene)
    {
        LevelMaps = new[]
        {
            new[] { MapId.WoodLight_M1, MapId.WoodLight_M2 },
            new[] { MapId.FairyGlade_M1, MapId.FairyGlade_M2 },
            new[] { MapId.MarshAwakening1 },
            new[] { MapId.SanctuaryOfBigTree_M1, MapId.SanctuaryOfBigTree_M2 },
            new[] { MapId.BossMachine },
            new[] { MapId.Bonus1 },

            new[] { MapId.MissileRace1 },
            new[] { MapId.EchoingCaves_M1, MapId.EchoingCaves_M2 },
            new[] { MapId.CavesOfBadDreams_M1, MapId.CavesOfBadDreams_M2 },
            new[] { MapId.MenhirHills_M1, MapId.MenhirHills_M2 },
            new[] { MapId.MarshAwakening2 },
            new[] { MapId.BossBadDreams },
            new[] { MapId.Bonus2 },
            new[] { MapId.ChallengeLy1 },

            new[] { MapId.SanctuaryOfRockAndLava_M1, MapId.SanctuaryOfRockAndLava_M2, MapId.SanctuaryOfRockAndLava_M3 },
            new[] { MapId.BeneathTheSanctuary_M1, MapId.BeneathTheSanctuary_M2 },
            new[] { MapId.ThePrecipice_M1, MapId.ThePrecipice_M2 },
            new[] { MapId.TheCanopy_M1, MapId.TheCanopy_M2 },
            new[] { MapId.SanctuaryOfStoneAndFire_M1, MapId.SanctuaryOfStoneAndFire_M2, MapId.SanctuaryOfStoneAndFire_M3 },
            new[] { MapId.BossRockAndLava },
            new[] { MapId.Bonus3 },

            new[] { MapId.TombOfTheAncients_M1, MapId.TombOfTheAncients_M2 },
            new[] { MapId.IronMountains_M1, MapId.IronMountains_M2 },
            new[] { MapId.MissileRace2 },
            new[] { MapId.PirateShip_M1, MapId.PirateShip_M2 },
            new[] { MapId.BossScaleMan },
            new[] { MapId.BossFinal_M1 },
            new[] { MapId.Bonus4 },
            new[] { MapId.ChallengeLy2 },
            new[] { MapId._1000Lums },
            new[] { MapId.ChallengeLyGCN },
        };

        WaitTimer = 0;
        LevelInfoBarDrawStep = BarDrawStep.Hide;
        OffsetY = 40;
        LevelCurtainId = 0;
    }

    public MapId[][] LevelMaps { get; }

    public BarDrawStep LevelInfoBarDrawStep { get; set; }
    public int WaitTimer { get; set; }
    public int OffsetY { get; set; }
    public int LevelCurtainId { get; set; }

    public SpriteTextObject LevelName { get; set; }
    public AnimatedObject Canvas { get; set; }
    public AnimatedObject CollectedLumsDigit1 { get; set; }
    public AnimatedObject CollectedLumsDigit2 { get; set; }
    public AnimatedObject TotalLumsDigit1 { get; set; }
    public AnimatedObject TotalLumsDigit2 { get; set; }
    public AnimatedObject CollectedCagesDigit { get; set; }
    public AnimatedObject TotalCagesDigit { get; set; }

    private string GetLevelName()
    {
        int textId = GameInfo.Levels[(int)LevelMaps[LevelCurtainId][0]].NameTextId;
        return Localization.GetText(8, textId)[0];
    }

    public void SetLevel(int levelCurtainId)
    {
        LevelInfoBarDrawStep = BarDrawStep.MoveIn;
        WaitTimer = 0;
        LevelCurtainId = levelCurtainId;
        Set();
    }

    public override void Load()
    {
        // NOTE: Game has it set up so Load can be called multiple times. Dynamic objects don't get recreated after the first time, but instead
        //       reloaded into VRAM. We don't need to do that though due to how the graphics system works here, so just always create everything.

        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.LevelDashboardAnimations);

        Canvas = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 0,
            ObjPriority = 0,
            ScreenPos = new Vector2(85, 8),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            VerticalAnchor = VerticalAnchorMode.Bottom,
            CurrentAnimation = 10,
            Camera = Scene.HudCamera,
        };

        CollectedLumsDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 0,
            ObjPriority = 0,
            ScreenPos = new Vector2(Engine.Settings.Platform switch
            {
                Platform.GBA => 77,
                Platform.NGage => 44,
                _ => throw new UnsupportedPlatformException()
            }, -6),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            VerticalAnchor = VerticalAnchorMode.Bottom,
            CurrentAnimation = 0,
            Camera = Scene.HudCamera,
        };

        CollectedLumsDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 0,
            ObjPriority = 0,
            ScreenPos = new Vector2(Engine.Settings.Platform switch
            {
                Platform.GBA => 86,
                Platform.NGage => 53,
                _ => throw new UnsupportedPlatformException()
            }, -6),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            VerticalAnchor = VerticalAnchorMode.Bottom,
            CurrentAnimation = 0,
            Camera = Scene.HudCamera,
        };

        TotalLumsDigit1 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 0,
            ObjPriority = 0,
            ScreenPos = new Vector2(Engine.Settings.Platform switch
            {
                Platform.GBA => 101,
                Platform.NGage => 68,
                _ => throw new UnsupportedPlatformException()
            }, -6),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            VerticalAnchor = VerticalAnchorMode.Bottom,
            CurrentAnimation = 0,
            Camera = Scene.HudCamera,
        };

        TotalLumsDigit2 = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 0,
            ObjPriority = 0,
            ScreenPos = new Vector2(Engine.Settings.Platform switch
            {
                Platform.GBA => 110,
                Platform.NGage => 77,
                _ => throw new UnsupportedPlatformException()
            }, -6),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            VerticalAnchor = VerticalAnchorMode.Bottom,
            CurrentAnimation = 0,
            Camera = Scene.HudCamera,
        };

        CollectedCagesDigit = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 0,
            ObjPriority = 0,
            ScreenPos = new Vector2(Engine.Settings.Platform switch
            {
                Platform.GBA => 151,
                Platform.NGage => 112,
                _ => throw new UnsupportedPlatformException()
            }, -7),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            VerticalAnchor = VerticalAnchorMode.Bottom,
            CurrentAnimation = 0,
            Camera = Scene.HudCamera,
        };

        TotalCagesDigit = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 0,
            ObjPriority = 0,
            ScreenPos = new Vector2(Engine.Settings.Platform switch
            {
                Platform.GBA => 166,
                Platform.NGage => 127,
                _ => throw new UnsupportedPlatformException()
            }, -7),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            VerticalAnchor = VerticalAnchorMode.Bottom,
            CurrentAnimation = 0,
            Camera = Scene.HudCamera,
        };

        LevelName = new SpriteTextObject()
        {
            Color = TextColor.LevelName,
            FontSize = FontSize.Font16,
            ScreenPos = new Vector2(0, -32),
            HorizontalAnchor = HorizontalAnchorMode.Center,
            VerticalAnchor = VerticalAnchorMode.Bottom,
            Camera = Scene.HudCamera,
        };
    }

    public override void Set()
    {
        // Calculate collectibles
        int collectedLums = 0;
        int totalLums = 0;
        int collectedCages = 0;
        int totalCages = 0;

        foreach (MapId mapId in LevelMaps[LevelCurtainId])
        {
            collectedLums += GameInfo.GetCollectedYellowLumsInLevel(mapId);
            totalLums += GameInfo.Levels[(int)mapId].LumsCount;
            collectedCages += GameInfo.GetCollectedCagesInLevel(mapId);
            totalCages += GameInfo.Levels[(int)mapId].CagesCount;
        }

        // Set animations
        CollectedLumsDigit1.CurrentAnimation = collectedLums / 10;
        CollectedLumsDigit2.CurrentAnimation = collectedLums % 10;
        TotalLumsDigit1.CurrentAnimation = totalLums / 10;
        TotalLumsDigit2.CurrentAnimation = totalLums % 10;
        CollectedCagesDigit.CurrentAnimation = collectedCages;
        TotalCagesDigit.CurrentAnimation = totalCages;

        bool noCollectibles;
        bool complete;
        switch (LevelCurtainId)
        {
            case 4:
                complete = GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.BossMachine;
                noCollectibles = true;
                break;

            case 11:
                complete = GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.BossBadDreams;
                noCollectibles = true;
                break;

            case 13:
                complete = GameInfo.PersistentInfo.FinishedLyChallenge1;
                noCollectibles = true;
                break;

            case 19:
                complete = GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.BossRockAndLava;
                noCollectibles = true;
                break;

            case 25:
                complete = GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.BossScaleMan;
                noCollectibles = true;
                break;

            case 26:
                complete = GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.BossFinal_M2;
                noCollectibles = true;
                break;

            case 28:
                complete = GameInfo.PersistentInfo.FinishedLyChallenge2;
                noCollectibles = true;
                break;

            case 30:
                complete = GameInfo.PersistentInfo.FinishedLyChallengeGCN;
                noCollectibles = true;
                break;

            default:
                noCollectibles = false;
                complete = collectedLums == totalLums && collectedCages == totalCages;
                break;
        }

        LevelName.Color = complete ? TextColor.LevelNameComplete : TextColor.LevelName;
        Canvas.CurrentAnimation = noCollectibles ? 11 : 10;
        LevelName.Text = GetLevelName();
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        switch (LevelInfoBarDrawStep)
        {
            case BarDrawStep.Hide:
                OffsetY = 40;
                break;

            case BarDrawStep.MoveIn:
                if (OffsetY != 0)
                {
                    if (OffsetY == 36)
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PannelUp_Mix01);

                    OffsetY -= 2;
                }
                else
                {
                    LevelInfoBarDrawStep = BarDrawStep.Wait;
                }
                break;

            case BarDrawStep.MoveOut:
                if (OffsetY < 40)
                {
                    OffsetY += 2;
                }
                else
                {
                    OffsetY = 40;
                    LevelInfoBarDrawStep = BarDrawStep.Hide;
                }
                break;

            case BarDrawStep.Wait:
                if (WaitTimer >= 20)
                {
                    OffsetY = 0;
                    LevelInfoBarDrawStep = BarDrawStep.MoveOut;
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PannelDw_Mix01);
                }
                else
                {
                    WaitTimer++;
                }
                break;
        }

        if (LevelInfoBarDrawStep != BarDrawStep.Hide)
        {
            LevelName.ScreenPos = new Vector2(
                x: -LevelName.GetStringWidth() / 2f,
                y: -32 + OffsetY);

            Canvas.ScreenPos = Canvas.ScreenPos with { Y = 8 + OffsetY };

            CollectedLumsDigit1.ScreenPos = CollectedLumsDigit1.ScreenPos with { Y = -6 + OffsetY };
            CollectedLumsDigit2.ScreenPos = CollectedLumsDigit2.ScreenPos with { Y = -6 + OffsetY };

            TotalLumsDigit1.ScreenPos = TotalLumsDigit1.ScreenPos with { Y = -6 + OffsetY };
            TotalLumsDigit2.ScreenPos = TotalLumsDigit2.ScreenPos with { Y = -6 + OffsetY };

            CollectedCagesDigit.ScreenPos = CollectedCagesDigit.ScreenPos with { Y = -7 + OffsetY };

            TotalCagesDigit.ScreenPos = TotalCagesDigit.ScreenPos with { Y = -7 + OffsetY };

            animationPlayer.PlayFront(Canvas);

            if (Canvas.CurrentAnimation != 11)
            {
                animationPlayer.PlayFront(CollectedLumsDigit1);
                animationPlayer.PlayFront(CollectedLumsDigit2);
                animationPlayer.PlayFront(TotalLumsDigit1);
                animationPlayer.PlayFront(TotalLumsDigit2);
                animationPlayer.PlayFront(CollectedCagesDigit);
                animationPlayer.PlayFront(TotalCagesDigit);
            }

            animationPlayer.PlayFront(LevelName);
        }
    }
}