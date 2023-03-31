namespace OnyxCs.Gba.Sdk;

public class GameTime
{
    private long _elapsedFrames;
    private bool _isPaused { get; set; }

    public void Pause() => _isPaused = true;
    public void Resume() => _isPaused = false;

    public long GetElapsedFrames() => _elapsedFrames;

    public void Update()
    {
        if (!_isPaused)
            _elapsedFrames++;
    }
}