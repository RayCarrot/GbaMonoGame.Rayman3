using Microsoft.Xna.Framework.Input;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.MonoGame;

public class MonoGameJoyPad : JoyPad
{
    private KeyboardState _state;

    public override bool Check(Input input)
    {
        // TODO: Add all inputs and read key bindings from config. Also support a controller.

        if ((input & Input.Start) != 0 && _state.IsKeyDown(Keys.V))
            return true;

        return false;
    }

    public override void Scan()
    {
        _state = Keyboard.GetState();
    }
}