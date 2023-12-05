using System.Linq;
using BinarySerializer.Onyx.Gba.Rayman3;

namespace OnyxCs.Gba.Rayman3;

public static class Localization
{
    public static int Language { get; private set; }
    public static int LanguageUiIndex { get; private set; }
    
    public static TextBank[] TextBanks { get; private set; }

    public static void SetLanguage(int language)
    {
        Language = language;
        LanguageUiIndex = language; // TODO: On N-Gage this is different!

        TextBanks = Engine.Loader.Rayman3_LocalizedTextBanks.TextBanks[language].Value.Select(x => x.Value).ToArray();
    }
}