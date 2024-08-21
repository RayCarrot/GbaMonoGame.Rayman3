using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public partial class GameCubeMenu : Frame
{
    #region Constructor

    public GameCubeMenu()
    {
        SoundEventsManager.StopAllSongs();

        // Use filesystem for now. In the future we can allow JoyBus mode and perhaps connect to
        // Dolphin through TCP (see https://github.dev/mgba-emu/mgba/tree/master/src/gba/sio).
        UseJoyBus = false;
    }

    #endregion

    #region Private Properties

    private AnimationPlayer AnimationPlayer { get; set; }
    private TransitionsFX TransitionsFX { get; set; }
    private GameCubeMenuData Data { get; set; }
    public FiniteStateMachine State { get; } = new();

    private bool UseJoyBus { get; set; }
    private JoyBus JoyBus { get; set; }
    private bool IsJoyBusActive { get; set; }
    private bool WaitingForConnection { get; set; }
    private int MapScroll { get; set; }
    private int SelectedMap { get; set; }
    private byte GbaUnlockFlags { get; set; }
    private byte GcnUnlockFlags { get; set; }
    private bool IsShowingLyChallengeUnlocked { get; set; }
    private bool IsActive { get; set; }
    private int WheelRotation { get; set; }
    private int Timer { get; set; }
    private int MapInfoFileSize { get; set; }

    // Downloaded
    private GameCubeMapInfos MapInfos { get; set; }
    private GameCubeMap Map { get; set; }

    #endregion

    #region Private Methods

    private bool IsMapUnlocked(int mapId)
    {
        int lums = GameInfo.GetTotalCollectedYellowLums();
        return lums >= (mapId + 1) * 100 &&
               GameInfo.PersistentInfo.CompletedGCNBonusLevels >= mapId;
    }

    private bool IsMapCompleted(int mapId)
    {
        return GameInfo.PersistentInfo.CompletedGCNBonusLevels > mapId;
    }

    private void ShowPleaseConnectText()
    {
        string[] text = Localization.GetText(11, 6);

        for (int i = 0; i < text.Length; i++)
        {
            SpriteTextObject textObj = i == 0 ? Data.StatusText : Data.ReusableTexts[i - 1];

            textObj.Color = TextColor.GameCubeMenu;
            textObj.Text = text[i];
            textObj.ScreenPos = new Vector2(140 - textObj.GetStringWidth() / 2f, 34 + i * 14);
        }
    }

    private void MapSelectionUpdateText()
    {
        // Set text colors
        int selectedIndex = SelectedMap - MapScroll;
        for (int i = 0; i < 3; i++)
            Data.ReusableTexts[i].Color = i == selectedIndex ? TextColor.GameCubeMenu : TextColor.GameCubeMenuFaded;

        // Update animations and texts
        for (int i = 0; i < 3; i++)
        {
            MapSelectionUpdateAnimations(MapScroll + i, i);
            Data.LumRequirementTexts[i].Text = ((MapScroll + i + 1) * 100).ToString();
            Data.ReusableTexts[i].Text = MapInfos.Maps[MapScroll + i].Name;
        }
    }

    private void MapSelectionUpdateAnimations(int mapId, int index)
    {
        if (!IsMapUnlocked(mapId))
            Data.LevelChecks[index].CurrentAnimation = 2;
        else if (!IsMapCompleted(mapId))
            Data.LevelChecks[index].CurrentAnimation = 0;
        else
            Data.LevelChecks[index].CurrentAnimation = 1;
    }

    private void ResetReusableTexts()
    {
        for (int i = 0; i < 3; i++)
            Data.ReusableTexts[i].ScreenPos = new Vector2(85, 36 + i * 24);
    }

    #endregion

    #region Public Methods

    public override void Init()
    {
        Engine.GameViewPort.SetResolutionBoundsToOriginalResolution();

        AnimationPlayer = new AnimationPlayer(false, null);

        Gfx.AddScreen(new GfxScreen(2)
        {
            IsEnabled = true,
            Priority = 1,
            Offset = Vector2.Zero,
            Renderer = new TextureScreenRenderer(Engine.TextureCache.GetOrCreateObject(
                pointer: Engine.Loader.Rayman3_GameCubeMenuBitmap.Offset,
                id: 0,
                createObjFunc: static () => new BitmapTexture2D(
                    width: (int)Engine.GameViewPort.OriginalGameResolution.X,
                    height: (int)Engine.GameViewPort.OriginalGameResolution.Y,
                    bitmap: Engine.Loader.Rayman3_GameCubeMenuBitmap.ImgData,
                    palette: new Palette(Engine.Loader.Rayman3_GameCubeMenuPalette))))
        });

        Data = new GameCubeMenuData();
        
        JoyBus = new JoyBus();
        JoyBus.Connect();
        // TODO: If we use this we should allow both PAL and NTSC regions. Currently this is for the PAL region (AYZP & GRHP).
        JoyBus.SetRegion(0x41595a50, 0x47524850);
        IsJoyBusActive = true;
        
        MapScroll = 0;
        SelectedMap = 0;

        GbaUnlockFlags = 0;
        GcnUnlockFlags = 0;
        IsShowingLyChallengeUnlocked = false;

        if (GameInfo.HasCollectedAllYellowLums())
            GcnUnlockFlags |= 1;

        if (GameInfo.HasCollectedAllCages())
            GcnUnlockFlags |= 2;

        if (GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.BossFinal_M2)
            GcnUnlockFlags |= 4;

        if (GameInfo.PersistentInfo.FinishedLyChallenge1 &&
            GameInfo.PersistentInfo.FinishedLyChallenge2 &&
            GameInfo.PersistentInfo.UnlockedBonus1 &&
            GameInfo.PersistentInfo.UnlockedBonus2 &&
            GameInfo.PersistentInfo.UnlockedBonus3 &&
            GameInfo.PersistentInfo.UnlockedBonus4 &&
            GameInfo.PersistentInfo.UnlockedWorld2 &&
            GameInfo.PersistentInfo.UnlockedWorld3 &&
            GameInfo.PersistentInfo.UnlockedWorld4 &&
            GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.BossFinal_M2)
        {
            GcnUnlockFlags |= 8;
        }

        WheelRotation = 0;
        WaitingForConnection = false;
        IsActive = true;
        State.MoveTo(Fsm_PreInit);
    }

    public override void UnInit()
    {
        if (IsJoyBusActive)
            JoyBus.Disconnect();

        Gfx.SetFullFade();
    }

    public override void Step()
    {
        State.Step();

        WheelRotation += 4;

        if (WheelRotation >= 2048)
            WheelRotation = 0;

        Data.Wheel1.AffineMatrix = new AffineMatrix(WheelRotation % 256, 1, 1);
        Data.Wheel2.AffineMatrix = new AffineMatrix(255 - WheelRotation / 2f % 256, 1, 1);
        Data.Wheel3.AffineMatrix = new AffineMatrix(WheelRotation / 4f % 256, 1, 1);
        Data.Wheel4.AffineMatrix = new AffineMatrix(WheelRotation / 8f % 256, 1, 1);

        AnimationPlayer.Play(Data.Wheel1);
        AnimationPlayer.Play(Data.Wheel2);
        AnimationPlayer.Play(Data.Wheel3);
        AnimationPlayer.Play(Data.Wheel4);

        if (WaitingForConnection)
        {
            foreach (SpriteTextObject text in Data.ReusableTexts)
                AnimationPlayer.Play(text);
        }
        else if (State == Fsm_DownloadMap)
        {
            AnimationPlayer.Play(Data.ReusableTexts[0]);
            AnimationPlayer.Play(Data.ReusableTexts[1]);
        }
        else if (State == Fsm_SelectMap)
        {
            if (IsShowingLyChallengeUnlocked)
            {
                AnimationPlayer.Play(Data.ReusableTexts[0]);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    AnimationPlayer.Play(Data.ReusableTexts[i]);
                    AnimationPlayer.Play(Data.LumRequirementTexts[i]);
                    AnimationPlayer.Play(Data.LumIcons[i]);
                    AnimationPlayer.Play(Data.LevelChecks[i]);
                }
            }
        }

        AnimationPlayer.Play(Data.TotalLumsText);

        if (WaitingForConnection || State == Fsm_DownloadMap || State == Fsm_SelectMap || State == Fsm_DownloadMapAck)
            AnimationPlayer.Play(Data.StatusText);

        TransitionsFX.StepAll();
        AnimationPlayer.Execute();
    }

    #endregion
}