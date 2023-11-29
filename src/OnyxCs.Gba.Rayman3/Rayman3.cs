using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;
using Game = BinarySerializer.Onyx.Gba.Game;

namespace OnyxCs.Gba.Rayman3;

public class Rayman3 : GbaGame
{
    #region Protected Properties

    protected override Game Game => Game.Rayman3;
    protected override int SoundBankResourceId => 65;
    protected override Dictionary<int, string> SongTable => new()
    {
        [6] = "sfx/OnoExpir",
        [8] = "sfx/OnoInspi",
        [9] = "sfx/OnoJump1",
        [14] = "sfx/OnoThrow_Mix02",
        [16] = "sfx/OnoWin_Mix02",
        [18] = "sfx/OnoWinRM_Mix02",
        [27] = "sfx/SkiLoop1",
        [61] = "sfx/MenuMove",
        [62] = "sfx/MinHP",
        [80] = "sfx/RireMumu_Mix03",
        [89] = "music/canopy",
        [90] = "music/baddreams",
        [91] = "music/precipice",
        [92] = "music/lyrace",
        [98] = "music/jano",
        [99] = "music/mountain1",
        [100] = "music/polokus",
        [101] = "music/rockchase",
        [102] = "music/spiderchase",
        [103] = "music/bigplatform",
        [104] = "music/bigtrees",
        [105] = "music/boss1",
        [106] = "music/boss34",
        [107] = "music/fairyglades",
        [108] = "music/helico",
        [109] = "music/rocket",
        [110] = "music/waterski",
        [111] = "music/woodlight",
        [114] = "sfx/Bounce01_Mix02",
        [115] = "sfx/FishJaw1_Mix02",
        [116] = "sfx/CageSnd1_Mix02",
        [118] = "sfx/RootIn_Mix01",
        [134] = "music/barrel",
        [135] = "music/echocave",
        [136] = "music/mountain2",
        [139] = "sfx/SlideOut_Mix01",
        [141] = "sfx/SldGreen_SkiLoop1",
        [147] = "music/ancients",
        [148] = "music/finalboss",
        [149] = "music/firestone",
        [150] = "music/happyslide",
        [151] = "music/lyfree",
        [152] = "music/raytheme",
        [153] = "music/ship",
        [154] = "music/tag",
        [161] = "sfx/GumBlow_Mix01",
        [162] = "sfx/CageSnd2_Mix02",
        [171] = "sfx/Switch1_Mix03",
        [177] = "music/enemy1",
        [178] = "music/enemy2",
        [179] = "music/sadslide",
        [180] = "music/barbeslide",
        [181] = "music/tizetre",
        [182] = "music/win1",
        [183] = "music/win2",
        [184] = "music/win3",
        [185] = "music/death",
        [186] = "music/lyfreeVOX4",
        [199] = "sfx/SlideIn_Mix02",
        [205] = "sfx/MetlGate_Mix01",
        [214] = "sfx/Combust1_Mix02",
        [219] = "sfx/BarlLeaf_SkiWeed_Mix02",
        [222] = "sfx/WallSlid_Mix02",
        [224] = "sfx/Grenad01_Mix03",
        [226] = "sfx/VibraFLW_Mix02",
        [249] = "sfx/WWallBrk_PF2Fall_Mix03",
        [253] = "sfx/OnoJump3_Mix01",
        [254] = "sfx/OnoJump4_Mix01",
        [255] = "sfx/OnoJump5_Mix01",
        [256] = "sfx/OnoJump6_Mix01",
        [257] = "sfx/RayFist_Mix02",
        [261] = "sfx/SuprFist_Mix01",
        [262] = "sfx/LumMauve_Mix02",
        [268] = "sfx/HeliCut_Mix01",
        [269] = "sfx/Valid01_Mix01",
        [270] = "sfx/Back01_Mix01",
        [275] = "sfx/Helico01_Mix10",
        [280] = "sfx/Laser1_Mix02",
        [281] = "sfx/Laser2_Mix02",
        [285] = "sfx/OnoPeur1_Mix03",
        [286] = "sfx/BodyAtk1_Mix01",
        [294] = "sfx/OnoGO_Mix02",
        [298] = "sfx/BangGen1_Mix07",
        [300] = "sfx/BigSplsh_SplshGen_Mix04",
        [302] = "sfx/OnoEfor1_Mix02",
        [304] = "sfx/Boing_Mix02",
        [318] = "music/LumBoost_Mix01GEN_P1",
        [319] = "music/LumBoost_Mix01GEN_P2",
        [320] = "music/LumBoost_Mix01GEN_P3",
        [335] = "sfx/Grimace1_Mix04",
        [336] = "sfx/OnoWoHoo_Mix01",
        [337] = "music/barrel_BA",
        [338] = "sfx/MachAtk1_Mix01",
        [341] = "sfx/FishDead_Mix02",
        [342] = "music/rocket_BA",
        [344] = "sfx/SkulShak_Mix01",
        [346] = "sfx/HandTap2_Mix03",
        [351] = "sfx/HorseCry_Mix02",
        [352] = "sfx/BigFoot1_Mix02",
        [358] = "sfx/YoyoMove_Mix02",
        [361] = "sfx/CagoDie2_Mix01",
        [365] = "sfx/BallInit_Mix01",
        [366] = "sfx/MachMotr_Mix01",
        [367] = "sfx/BombFly_Mix03",
        [370] = "sfx/Charge2_Mix04",
        [375] = "sfx/RootOut_Mix04",
        [376] = "sfx/PF2Crac_Mix02",
        [380] = "music/RootOut_Pitch",
        [381] = "sfx/LumGreen_Mix04",
        [382] = "sfx/NoCombus_Mix04",
        [386] = "sfx/Bounce00_Mix03",
        [388] = "sfx/PinBall_Mix02",
        [389] = "music/PinBall_Low",
        [394] = "sfx/RaySpin_Mix06",
        [395] = "sfx/JanoShot_Mix01",
        [397] = "sfx/ScalDead_Mix02",
        [398] = "sfx/CaCombus_NoCombus_Mix04",
        [399] = "sfx/MachAtk2_Mix02",
        [400] = "sfx/LumSwing_Mix03",
        [401] = "sfx/BallSwng_LumSwing_Mix03",
        [402] = "sfx/GenWoosh_LumSwing_Mix03",
        [403] = "sfx/PiraJump_BigFoot1_Mix02",
        [404] = "sfx/BallImp1_BigFoot1_Mix02",
        [405] = "sfx/BarlFall_Mix04",
        [406] = "sfx/PiraHit1_Mix02",
        [407] = "sfx/PiraHit3_Mix03",
        [408] = "sfx/CageHit_Mix07",
        [409] = "sfx/GumChew_Mix02",
        [410] = "sfx/HeliStop_Mix06",
        [411] = "sfx/Horror_Mix08",
        [412] = "sfx/LumBleu_Mix02",
        [413] = "sfx/LumSlvr_Mix02",
        [414] = "sfx/MumuHit_SkullHit_Mix02",
        [417] = "sfx/MumuWake_LumHit_Mix03",
        [419] = "sfx/CaFlyDie_Mix03",
        [420] = "sfx/CagouRit_Mix03",
        [421] = "sfx/MetlImp1_PiraHit3_Mix03",
        [423] = "sfx/Thunder1_Mix04",
        [426] = "sfx/LumTimer_Mix02",
        [427] = "sfx/BBQ_Mix10",
        [428] = "sfx/Tag_Mix02",
        [431] = "sfx/Motor01_Mix12",
        [432] = "sfx/Laser4_Mix01",
        [433] = "sfx/MetlTrap_MetlGate_Mix01",
        [436] = "sfx/LumRed_Mix03",
        [438] = "sfx/LumOrag_Mix06",
        [439] = "sfx/GlueFoot_PlumSnd2_Mix02",
        [440] = "sfx/Cloche01_Mix01",
        [444] = "sfx/Janogrrr_Mix03",
        [447] = "sfx/CagoWlk1_Mix02",
        [449] = "sfx/Fire02_Laser4_Mix01",
        [451] = "sfx/SkulInit_Mix04",
        [452] = "sfx/SkullEnd_Mix02",
        [453] = "sfx/SkullHit_Mix02",
        [454] = "sfx/Chain_Mix01",
        [455] = "sfx/CageTrsh_Mix05",
        [458] = "sfx/Store01_Mix01",
        [460] = "sfx/Pannel_BigFoot1_Mix02",
        [461] = "sfx/CagOno01_Mix01",
        [462] = "sfx/RocktLeg_Mix03",
        [463] = "sfx/Bounce02_Mix03",
        [464] = "sfx/PF2Fall_Mix03",
        [467] = "sfx/JanoSkul_Mix03",
        [468] = "sfx/LavaBubl_Mix02",
        [469] = "sfx/LavaStrt_Mix04",
        [470] = "sfx/LumAtk01_Mix02",
        [471] = "sfx/LumAtk02_Mix01",
        [472] = "sfx/LumTotal_Mix02",
        [473] = "sfx/MumuDead_Mix04",
        [475] = "sfx/OnoEfor2_Mix03",
        [476] = "sfx/OnoEquil_Mix03",
        [477] = "sfx/PF1Fall_PF2Fall_Mix03",
        [478] = "sfx/PlumSnd1_Mix03",
        [480] = "sfx/RaDeath_Mix03",
        [481] = "sfx/SkiWeed_Mix02",
        [482] = "sfx/SpherImp_Mix02",
        [483] = "sfx/SpidrAtk_Mix02",
        [484] = "sfx/SplshGen_Mix04",
        [488] = "sfx/Electric_Mix02",
        [494] = "sfx/SocleFX1_Mix01",
        [495] = "sfx/LyVO1_Mix01",
        [496] = "sfx/CagouHit_Mix03",
        [497] = "sfx/ArmSwing_LumSwing_Mix03",
        [498] = "sfx/BlobFX02_Mix02",
        [500] = "sfx/ScaMorf1_Mix02",
        [501] = "sfx/ScaMorf2_Mix02",
        [502] = "sfx/ScaHurt2_Mix02",
        [504] = "sfx/BossVO01_Mix01",
        [505] = "sfx/BossVO02_Mix01",
        [506] = "sfx/Appear_SocleFX1_Mix01",
        [507] = "sfx/MurfHeli_Mix01",
        [508] = "sfx/Curtain_YoyoMove_Mix02",
        [509] = "sfx/ScalBong_PinBall_Mix02",
        [511] = "sfx/LumVioNP_SkulShak_Mix01",
        [512] = "sfx/PiraHurt_Mix02",
        [515] = "sfx/PiraAtk1_Mix01",
        [519] = "sfx/LineFX01_Mix02_P1_",
        [520] = "sfx/LineFX01_Mix02_P2_",
        [523] = "sfx/Charge_Mix05",
        [524] = "sfx/JanoGrwl_Mix03",
        [525] = "sfx/PlumSnd2_Mix03",
        [526] = "sfx/WoodImp_Mix03",
        [527] = "sfx/WoodBrk1_Mix04",
        [528] = "sfx/RireCago_RireMumu_Mix03",
        [529] = "sfx/Store02_Mix02",
        [530] = "sfx/ScalFlee_Mix02",
        [534] = "sfx/BossVO03_Mix01",
        [537] = "sfx/CagoTurn_Mix03",
        [538] = "sfx/RyVOGlob_Mix02",
        [540] = "sfx/GateClos_MetlGate_Mix01",
        [541] = "sfx/LyMagic1_Mix01",
        [543] = "sfx/Sparkles_Mix01",
        [544] = "sfx/MurfyVO1A_Mix01",
        [545] = "sfx/MurfyVO1B_Mix01",
        [548] = "sfx/MurfyVO3A_Mix01",
        [550] = "sfx/MurfyVO4A_Mix01",
        [557] = "sfx/GhstDead_Mix05",
        [558] = "sfx/OnoRcvH1_Mix04",
        [559] = "sfx/PiraDead_Mix05",
        [560] = "music/tizetre_Swing",
        [561] = "sfx/RayFist2_Mix01",
        [563] = "sfx/CagoAttk_Mix03",
        [564] = "sfx/RoktSpin_Mix03",
        [565] = "sfx/Whistle1_Mix01",
        [566] = "sfx/TiztrVO1_Mix01",
        [567] = "sfx/TiztrVO2_Mix01",
        [568] = "sfx/TiztrVO3_Mix01",
        [569] = "sfx/TiztrVO4_Mix01",
        [571] = "sfx/LightFX1_Mix01",
        [578] = "sfx/Missile1_Mix01",
        [579] = "sfx/PannelUp_Mix01",
        [580] = "sfx/PannelDw_Mix01",
        [581] = "music/Win_BOSS",
        [582] = "sfx/ScalUp_Mix03",
        [584] = "sfx/GameOver_BeepFX01_Mix02",
        [585] = "sfx/LyMagic2_Mix07",
        [586] = "sfx/JanoRire_Mix01",
        [587] = "sfx/ScalHurt_Mix02",
        [588] = "sfx/LumAtk03_LumAtk01_Mix02",
        [589] = "sfx/LumAtk04_LumAtk02_Mix01",
        [590] = "sfx/CountDwn_Mix07_P1_",
        [591] = "sfx/CountDwn_Mix07_P2_",
        [592] = "sfx/CountDwn_Mix07_P3_",
        [593] = "sfx/Globox_Mix04",
        [594] = "sfx/PathFX_Mix01",
        [595] = "sfx/BeepFX01_Mix02",
        [596] = "sfx/Spirale_Mix01",
        [597] = "sfx/RyVOLy_Mix01",
        [598] = "sfx/RyVO1_Mix01",
        [599] = "sfx/RyVO2_Mix01",
        [600] = "sfx/RyVO3_Mix01",
        [602] = "sfx/BossHurt_Mix02",
        [603] = "sfx/Spark2_Sparkles_Mix01",
        [604] = "sfx/Spark3_Sparkles_Mix01",
        [605] = "sfx/LumPeek_Mix01",
        [607] = "sfx/Spark4_Sparkles_Mix01",
        [608] = "sfx/RaySigh_Mix01",
        [609] = "sfx/SnoreFX1_Mix01",
        [610] = "sfx/SnoreFX2_SnoreFX1_Mix01",
        [611] = "sfx/ScalGrrr_Mix02",
        [613] = "sfx/Laser3_Mix03",
        [614] = "sfx/HandTap1_Mix04",
        [615] = "sfx/NewPower_Mix06",
        [616] = "sfx/Curtain2_YoyoMove_Mix02",
        [618] = "sfx/Propulse_Combust1_Mix02",
    };

