using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace GbaMonoGame;

public class GameConfig
{
    #region Constructor

    public GameConfig()
    {
        DisplayMode defaultDisplayMode = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Last();

        FullscreenResolution = new Point(defaultDisplayMode.Width, defaultDisplayMode.Height);
        IsFullscreen = true;
        GbaWindowBounds = new Rectangle(0, 0, 240 * 4, 160 * 4);
        NGageWindowBounds = new Rectangle(0, 0, 176 * 4, 208 * 4);
        InternalResolution = null;
        PlayfieldCameraScale = 1;
        HudCameraScale = 1;
        SfxVolume = 1;
        MusicVolume = 1;
        WriteSerializerLog = false;
        DumpSprites = false;
    }

    #endregion

    #region Events

    public event EventHandler ConfigChanged;

    #endregion

    #region Public Properties

    // Display
    [JsonProperty("fullscreenResolution")] public Point FullscreenResolution { get; set; }
    [JsonProperty("isFullscreen")] public bool IsFullscreen { get; set; }
    [JsonProperty("gbaWindowBounds")] public Rectangle GbaWindowBounds { get; set; }
    [JsonProperty("nGageWindowBounds")] public Rectangle NGageWindowBounds { get; set; }
    
    // Game
    [JsonProperty("internalResolution")] public Point? InternalResolution { get; set; }
    [JsonProperty("playfieldCameraScale")] public float PlayfieldCameraScale { get; set; }
    [JsonProperty("hudCameraScale")] public float HudCameraScale { get; set; }

    // Sound
    [JsonProperty("sfxVolume")] public float SfxVolume { get; set; }
    [JsonProperty("musicVolume")] public float MusicVolume { get; set; }

    // Debug
    [JsonProperty("writeSerializerLog")] public bool WriteSerializerLog { get; set; }
    [JsonProperty("dumpSprites")] public bool DumpSprites { get; set; }

    #endregion

    #region Private Methods

    private static JsonSerializerSettings GetJsonSettings()
    {
        JsonSerializerSettings settings = new()
        {
            Formatting = Formatting.Indented,
        };

        return settings;
    }

    #endregion

    #region Public Methods

    public static GameConfig Load(string filePath)
    {
        if (File.Exists(filePath))
            return JsonConvert.DeserializeObject<GameConfig>(File.ReadAllText(filePath), GetJsonSettings());
        else
            return new GameConfig();
    }

    public void Save(string filePath)
    {
        ConfigChanged?.Invoke(this, EventArgs.Empty);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(this, GetJsonSettings()));
    }

    #endregion
}