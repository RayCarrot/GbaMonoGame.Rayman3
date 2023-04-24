using Microsoft.Xna.Framework.Input;
using OnyxCs.Gba.Sdk;

public class MonoGameJoyPad : JoyPad
{
    private KeyboardState _state;

    public override void Scan()
    {
        _state = Keyboard.GetState();
    }
}