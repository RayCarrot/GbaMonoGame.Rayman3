using System.Text.Json.Serialization;

namespace OnyxCs.Gba.Rayman3;

public record GameConfig
{
    [JsonPropertyName("romFile")]
    public string RomFile { get; set; } = "ROM.gba";

    [JsonPropertyName("serializerLogFile")]
    public string SerializerLogFile { get; set; }
}