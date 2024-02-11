namespace GbaMonoGame.Rayman3;

public class BossMachine : FrameSideScroller
{
    public BossMachine(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        TextBoxDialog textBox = new();
        Scene.AddDialog(textBox, false, false);
        textBox.SetCutsceneCharacter(TextBoxCutsceneCharacter.Murfy);
        textBox.SetText(13);
    }
}