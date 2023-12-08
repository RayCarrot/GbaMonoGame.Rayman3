using System;
using System.Linq;
using ImGuiNET;

namespace OnyxCs.Gba;

public class PerformanceDebugWindow : DebugWindow
{
    private Graph FrameRateGraph { get; } = new(200);
    private Graph MemoryUsageGraph { get; } = new(200);
    private Graph UpdateTimeGraph { get; } = new(200);
    
    private int MinorFrameRateDrops { get; set; }
    private int MediumFrameRateDrops { get; set; }
    private int MajorFrameRateDrops { get; set; }
    
    public override string Name => "Performance";

    public void AddFps(float fps)
    {
        FrameRateGraph.Add((float)Math.Round(fps));

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

        MemoryUsageGraph.Add(mem);
    }

    public void AddUpdateTime(float time)
    {
        UpdateTimeGraph.Add(time);
    }

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        FrameRateGraph.Draw("Fps", 0, 60, new System.Numerics.Vector2(800, 80));
        ImGui.Text($"Major fps drops: {MajorFrameRateDrops}");
        ImGui.Text($"Medium fps drops: {MediumFrameRateDrops}");
        ImGui.Text($"Minor fps drops: {MinorFrameRateDrops}");

        UpdateTimeGraph.Draw("Update time", 0, 1000 / 60f, new System.Numerics.Vector2(800, 80));
        MemoryUsageGraph.Draw("Memory (mb)", 0, 0x400, new System.Numerics.Vector2(800, 200));
    }

    private class Graph
    {
        public Graph(int length)
        {
            Values = new float[length];
        }

        private float[] Values { get; }
        private int Index { get; set; }

        public void Add(float value)
        {
            if (Index == Values.Length)
            {
                Array.Copy(Values, 1, Values, 0, Values.Length - 1);
                Values[^1] = value;
            }
            else
            {
                Values[Index] = value;
                Index++;
            }
        }

        public void Draw(string label, float min, float max, System.Numerics.Vector2 size)
        {
            ImGui.PlotLines(
                label: label,
                values: ref Values[0],
                values_count: Index,
                values_offset: 0,
                overlay_text: $"{Values.ElementAtOrDefault(Index - 1):F}",
                scale_min: min,
                scale_max: max,
                graph_size: size);
        }
    }
}