using System.IO;
using BinarySerializer;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public class GameCubeMenu : Frame
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
        return GameInfo.PersistentInfo.CompletedGCNBonusLevels >= mapId;
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

    #region FSM

    private void Fsm_PreInit(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ShowPleaseConnectText();
                // TODO: Set up window transition. Rects with heights 54, 52 and 54.
                TransitionsFX = new TransitionsFX(true);
                Timer = 0;
                break;

            case FsmAction.Step:
                Timer += 8;
                
                // TODO: Update transition

                if (Timer >= 240)
                {
                    State.MoveTo(Fsm_WaitForConnection);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_WaitForConnection(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Timer = 0;
                break;

            case FsmAction.Step:
                // If not set to use the JoyBus then we read from the file system
                if (!UseJoyBus)
                {
                    Engine.BeginLoad();

                    string filePath = Path.Combine(Engine.GameInstallation.Directory, "gba.nfo");

                    if (!Engine.Context.FileExists(filePath))
                        Engine.Context.AddFile(new LinearFile(Engine.Context, filePath));

                    // TODO: Handle exception
                    // TODO: Handle file not existing
                    MapInfos = FileFactory.Read<GameCubeMapInfos>(Engine.Context, filePath);

                    State.MoveTo(Fsm_SelectMap);
                    return;
                }

                JoyBus.CheckForLostConnection();
                if (Timer < 10)
                {
                    Timer++;

                    if (Timer == 9)
                    {
                        ShowPleaseConnectText();
                        WaitingForConnection = true;
                    }
                }

                // Exit
                if (JoyPad.CheckSingle(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return;
                }

                // Connected
                if (JoyBus.IsConnected)
                {
                    WaitingForConnection = false;
                    State.MoveTo(Fsm_Connected);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_ConnectionLost(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Data.StatusText.Text = "";
                // Why does the game do this??
                MapInfoFileSize = (int)GameTime.ElapsedFrames;
                break;

            case FsmAction.Step:
                if (JoyPad.CheckSingle(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return;
                }

                ShowPleaseConnectText();
                State.MoveTo(Fsm_WaitForConnection);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Exit(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Back01_Mix01);
                // TODO: Set up window transition
                Timer = 0;
                break;

            case FsmAction.Step:
                Timer++;

                // TODO: Update transition

                if (Timer >= 80)
                {
                    GameInfo.World = 0;
                    GameInfo.LoadLevel(MapId.WorldMap);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Connected(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ResetReusableTexts();
                Data.StatusText.ScreenPos = new Vector2(105, 88);
                Data.StatusText.Text = "";
                break;

            case FsmAction.Step:
                // Lost connection
                if (JoyBus.CheckForLostConnection())
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return;
                }

                // Read data
                byte gbaUnlockFlags = 0;
                bool isValid = true;
                if (JoyBus.HasReceivedData)
                {
                    int data = JoyBus.ReceivedData;
                    MapInfoFileSize = BitHelpers.ExtractBits(data, 16, 0) * 4;
                    gbaUnlockFlags = (byte)BitHelpers.ExtractBits(data, 8, 16);
                    byte check = (byte)BitHelpers.ExtractBits(data, 8, 24);
                    isValid = gbaUnlockFlags == (byte)~check;
                }

                // Exit
                if (JoyPad.CheckSingle(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return;
                }

                // Error
                if (JoyBus.HasReceivedData && JoyBus.IsConnected && !isValid)
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return;
                }

                // Received data - download the map info
                if (JoyBus.HasReceivedData && JoyBus.IsConnected)
                {
                    GbaUnlockFlags = gbaUnlockFlags;
                    State.MoveTo(Fsm_DownloadMapInfo);
                    return;
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return;
                }
                break;

            case FsmAction.UnInit:
                if (IsActive)
                {
                    JoyBus.NewTransfer(MapInfoFileSize);

                    int value = 0;
                    value = BitHelpers.SetBits(value, MapInfoFileSize / 4, 16, 0);
                    value = BitHelpers.SetBits(value, GcnUnlockFlags, 8, 16);
                    value = BitHelpers.SetBits(value, ~GcnUnlockFlags, 8, 24);
                    JoyBus.SendValue(value);
                }
                break;
        }
    }

    private void Fsm_DownloadMapInfo(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                // Lost connection
                if (JoyBus.CheckForLostConnection())
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return;
                }

                // Exit
                if (JoyPad.CheckSingle(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return;
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return;
                }

                // Error
                if (JoyBus.ErrorState == 0xFF)
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return;
                }

                // Finished downloading map info
                if (JoyBus.RemainingSize == 0)
                {
                    State.MoveTo(Fsm_DownloadMapInfoAck);
                    return;
                }
                break;

            case FsmAction.UnInit:
                if (IsActive)
                {
                    JoyBus.SendValue(JoyBus.ReceivedData);
                    JoyBus.HasReceivedData = false;
                    JoyBus.Checksum = 0;
                }
                break;
        }
    }

    private void Fsm_DownloadMapInfoAck(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                // Lost connection
                if (JoyBus.CheckForLostConnection())
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return;
                }

                // Exit
                if (JoyPad.CheckSingle(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return;
                }

                // Select map - Received download acknowledgement
                if (JoyBus.HasReceivedData && JoyBus.ReceivedData == 0x22222222)
                {
                    State.MoveTo(Fsm_SelectMap);
                    return;
                }

                // Error - received invalid data
                if (JoyBus.HasReceivedData)
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return;
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_SelectMap(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                SelectedMap = 0;
                MapScroll = 0;

                // TODO: Have some way of unlocking the Ly challenge without connection - by completing all 10 levels?
                if ((GbaUnlockFlags & 1) != 0 && !GameInfo.PersistentInfo.UnlockedLyChallengeGCN)
                {
                    GameInfo.PersistentInfo.UnlockedLyChallengeGCN = true;

                    string[] text = Localization.GetText(11, 8);

                    Data.StatusText.Text = text[0];
                    Data.StatusText.ScreenPos = new Vector2(140 - Data.StatusText.GetStringWidth() / 2f, 50);

                    Data.ReusableTexts[0].Text = text[1];
                    Data.ReusableTexts[0].ScreenPos = new Vector2(140 - Data.ReusableTexts[0].GetStringWidth() / 2f, 70);

                    IsShowingLyChallengeUnlocked = true;
                }
                else
                {
                    Data.StatusText.Text = "";
                    ResetReusableTexts();
                    MapSelectionUpdateText();
                }
                break;

            case FsmAction.Step:
                // Lost connection
                if (UseJoyBus && JoyBus.CheckForLostConnection())
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return;
                }

                bool hasSelectedMap = false;

                if (IsShowingLyChallengeUnlocked)
                {
                    if (JoyPad.CheckSingle(GbaInput.Start) || JoyPad.CheckSingle(GbaInput.A))
                    {
                        IsShowingLyChallengeUnlocked = false;
                        Data.StatusText.Text = "";
                        ResetReusableTexts();
                        MapSelectionUpdateText();
                    }
                }
                else
                {
                    // Select map
                    if (JoyPad.CheckSingle(GbaInput.Start) || JoyPad.CheckSingle(GbaInput.A))
                    {
                        // Make sure map is unlocked
                        if (IsMapUnlocked(SelectedMap))
                        {
                            if (UseJoyBus)
                            {
                                JoyBus.NewTransfer(MapInfos.Maps[SelectedMap].FileSize);
                                JoyBus.SendValue(3);
                                hasSelectedMap = true;
                            }
                            else
                            {
                                Engine.BeginLoad();

                                string filePath = Path.Combine(Engine.GameInstallation.Directory, $"map.{SelectedMap:000}");

                                if (!Engine.Context.FileExists(filePath))
                                    Engine.Context.AddFile(new LinearFile(Engine.Context, filePath));

                                // TODO: Handle exception
                                // TODO: Handle file not existing
                                Map = FileFactory.Read<GameCubeMap>(Engine.Context, filePath);
                                
                                FrameManager.SetNextFrame(new FrameSideScrollerGCN(MapInfos.Maps[SelectedMap], Map, SelectedMap));
                            }

                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Valid01_Mix01);
                        }
                        else
                        {
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Back01_Mix01);
                        }
                    }

                    // Move up
                    if (JoyPad.CheckSingle(GbaInput.Up) && SelectedMap != 0)
                    {
                        SelectedMap--;
                        if (MapScroll > SelectedMap)
                            MapScroll = SelectedMap;

                        MapSelectionUpdateText();
                        JoyBus.SendValue(2);
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MenuMove);
                    }

                    // Move down
                    if (JoyPad.CheckSingle(GbaInput.Down) && SelectedMap < MapInfos.MapsCount - 1)
                    {
                        SelectedMap++;
                        if (MapScroll + 2 < SelectedMap)
                            MapScroll = SelectedMap - 2;

                        MapSelectionUpdateText();
                        JoyBus.SendValue(1);
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MenuMove);
                    }
                }

                // Exit
                if (JoyPad.CheckSingle(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return;
                }

                if (hasSelectedMap)
                {
                    State.MoveTo(Fsm_DownloadMap);
                    return;
                }

                // Disconnected
                if (UseJoyBus && !JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return;
                }
                break;

            case FsmAction.UnInit:
                if (IsActive)
                    IsShowingLyChallengeUnlocked = false;
                break;
        }
    }

    private void Fsm_DownloadMap(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Data.StatusText.Text = "   %";

                Data.ReusableTexts[0].Color = TextColor.GameCubeMenu;
                Data.ReusableTexts[1].Color = TextColor.GameCubeMenu;

                string[] text = Localization.GetText(11, 7);
                Data.ReusableTexts[0].Text = text[0];
                Data.ReusableTexts[0].ScreenPos = new Vector2(140 - Data.ReusableTexts[0].GetStringWidth() / 2f, 40);

                Data.ReusableTexts[1].Text = MapInfos.Maps[SelectedMap].Name;
                Data.ReusableTexts[1].ScreenPos = new Vector2(140 - Data.ReusableTexts[1].GetStringWidth() / 2f, 60);

                Data.StatusText.ScreenPos = new Vector2(122, 80);
                break;

            case FsmAction.Step:
                // Lost connection
                if (JoyBus.CheckForLostConnection())
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return;
                }

                // Calculate download percentage
                int percentage = (JoyBus.Size - JoyBus.RemainingSize) * 100 / JoyBus.Size;
                string percentageString = $"{percentage}%";
                percentageString = percentageString.PadLeft(4);

                Data.StatusText.Text = percentageString;

                // Stop download
                if (JoyPad.CheckSingle(GbaInput.B))
                {
                    JoyBus.Disconnect();
                    JoyBus.Connect();
                    State.MoveTo(Fsm_WaitForConnection);
                    return;
                }

                // Error
                if (JoyBus.ErrorState == 0xFF)
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return;
                }

                // Finished downloading map
                if (JoyBus.RemainingSize == 0)
                {
                    State.MoveTo(Fsm_DownloadMapAck);
                    return;
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return;
                }
                break;

            case FsmAction.UnInit:
                if (IsActive)
                {
                    JoyBus.SendValue(JoyBus.ReceivedData);
                    JoyBus.HasReceivedData = false;
                    JoyBus.Checksum = 0;
                }
                break;
        }
    }

    private void Fsm_DownloadMapAck(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Data.StatusText.Text = "100%";
                break;

            case FsmAction.Step:
                if (JoyBus.HasReceivedData)
                {
                    FrameManager.SetNextFrame(new FrameSideScrollerGCN(MapInfos.Maps[SelectedMap], Map, SelectedMap));

                    if (JoyBus.ReceivedData == 0x22222222)
                    {
                        State.MoveTo(Fsm_WaitForConnection);
                        return;
                    }
                    else
                    {
                        State.MoveTo(Fsm_ConnectionLost);
                        return;
                    }
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
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

        Gfx.Fade = 1;
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
        else if (State.EqualsState(Fsm_DownloadMap))
        {
            AnimationPlayer.Play(Data.ReusableTexts[0]);
            AnimationPlayer.Play(Data.ReusableTexts[1]);
        }
        else if (State.EqualsState(Fsm_SelectMap))
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

        if (WaitingForConnection || State.EqualsState(Fsm_DownloadMap) || State.EqualsState(Fsm_SelectMap) || State.EqualsState(Fsm_DownloadMapAck))
            AnimationPlayer.Play(Data.StatusText);

        TransitionsFX.StepAll();
        AnimationPlayer.Execute();
    }

    #endregion

    #region Enums

    private enum DownloadMode
    {
        JoyBus,
        FileSystem, // TODO: Implement reading from file system
    }

    #endregion
}