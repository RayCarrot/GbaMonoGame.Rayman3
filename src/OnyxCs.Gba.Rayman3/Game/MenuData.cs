namespace OnyxCs.Gba.Rayman3;

// TODO: Add N-Gage support
public class MenuData
{
    public MenuData()
    {
        AnimatedObjectResource menuResource_5C = Storage.LoadResource<AnimatedObjectResource>(0x5C);
        AnimatedObjectResource menuResource_5D = Storage.LoadResource<AnimatedObjectResource>(0x5D);
        AnimatedObjectResource menuResource_60 = Storage.LoadResource<AnimatedObjectResource>(0x60);
        AnimatedObjectResource menuResource_61 = Storage.LoadResource<AnimatedObjectResource>(0x61);
        AnimatedObjectResource menuResource_62 = Storage.LoadResource<AnimatedObjectResource>(0x62);
        AnimatedObjectResource menuResource_69 = Storage.LoadResource<AnimatedObjectResource>(0x69);

        LanguageList = new AnimatedObject(menuResource_62, menuResource_62.IsDynamic)
        {
            Priority = 0,
            ScreenPos = new Vector2(120, 28)
        };
        LanguageList.SetCurrentAnimation(0);

        Wheel1 = new AnimatedObject(menuResource_5C, menuResource_5C.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(7, 110)
        };
        Wheel1.SetCurrentAnimation(2);
        Wheel1.AffineMatrix = AffineMatrix.Identity;

        Wheel2 = new AnimatedObject(menuResource_5C, menuResource_5C.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(136, 110)
        };
        Wheel2.SetCurrentAnimation(3);
        Wheel2.AffineMatrix = AffineMatrix.Identity;

        Wheel3 = new AnimatedObject(menuResource_5C, menuResource_5C.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(172, 110)
        };
        Wheel3.SetCurrentAnimation(4);
        Wheel3.AffineMatrix = AffineMatrix.Identity;

        Wheel4 = new AnimatedObject(menuResource_5C, menuResource_5C.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(66, 144)
        };
        Wheel4.SetCurrentAnimation(3);
        Wheel4.AffineMatrix = AffineMatrix.Identity;

        Cursor = new AnimatedObject(menuResource_5C, menuResource_5C.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(33, 67)
        };
        Cursor.SetCurrentAnimation(0);

        Stem = new AnimatedObject(menuResource_5C, menuResource_5C.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(47, 160)
        };
        Stem.SetCurrentAnimation(14);

        Steam = new AnimatedObject(menuResource_69, menuResource_69.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(27, 20)
        };
        Steam.SetCurrentAnimation(0);

        GameLogo = new AnimatedObject(menuResource_60, menuResource_60.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(174, 16)
        };
        GameLogo.SetCurrentAnimation(0);

        GameModeList = new AnimatedObject(menuResource_61, menuResource_61.IsDynamic)
        {
            Priority = 3,
            ScreenPos = new Vector2(73, 52)
        };
        GameModeList.SetCurrentAnimation(0);

        StartEraseSelection = new AnimatedObject(menuResource_5D, menuResource_5D.IsDynamic)
        {
            Priority = 1,
            ScreenPos = new Vector2(80, 30)
        };
        StartEraseSelection.SetCurrentAnimation(1);

        StartEraseCursor = new AnimatedObject(menuResource_5D, menuResource_5D.IsDynamic)
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