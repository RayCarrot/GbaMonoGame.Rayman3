using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public class GameConfig
{
    #region Constructor

    public GameConfig()
    {
        DisplayMode defaultDisplayMode = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Last();

        FullscreenResolution = new Point(defaultDisplayMode.Width, defaultDisplayMode.Height);
        IsFullscreen = true;
        GbaWindowResolution = new Point(240 * 4, 160 * 4);
        NGageWindowResolution = new Point(176 * 4, 208 * 4);
        GbaWindowPosition = null;
        NGageWindowPosition = null;
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
    [JsonPropertyName("fullscreenResolution")] public Point FullscreenResolution { get; set; }
    [JsonPropertyName("isFullscreen")] public bool IsFullscreen { get; set; }
    [JsonPropertyName("gbaWindowResolution")] public Point GbaWindowResolution { get; set; }
    [JsonPropertyName("nGageWindowResolution")] public Point NGageWindowResolution { get; set; }
    [JsonPropertyName("gbaWindowPosition")] public Point? GbaWindowPosition { get; set; }
    [JsonPropertyName("nGageWindowPosition")] public Point? NGageWindowPosition { get; set; }
    [JsonPropertyName("internalResolution")] public Point? InternalResolution { get; set; }
    [JsonPropertyName("playfieldCameraScale")] public float PlayfieldCameraScale { get; set; }
    [JsonPropertyName("hudCameraScale")] public float HudCameraScale { get; set; }

    // Sound
    [JsonPropertyName("sfxVolume")] public float SfxVolume { get; set; }
    [JsonPropertyName("musicVolume")] public float MusicVolume { get; set; }

    // Debug
    [JsonPropertyName("writeSerializerLog")] public bool WriteSerializerLog { get; set; } // TODO: Add debug option to enable this
    [JsonPropertyName("dumpSprites")] public bool DumpSprites { get; set; } // TODO: Add debug option to enable this

    #endregion

    #region Private Methods

    private static JsonSerializerOptions GetJsonOptions()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            IncludeFields = true, // Need this for the Point struct
        };

        return options;
    }

    #endregion

    #region Public Methods

    public static GameConfig Load(string filePath)
    {
        if (File.Exists(filePath))
            return JsonSerializer.Deserialize<GameConfig>(File.ReadAllText(filePath), GetJsonOptions());
        else
            return new GameConfig();
    }

    public void Save(string filePath)
    {
        ConfigChanged?.Invoke(this, EventArgs.Empty);
        File.WriteAllText(filePath, JsonSerializer.Serialize(this, GetJsonOptions()));
    }

    #endregion
}