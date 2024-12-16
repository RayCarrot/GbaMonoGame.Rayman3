using System;
using System.Linq;
using ImGuiNET;

namespace GbaMonoGame;

public class PerformanceDebugWindow : DebugWindow
{
    private readonly Graph _frameRateGraph = new(200);
    private readonly Graph _skippedDrawsGraph = new(200);
    private readonly Graph _memoryUsageGraph = new(200);
    private readonly Graph _updateTimeGraph = new(200);
    private readonly Graph _drawCallsGraph = new(200);

    private int _minorFrameRateDrops;
    private int _mediumFrameRateDrops;
    private int _majorFrameRateDrops;
    
    public override string Name => "Performance";

    public void AddFps(float fps)
    {
        _frameRateGraph.Add((float)Math.Round(fps));

        if (fps < 50)
            _majorFrameRateDrops++;
        else if (fps < 57)
            _mediumFrameRateDrops++;
        else if (fps < 59)
            _minorFrameRateDrops++;
    }

    public void AddSkippedDraws(float skippedDraws)
    {
        _skippedDrawsGraph.Add(skippedDraws);
    }

    public void AddMemoryUsage(float mem)
    {
        // Get mb from bytes
        mem /= 0x100000;

        _memoryUsageGraph.Add(mem);
    }

    public void AddUpdateTime(float time)
    {
        _updateTimeGraph.Add(time);
    }

    public void AddDrawCalls(float drawCalls)
    {
        _drawCallsGraph.Add(drawCalls);
    }

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        _frameRateGraph.Draw("Fps", 0, 60, new System.Numerics.Vector2(800, 80));
        ImGui.Text($"Major fps drops: {_majorFrameRateDrops}");
        ImGui.Text($"Medium fps drops: {_mediumFrameRateDrops}");
        ImGui.Text($"Minor fps drops: {_minorFrameRateDrops}");

        ImGui.Spacing();

        _skippedDrawsGraph.Draw("Skipped draws", 0, 20, new System.Numerics.Vector2(800, 80));

        ImGui.Spacing();

        _updateTimeGraph.Draw("Update time", 0, 1000 / 60f, new System.Numerics.Vector2(800, 80));
        
        ImGui.Spacing();
        
        _memoryUsageGraph.Draw("Memory (mb)", 0, 0x400, new System.Numerics.Vector2(800, 200));
        
        ImGui.Spacing();
        
        _drawCallsGraph.Draw("Draw calls", 0, 500, new System.Numerics.Vector2(800, 80));
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