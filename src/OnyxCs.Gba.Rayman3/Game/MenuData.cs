using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Rayman3;

// TODO: Add N-Gage support
public class MenuData
{
    public MenuData()
    {
        AnimatedObjectResource propsAnimations = Storage.LoadResource<AnimatedObjectResource>(GameResource.MenuPropAnimations);
        AnimatedObjectResource startEraseAnimations = Storage.LoadResource<AnimatedObjectResource>(GameResource.MenuStartEraseAnimations);
        AnimatedObjectResource gameLogoAnimations = Storage.LoadResource<AnimatedObjectResource>(GameResource.MenuGameLogoAnimations);
        AnimatedObjectResource gameModeAnimations = Storage.LoadResource<AnimatedObjectResource>(GameResource.MenuGameModeAnimations);
        AnimatedObjectResource languageListAnimations = Storage.LoadResource<AnimatedObjectResource>(GameResource.MenuLanguageListAnimations);
        AnimatedObjectResource slotEmptyAnimations = Storage.LoadResource<AnimatedObjectResource>(GameResource.SlotEmptyAnimations);
        AnimatedObjectResource steamAnimations = Storage.LoadResource<AnimatedObjectResource>(GameResource.MenuSteamAnimations);

        Wheel1 = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(7, 110),
            CurrentAnimation = 2,
            AffineMatrix = AffineMatrix.Identity
        };

        Wheel2 = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(136, 110),
            CurrentAnimation = 3,
            AffineMatrix = AffineMatrix.Identity
        };

        Wheel3 = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(172, 110),
            CurrentAnimation = 4,
            AffineMatrix = AffineMatrix.Identity
        };

        Wheel4 = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(66, 144),
            CurrentAnimation = 3,
            AffineMatrix = AffineMatrix.Identity
        };

        Cursor = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(33, 67),
            CurrentAnimation = 0,
        };

        Stem = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(47, 160),
            CurrentAnimation = 14,
        };

        Steam = new AnimatedObject(steamAnimations, steamAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(27, 20),
            CurrentAnimation = 0
        };

        LanguageList = new AnimatedObject(languageListAnimations, languageListAnimations.IsDynamic)
        {
            Priority = 0,
            ScreenPos = new Vector2(120, 28),
            CurrentAnimation = 0,
        };

        GameModeList = new AnimatedObject(gameModeAnimations, gameModeAnimations.IsDynamic)
        {
            Priority = 3,
            ScreenPos = new Vector2(73, 52),
            CurrentAnimation = 0,
        };

        GameLogo = new AnimatedObject(gameLogoAnimations, gameLogoAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(174, 16),
            CurrentAnimation = 0,
        };

        // field8_0x148
        // field17_0x170
        // field18_0x190

        SlotLumIcons = new AnimatedObject[3];
        SlotCageIcons = new AnimatedObject[3];
        SlotIcons = new AnimatedObject[3];
        SlotLumTexts = new SpriteTextObject[3];
        SlotCageTexts = new SpriteTextObject[3];
        SlotEmptyTexts = new AnimatedObject[3];
        for (int i = 0; i < 3; i++)
        {
            SlotLumIcons[i] = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
            {
                Priority = 3,
                ScreenPos = new Vector2(107, 55 + i * 18),
                CurrentAnimation = 13,
            };
            SlotCageIcons[i] = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
            {
                Priority = 3,
                ScreenPos = new Vector2(165, 51 + i * 18),
                CurrentAnimation = 11,
            };
            SlotIcons[i] = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
            {
                Priority = 3,
                ScreenPos = new Vector2(84, 54 + i * 18),
                CurrentAnimation = 8 + i,
            };
            SlotLumTexts[i] = new SpriteTextObject()
            {
                Priority = 3,
                ScreenPos = new Vector2(129, 55 + i * 18),
                Text = "1000",
                FontSize = FontSize.Font16,
            };
            SlotCageTexts[i] = new SpriteTextObject()
            {
                Priority = 3,
                ScreenPos = new Vector2(190, 55 + i * 18),
                Text = "50",
                FontSize = FontSize.Font16,
            };
            SlotEmptyTexts[i] = new AnimatedObject(slotEmptyAnimations, slotEmptyAnimations.IsDynamic)
            {
                Priority = 3,
                ScreenPos = new Vector2(126, 54 + i * 18),
                CurrentAnimation = 0,
            };
        }

        // TODO: Load data
        // field44_0x6e0
        // field29_0x448

        StartEraseSelection = new AnimatedObject(startEraseAnimations, startEraseAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(80, 30),
            CurrentAnimation = 1,
        };

        StartEraseCursor = new AnimatedObject(startEraseAnimations, startEraseAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(106, 12),
            CurrentAnimation = 40
        };

        // TODO: Load data
        // field34_0x548
        // field30_0x468
        // field31_0x488
        // field33_0x4c8
        // field32_0x4a8
        // field35_0x5c0
        // field36_0x5e0
        // field37_0x608
        // field38_0x628
        // field39_0x648
        // field40_0x668
        // field41_0x690
        // field42_0x6a8
        // field43_0x6c0
    }

    public AnimatedObject Wheel1 { get; }
    public AnimatedObject Wheel2 { get; }
    public AnimatedObject Wheel3 { get; }
    public AnimatedObject Wheel4 { get; }
    public AnimatedObject Cursor { get; }
    public AnimatedObject Stem { get; }
    public AnimatedObject Steam { get; }
    public AnimatedObject LanguageList { get; }
    public AnimatedObject GameModeList { get; }
    public AnimatedObject GameLogo { get; }
    public AnimatedObject[] SlotLumIcons { get; }
    public AnimatedObject[] SlotCageIcons { get; }
    public AnimatedObject[] SlotIcons { get; }
    public SpriteTextObject[] SlotLumTexts { get; }
    public SpriteTextObject[] SlotCageTexts { get; }
    public AnimatedObject[] SlotEmptyTexts { get; }
    public AnimatedObject StartEraseSelection { get; }
    public AnimatedObject StartEraseCursor { get; }
}