    #endregion

    #region Private Methods

    private void SetGameZoom(float zoom)
    {
        Engine.ScreenCamera.ResizeGame(new Point(
            (int)Math.Round(Engine.ScreenCamera.OriginalGameResolution.X * zoom), 
            (int)Math.Round(Engine.ScreenCamera.OriginalGameResolution.Y * zoom)));
        //Gfx.GfxCamera.ResizeScreen(Window.ClientBounds.Size);
    }

    private void UpdateGameZoom(Microsoft.Xna.Framework.GameTime gameTime)
    {
        MouseState mouse = JoyPad.GetMouseState();

        if (mouse.MiddleButton == ButtonState.Pressed)
        {
            SetGameZoom(1);
        }
        else if (Frame.Current is IHasPlayfield { Playfield: TgxPlayfield2D playfield2D })
        {
            int mouseWheelDelta = JoyPad.GetMouseWheelDelta();

            if (mouseWheelDelta != 0)
            {
                float deltaFloat = mouseWheelDelta * (float)gameTime.ElapsedGameTime.TotalSeconds;
                const float zoomSpeed = 0.03f;

                TgxCluster mainCluster = playfield2D.Camera.GetMainCluster();

                // TODO: Modify position if max zoom so that we can zoom more?
                // TODO: Auto-correct zoom when playfield changes?
                float maxZoom = Math.Min(
                    (mainCluster.Size.X - mainCluster.Position.X) / Engine.ScreenCamera.OriginalGameResolution.X, 
                    (mainCluster.Size.Y - mainCluster.Position.Y) / Engine.ScreenCamera.OriginalGameResolution.Y);

                float zoom = Engine.ScreenCamera.GameResolution.X / (float)Engine.ScreenCamera.OriginalGameResolution.X;
                zoom = Math.Clamp(zoom + zoomSpeed * deltaFloat * -1, 0.2f, maxZoom);

                SetGameZoom(zoom);
            }
        }
    }

