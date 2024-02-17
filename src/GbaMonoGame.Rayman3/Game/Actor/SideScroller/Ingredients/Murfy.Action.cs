namespace GbaMonoGame.Rayman3;

public partial class Murfy
{
    private new Action ActionId
    {
        get => (Action)base.ActionId;
        set => base.ActionId = (int)value;
    }

    private enum Action
    {
        BeginIdle_Right = 0,
        BeginIdle_Left = 1,
        BeginLeave_Right = 2,
        BeginLeave_Left = 3,
        Idle_Right = 4,
        Idle_Left = 5,
        Talk1_Right = 6, 
        Talk1_Left = 7, 
        Talk2_Right = 8, 
        Talk2_Left = 9, 
        Talk3_Right = 10, 
        Talk3_Left = 11, 
        Talk4_Right = 12, 
        Talk4_Left = 13, 
        Talk5_Right = 14, 
        Talk5_Left = 15, 
        Fly_Right = 16, 
        Fly_Left = 17, 
    }
}