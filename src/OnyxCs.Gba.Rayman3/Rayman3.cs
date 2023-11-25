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
    #region Constructor

    public Rayman3() : base(Game.Rayman3) { }

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

    protected override Frame CreateInitialFrame() => new Intro();

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