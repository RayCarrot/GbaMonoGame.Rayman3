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
        else if (Frame.GetComponent<TgxPlayfield2D>() is { } playfield2D)
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
        if (JoyPad.GetMouseState().RightButton == ButtonState.Pressed && Frame.GetComponent<TgxPlayfield2D>() is { } playfield2D)
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
            { ActorType.Rayman, (id, resource) => new Rayman(id, resource) },

            { ActorType.Piranha, (id, resource) => new Piranha(id, resource) },
            { ActorType.Splash, (id, resource) => new Splash(id, resource) },

            { ActorType.Cage, (id, resource) => new Cage(id, resource) },

            { ActorType.Butterfly, (id, resource) => new Butterfly(id, resource) },
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
            TgxPlayfield2D playfield = Frame.GetComponent<TgxPlayfield2D>();

            if (playfield != null)
                playfield.PhysicalLayer.DebugScreen.IsEnabled = !playfield.PhysicalLayer.DebugScreen.IsEnabled;
        }

        UpdateGameZoom(gameTime);
        UpdateGameScroll();
    }

    #endregion
}