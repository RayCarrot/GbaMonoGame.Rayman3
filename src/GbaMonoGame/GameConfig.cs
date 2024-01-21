using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GbaMonoGame;

public class GameConfig
{
    #region Events

    public event EventHandler ConfigChanged;

    #endregion

    #region Public Properties

    [JsonPropertyName("writeSerializerLog")] public bool WriteSerializerLog { get; set; }
    [JsonPropertyName("scale")] public float Scale { get; set; } = 1;

    #endregion

    #region Public Methods

    public static GameConfig Load(string filePath)
    {
        if (File.Exists(filePath))
            return JsonSerializer.Deserialize<GameConfig>(File.ReadAllText(filePath));
        else
            return new GameConfig();
    }

    public void Save(string filePath)
    {
        ConfigChanged?.Invoke(this, EventArgs.Empty);
        File.WriteAllText(filePath, JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true,
        }));
    }

    #endregion
}