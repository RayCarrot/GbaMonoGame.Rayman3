using System.Text.Json.Serialization;

namespace OnyxCs.Gba.MonoGame.Rayman3;

public record GameConfig
{
    [JsonPropertyName("romFile")]
    public string RomFile { get; set; } = "ROM.gba";

    [JsonPropertyName("serializerLogFile")]
    public string SerializerLogFile { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; } = 240;
    
    [JsonPropertyName("height")]
    public int Height { get; set; } = 160;
    
    [JsonPropertyName("scale")]
    public float Scale { get; set; } = 2;
}