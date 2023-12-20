using System;
using System.Text.Json.Serialization;

namespace OnyxCs.Gba;

public class GameConfig
{
    [JsonPropertyName("romFile")]
    public string RomFile { get; set; }

    [JsonPropertyName("saveFile")]
    public string SaveFile { get; set; }

    [JsonPropertyName("serializerLogFile")]
    public string SerializerLogFile { get; set; }

    [JsonPropertyName("scale")]
    public float Scale { get; set; } = 1;

    public event EventHandler ConfigChanged;
    public void OnConfigChanged() => ConfigChanged?.Invoke(this, EventArgs.Empty);
}