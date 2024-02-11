using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// TODO: Implement
public class TextBoxDialog : Dialog
{
    private int StringId { get; set; }

    public void UpdateText1()
    {
        // TODO: Implement
    }

    public void UpdateText2()
    {
        // TODO: Implement
    }

    public void SetText(int stringId)
    {
        StringId = stringId;
        UpdateText1();
        UpdateText2();
    }

    public void SetCutsceneCharacter(TextBoxCutsceneCharacter cutsceneCharacter)
    {
        // TODO: Implement
    }

    public void FUN_10077108(bool param1)
    {
        // TODO: Implement
        /*
        if (param1)
        {
            Fsm.ChangeAction(FUN_08016f70); // FUN_100771e4
        }
        else
        {
            Fsm.ChangeAction(); // FUN_100775f0
        }
        */
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        return false;
    }

    public override void Load()
    {
        // TODO: Implement
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        // TODO: Implement
    }
}