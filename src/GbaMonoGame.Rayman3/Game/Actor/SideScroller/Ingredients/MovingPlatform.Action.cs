namespace GbaMonoGame.Rayman3;

public partial class MovingPlatform
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        Stationary = 0,
        
        Move_Left = 1,
        Move_Right = 2,
        Move_Up = 3,
        Move_Down = 4,
        
        Move_DownLeft = 5,
        Move_DownRight = 6,
        Move_UpRight = 7,
        Move_UpLeft = 8,

        WaitForProximity = 9,
        WaitForContact = 10,
        WaitForContactWithReturn = 11,

        Impact = 12,
        
        MoveAccelerated_Left = 13,
        MoveAccelerated_Right = 14,
        MoveAccelerated_Up = 15,
        MoveAccelerated_Down = 16,
        
        Unused_Left = 17,
        Unused_Right = 18,
        Unused_Up = 19,
        Unused_Down = 20,
    }
}