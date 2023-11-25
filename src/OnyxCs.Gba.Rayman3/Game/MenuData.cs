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
        AnimatedObjectResource steamAnimations = Storage.LoadResource<AnimatedObjectResource>(GameResource.MenuSteamAnimations);

        LanguageList = new AnimatedObject(languageListAnimations, languageListAnimations.IsDynamic)
        {
            Priority = 0,
            ScreenPos = new Vector2(120, 28)
        };
        LanguageList.SetCurrentAnimation(0);

        Wheel1 = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(7, 110)
        };
        Wheel1.SetCurrentAnimation(2);
        Wheel1.AffineMatrix = AffineMatrix.Identity;

        Wheel2 = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(136, 110)
        };
        Wheel2.SetCurrentAnimation(3);
        Wheel2.AffineMatrix = AffineMatrix.Identity;

        Wheel3 = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(172, 110)
        };
        Wheel3.SetCurrentAnimation(4);
        Wheel3.AffineMatrix = AffineMatrix.Identity;

        Wheel4 = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(66, 144)
        };
        Wheel4.SetCurrentAnimation(3);
        Wheel4.AffineMatrix = AffineMatrix.Identity;

        Cursor = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(33, 67)
        };
        Cursor.SetCurrentAnimation(0);

        Stem = new AnimatedObject(propsAnimations, propsAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(47, 160)
        };
        Stem.SetCurrentAnimation(14);

        Steam = new AnimatedObject(steamAnimations, steamAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(27, 20)
        };
        Steam.SetCurrentAnimation(0);

        GameLogo = new AnimatedObject(gameLogoAnimations, gameLogoAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(174, 16)
        };
        GameLogo.SetCurrentAnimation(0);

        GameModeList = new AnimatedObject(gameModeAnimations, gameModeAnimations.IsDynamic)
        {
            Priority = 3,
            ScreenPos = new Vector2(73, 52)
        };
        GameModeList.SetCurrentAnimation(0);

        StartEraseSelection = new AnimatedObject(startEraseAnimations, startEraseAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(80, 30)
        };
        StartEraseSelection.SetCurrentAnimation(1);

        StartEraseCursor = new AnimatedObject(startEraseAnimations, startEraseAnimations.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(106, 12)
        };
        StartEraseCursor.SetCurrentAnimation(40);
    }

    public AnimatedObject LanguageList { get; }
    public AnimatedObject Wheel1 { get; }
    public AnimatedObject Wheel2 { get; }
    public AnimatedObject Wheel3 { get; }
    public AnimatedObject Wheel4 { get; }
    public AnimatedObject Cursor { get; }
    public AnimatedObject Stem { get; }
    public AnimatedObject Steam { get; }
    public AnimatedObject GameLogo { get; }
    public AnimatedObject GameModeList { get; }
    public AnimatedObject StartEraseSelection { get; }
    public AnimatedObject StartEraseCursor { get; }
}