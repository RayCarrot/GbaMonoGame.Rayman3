using System;

namespace GbaMonoGame;

public class CallBackSet
{
    public CallBackSet(Func<object, Vector2> getObjectPosition, Func<object, Vector2> getMikePosition, Func<int> getSwitchIndex)
    {
        GetObjectPosition = getObjectPosition;
        GetMikePosition = getMikePosition;
        GetSwitchIndex = getSwitchIndex;
    }

    public Func<object, Vector2> GetObjectPosition { get; }
    public Func<object, Vector2> GetMikePosition { get; }
    public Func<int> GetSwitchIndex { get; } // Unused in Rayman 3
}