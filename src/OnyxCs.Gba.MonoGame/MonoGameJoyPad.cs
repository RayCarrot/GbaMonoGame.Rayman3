using Microsoft.Xna.Framework.Input;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.MonoGame;

public class MonoGameJoyPad : JoyPad
{
    private KeyboardState _state;

    public override bool Check(Input input)
    {
        if ((input & Input.Start) != 0)
            return _state.IsKeyDown(Keys.V);

        return false;
    }

    public override void Scan()
    {
        _state = Keyboard.GetState();
    }
}