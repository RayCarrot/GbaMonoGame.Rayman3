using System.Text.Json.Serialization;

namespace OnyxCs.Gba.Rayman3;

// TODO: Save config on close
public record GameConfig
{
    [JsonPropertyName("romFile")]
    public string RomFile { get; set; } = "ROM.gba";

    [JsonPropertyName("serializerLogFile")]
    public string SerializerLogFile { get; set; }
    
    [JsonPropertyName("debug")]
    public bool Debug { get; set; }

    [JsonPropertyName("paused")]
    public bool Paused { get; set; }
}