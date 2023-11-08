namespace OnyxCs.Gba.Rayman3;

// TODO: Add N-Gage support
public class MenuData
{
    public MenuData()
    {
        AnimatedObjectResource menuResource1 = Storage.LoadResource<AnimatedObjectResource>(0x5C);
        AnimatedObjectResource menuResource2 = Storage.LoadResource<AnimatedObjectResource>(0x60);
        AnimatedObjectResource menuResource3 = Storage.LoadResource<AnimatedObjectResource>(0x61);
        AnimatedObjectResource menuResource4 = Storage.LoadResource<AnimatedObjectResource>(0x62);
        AnimatedObjectResource menuResource5 = Storage.LoadResource<AnimatedObjectResource>(0x69);

        LanguageList = new AnimatedObject(menuResource4, menuResource4.IsDynamic)
        {
            Priority = 0,
            ScreenPos = new Vector2(120, 28)
        };
        LanguageList.SetCurrentAnimation(0);

        Wheel1 = new AnimatedObject(menuResource1, menuResource1.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(7, 110)
        };
        Wheel1.SetCurrentAnimation(2);
        Wheel1.AffineMatrix = AffineMatrix.Identity;

        Wheel2 = new AnimatedObject(menuResource1, menuResource1.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(136, 110)
        };
        Wheel2.SetCurrentAnimation(3);
        Wheel2.AffineMatrix = AffineMatrix.Identity;

        Wheel3 = new AnimatedObject(menuResource1, menuResource1.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(172, 110)
        };
        Wheel3.SetCurrentAnimation(4);
        Wheel3.AffineMatrix = AffineMatrix.Identity;

        Wheel4 = new AnimatedObject(menuResource1, menuResource1.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(66, 144)
        };
        Wheel4.SetCurrentAnimation(3);
        Wheel4.AffineMatrix = AffineMatrix.Identity;

        Cursor = new AnimatedObject(menuResource1, menuResource1.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(33, 67)
        };
        Cursor.SetCurrentAnimation(0);

        Stem = new AnimatedObject(menuResource1, menuResource1.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(47, 160)
        };
        Stem.SetCurrentAnimation(14);

        Steam = new AnimatedObject(menuResource5, menuResource5.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(27, 20)
        };
        Steam.SetCurrentAnimation(0);

        GameLogo = new AnimatedObject(menuResource2, menuResource2.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(174, 16)
        };
        GameLogo.SetCurrentAnimation(0);

        GameModeList = new AnimatedObject(menuResource3, menuResource3.IsDynamic)
        {
            Priority = 3,
            ScreenPos = new Vector2(73, 52)
        };
        GameModeList.SetCurrentAnimation(0);
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
}