namespace OnyxCs.Gba.Rayman3;

public class MenuData
{
    public MenuData()
    {
        AnimatedObjectResource menuAnimResource = Storage.LoadResource<AnimatedObjectResource>(0x62);

        LanguageList = new AnimatedObject(menuAnimResource, false)
        {
            Priority = 0,
            ScreenPos = new Vector2(0x78, 0x1c)
        };

        LanguageList.SetCurrentAnimation(0);
    }

    public AnimatedObject LanguageList { get; }
}