    private void UpdateGameScroll()
    {
        if (JoyPad.GetMouseState().RightButton == ButtonState.Pressed && Frame.Current is IHasPlayfield { Playfield: TgxPlayfield2D playfield2D })
            playfield2D.Camera.Position += JoyPad.GetMousePositionDelta() * -1;
    }

    #endregion

    #region Protected Methods

    protected override Frame CreateInitialFrame() => new Intro(); // TODO: N-Gage should start with language selection

    protected override void Initialize()
    {
        base.Initialize();

        ObjectFactory.Init(new Dictionary<ActorType, ObjectFactory.CreateActor>()
        {
            { ActorType.Rayman, (id, scene, resource) => new Rayman(id, scene, resource) },

            { ActorType.Piranha, (id, scene, resource) => new Piranha(id, scene, resource) },
            { ActorType.Splash, (id, scene, resource) => new Splash(id, scene, resource) },

            { ActorType.Cage, (id, scene, resource) => new Cage(id, scene, resource) },

            { ActorType.Butterfly, (id, scene, resource) => new Butterfly(id, scene, resource) },
        }, x => ((ActorType)x).ToString());
        LevelFactory.Init(new Dictionary<MapId, LevelFactory.CreateLevel>()
        {
            { MapId.WoodLight_M1, id => new WoodLight_M1(id) },
        });
    }

    protected override void AddDebugWindowsAndMenus(DebugLayout debugLayout)
    {
        debugLayout.AddWindow(new SceneDebugWindow());
        debugLayout.AddWindow(new GameObjectDebugWindow());
        debugLayout.AddWindow(new PlayfieldDebugWindow());
        debugLayout.AddMenu(new FramesDebugMenu());
    }

    protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        base.Update(gameTime);

        // Toggle showing debug collision screen
        if (JoyPad.CheckSingle(Keys.T))
        {
            if (Frame.Current is IHasPlayfield { Playfield: { } playfield })
                playfield.PhysicalLayer.DebugScreen.IsEnabled = !playfield.PhysicalLayer.DebugScreen.IsEnabled;
        }

        UpdateGameZoom(gameTime);
        UpdateGameScroll();
    }

    #endregion
}