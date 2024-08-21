﻿using System.IO;
using BinarySerializer;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public partial class GameCubeMenu
{
    private bool FsmStep_CheckConnection()
    {
        if (UseJoyBus && JoyBus.CheckForLostConnection())
        {
            State.MoveTo(Fsm_ConnectionLost);
            return false;
        }

        return true;
    }

    private bool Fsm_PreInit(FsmAction action)
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
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_WaitForConnection(FsmAction action)
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
                    return false;
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
                if (JoyPad.IsButtonJustPressed(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return false;
                }

                // Connected
                if (JoyBus.IsConnected)
                {
                    WaitingForConnection = false;
                    State.MoveTo(Fsm_Connected);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_ConnectionLost(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Data.StatusText.Text = "";
                // Why does the game do this??
                MapInfoFileSize = (int)GameTime.ElapsedFrames;
                break;

            case FsmAction.Step:
                if (JoyPad.IsButtonJustPressed(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return false;
                }
                else
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_Exit(FsmAction action)
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
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_Connected(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ResetReusableTexts();
                Data.StatusText.ScreenPos = new Vector2(105, 88);
                Data.StatusText.Text = "";
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckConnection())
                    return false;

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
                if (JoyPad.IsButtonJustPressed(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return false;
                }

                // Error
                if (JoyBus.HasReceivedData && JoyBus.IsConnected && !isValid)
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return false;
                }

                // Received data - download the map info
                if (JoyBus.HasReceivedData && JoyBus.IsConnected)
                {
                    GbaUnlockFlags = gbaUnlockFlags;
                    State.MoveTo(Fsm_DownloadMapInfo);
                    return false;
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return false;
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

        return true;
    }

    private bool Fsm_DownloadMapInfo(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckConnection())
                    return false;

                // Exit
                if (JoyPad.IsButtonJustPressed(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return false;
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return false;
                }

                // Error
                if (JoyBus.ErrorState == 0xFF)
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return false;
                }

                // Finished downloading map info
                if (JoyBus.RemainingSize == 0)
                {
                    State.MoveTo(Fsm_DownloadMapInfoAck);
                    return false;
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

        return true;
    }

    private bool Fsm_DownloadMapInfoAck(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckConnection())
                    return false;

                // Exit
                if (JoyPad.IsButtonJustPressed(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return false;
                }

                // Select map - Received download acknowledgement
                if (JoyBus.HasReceivedData && JoyBus.ReceivedData == 0x22222222)
                {
                    State.MoveTo(Fsm_SelectMap);
                    return false;
                }

                // Error - received invalid data
                if (JoyBus.HasReceivedData)
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return false;
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_SelectMap(FsmAction action)
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
                if (!FsmStep_CheckConnection())
                    return false;

                bool hasSelectedMap = false;

                if (IsShowingLyChallengeUnlocked)
                {
                    if (JoyPad.IsButtonJustPressed(GbaInput.Start) || JoyPad.IsButtonJustPressed(GbaInput.A))
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
                    if (JoyPad.IsButtonJustPressed(GbaInput.Start) || JoyPad.IsButtonJustPressed(GbaInput.A))
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
                    if (JoyPad.IsButtonJustPressed(GbaInput.Up) && SelectedMap != 0)
                    {
                        SelectedMap--;
                        if (MapScroll > SelectedMap)
                            MapScroll = SelectedMap;

                        MapSelectionUpdateText();
                        JoyBus.SendValue(2);
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MenuMove);
                    }

                    // Move down
                    if (JoyPad.IsButtonJustPressed(GbaInput.Down) && SelectedMap < MapInfos.MapsCount - 1)
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
                if (JoyPad.IsButtonJustPressed(GbaInput.B))
                {
                    IsActive = false;
                    State.MoveTo(Fsm_Exit);
                    return false;
                }

                if (hasSelectedMap)
                {
                    State.MoveTo(Fsm_DownloadMap);
                    return false;
                }

                // Disconnected
                if (UseJoyBus && !JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                if (IsActive)
                    IsShowingLyChallengeUnlocked = false;
                break;
        }

        return true;
    }

    private bool Fsm_DownloadMap(FsmAction action)
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
                if (!FsmStep_CheckConnection())
                    return false;

                // Calculate download percentage
                int percentage = (JoyBus.Size - JoyBus.RemainingSize) * 100 / JoyBus.Size;
                string percentageString = $"{percentage}%";
                percentageString = percentageString.PadLeft(4);

                Data.StatusText.Text = percentageString;

                // Stop download
                if (JoyPad.IsButtonJustPressed(GbaInput.B))
                {
                    JoyBus.Disconnect();
                    JoyBus.Connect();
                    State.MoveTo(Fsm_WaitForConnection);
                    return false;
                }

                // Error
                if (JoyBus.ErrorState == 0xFF)
                {
                    State.MoveTo(Fsm_ConnectionLost);
                    return false;
                }

                // Finished downloading map
                if (JoyBus.RemainingSize == 0)
                {
                    State.MoveTo(Fsm_DownloadMapAck);
                    return false;
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return false;
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

        return true;
    }

    private bool Fsm_DownloadMapAck(FsmAction action)
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
                        return false;
                    }
                    else
                    {
                        State.MoveTo(Fsm_ConnectionLost);
                        return false;
                    }
                }

                // Disconnected
                if (!JoyBus.IsConnected)
                {
                    ShowPleaseConnectText();
                    State.MoveTo(Fsm_WaitForConnection);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }
}