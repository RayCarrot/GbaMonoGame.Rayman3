using System.Text.Json.Serialization;

namespace OnyxCs.Gba;

public record GameConfig
{
    [JsonPropertyName("romFile")]
    public string RomFile { get; set; }

    [JsonPropertyName("serializerLogFile")]
    public string SerializerLogFile { get; set; }
}