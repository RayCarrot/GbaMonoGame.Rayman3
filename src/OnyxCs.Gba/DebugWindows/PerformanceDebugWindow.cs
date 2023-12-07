using System;
using System.Linq;
using ImGuiNET;

namespace OnyxCs.Gba;

public class PerformanceDebugWindow : DebugWindow
{
    private float[] FrameRates { get; } = new float[200];
    private int FrameRateIndex { get; set; }
    
    private float[] MemoryUsage { get; } = new float[200];
    private int MemoryUsageIndex { get; set; }
    
    private int MinorFrameRateDrops { get; set; }
    private int MediumFrameRateDrops { get; set; }
    private int MajorFrameRateDrops { get; set; }
    
    public override string Name => "Performance";

    public void AddFps(float fps)
    {
        if (FrameRateIndex == FrameRates.Length)
        {
            Array.Copy(FrameRates, 1, FrameRates, 0, FrameRates.Length - 1);
            FrameRates[^1] = fps;
        }
        else
        {
            FrameRates[FrameRateIndex] = fps;
            FrameRateIndex++;
        }

        if (fps < 50)
            MajorFrameRateDrops++;
        else if (fps < 57)
            MediumFrameRateDrops++;
        else if (fps < 59)
            MinorFrameRateDrops++;
    }

    public void AddMemoryUsage(float mem)
    {
        // Get mb from bytes
        mem /= 0x100000;

        if (MemoryUsageIndex == MemoryUsage.Length)
        {
            Array.Copy(MemoryUsage, 1, MemoryUsage, 0, MemoryUsage.Length - 1);
            MemoryUsage[^1] = mem;
        }
        else
        {
            MemoryUsage[MemoryUsageIndex] = mem;
            MemoryUsageIndex++;
        }
    }

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        ImGui.PlotLines(
            label: "Fps", 
            values: ref FrameRates[0], 
            values_count: FrameRateIndex, 
            values_offset: 0, 
            overlay_text: $"{FrameRates.ElementAtOrDefault(FrameRateIndex - 1):F}", 
            scale_min: 0, 
            scale_max: 60, 
            graph_size: new System.Numerics.Vector2(800, 80));
        ImGui.Text($"Major fps drops: {MajorFrameRateDrops}");
        ImGui.Text($"Medium fps drops: {MediumFrameRateDrops}");
        ImGui.Text($"Minor fps drops: {MinorFrameRateDrops}");

        ImGui.PlotLines(
            label: "Memory (gb)", 
            values: ref MemoryUsage[0], 
            values_count: MemoryUsageIndex, 
            values_offset: 0, 
            overlay_text: $"{MemoryUsage.ElementAtOrDefault(MemoryUsageIndex - 1):F}", 
            scale_min: 0, 
            scale_max: 0x400, 
            graph_size: new System.Numerics.Vector2(800, 200));
    }
}