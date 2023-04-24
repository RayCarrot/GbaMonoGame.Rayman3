using Microsoft.Xna.Framework.Input;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.MonoGame;

public class MonoGameJoyPad : JoyPad
{
    private KeyboardState _state;

    public override void Scan()
    {
        _state = Keyboard.GetState();
    }
}