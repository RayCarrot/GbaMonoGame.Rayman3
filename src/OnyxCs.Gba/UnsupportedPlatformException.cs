using System;

namespace OnyxCs.Gba;

public class UnsupportedPlatformException : Exception
{
    public UnsupportedPlatformException() { }
    public UnsupportedPlatformException(string message) : base(message) { }
    public UnsupportedPlatformException(string message, Exception inner) : base(message, inner) { }
}