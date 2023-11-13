using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class Rayman3 : Game
{
    #region Constructor

    public Rayman3()
    {
        _graphics = new GraphicsDeviceManager(this);
        _debugLayout = new DebugLayout();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += Window_ClientSizeChanged;

        // Force frame-rate to 60
        SetFramerate(60);
    }

    #endregion

    #region Private Fields

    private readonly GraphicsDeviceManager _graphics;
    private readonly DebugLayout _debugLayout;
    private SpriteBatch _spriteBatch;
    private GfxRenderer _gfxRenderer;
    private GameConfig _config;
    private Context _context;
    private RomLoader _romLoader;
    private GameRenderTarget _debugGameRenderTarget;
    private bool _runOneFrame;

    #endregion

    #region Event Handlers

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        if (!_config.Debug)
            SizeGameToWindow();
    }

    #endregion

    #region Private Methods

    private void SetFramerate(float fps)
    {
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1 / fps);
    }

    private void LoadConfig()
    {
        const string fileName = "Rayman3.json";

        if (File.Exists(fileName))
            _config = JsonSerializer.Deserialize<GameConfig>(File.ReadAllText(fileName));
        else
            _config = new GameConfig();
    }

    private void SetWindowSize(int width, int height)
    {
        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();
    }

    private void SizeGameToWindow()
    {
        Gfx.GfxCamera.ResizeScreen(Window.ClientBounds.Size, JoyPad.Check(Keys.LeftShift), changeScreenSizeCallback: newSize =>
        {
            _graphics.PreferredBackBufferWidth = newSize.X;
            _graphics.PreferredBackBufferHeight = newSize.Y;
            _graphics.ApplyChanges();
        });
    }

    private void LoadRom()
    {
        string romFilePath = Path.GetFullPath(_config.RomFile);
        string romDir = Path.GetDirectoryName(romFilePath);

        ISerializerLogger serializerLogger = _config.SerializerLogFile != null
            ? new FileSerializerLogger(_config.SerializerLogFile)
            : null;

        // TODO: Add a logger
        _context = new Context(romDir!, serializerLogger: serializerLogger);
        _romLoader = new RomLoader(_context, romFilePath);
        _romLoader.Load();
    }

    private void LoadEngine()
    {
        Gfx.GraphicsDevice = GraphicsDevice;
        Gfx.ContentManager = Content;
        Gfx.GfxCamera = new GfxCamera(Window.ClientBounds.Size);
    }

    private void LoadGame()
    {
        Storage.SetContext(_context);
        GameInfo.Levels = _romLoader.LevelInfo;

        ObjectFactory.Init(new Dictionary<ActorType, ObjectFactory.CreateActor>()
        {
            { ActorType.Rayman, (id, resource) => new Rayman(id, resource) },

            { ActorType.Piranha, (id, resource) => new Piranha(id, resource) },
            { ActorType.Splash, (id, resource) => new Splash(id, resource) },

            { ActorType.Cage, (id, resource) => new Cage(id, resource) },

            { ActorType.Butterfly, (id, resource) => new Butterfly(id, resource) },
        });
        LevelFactory.Init(new Dictionary<MapId, LevelFactory.CreateLevel>()
        {
            { MapId.WoodLight_M1, id => new WoodLight_M1(id) },
        });

        FrameManager.SetNextFrame(new Intro());
    }

    private void SetGameZoom(float zoom)
    {
        Gfx.GfxCamera.ResizeGame(new Point(
            (int)Math.Round(Gfx.GfxCamera.OriginalGameResolution.X * zoom), 
            (int)Math.Round(Gfx.GfxCamera.OriginalGameResolution.Y * zoom)));
        //Gfx.GfxCamera.ResizeScreen(Window.ClientBounds.Size);
    }

    private void StepEngine()
    {
        if (_config.Paused)
            return;

        try
        {
            FrameManager.Step();
        }
        catch
        {
            _context.Dispose();
            throw;
        }
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
                    (mainCluster.Size.X - mainCluster.Position.X) / Gfx.GfxCamera.OriginalGameResolution.X, 
                    (mainCluster.Size.Y - mainCluster.Position.Y) / Gfx.GfxCamera.OriginalGameResolution.Y);

                float zoom = Gfx.GfxCamera.GameResolution.X / (float)Gfx.GfxCamera.OriginalGameResolution.X;
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

    protected override void Initialize()
    {
        LoadConfig();
        SetWindowSize(Constants.ScreenWidth * 4, Constants.ScreenHeight * 4); // TODO: Save window size in config, as well as if maximized etc.
        LoadRom();
        LoadEngine();
        LoadGame();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _gfxRenderer = new GfxRenderer(_spriteBatch, Gfx.GfxCamera);
        _debugGameRenderTarget = new GameRenderTarget(GraphicsDevice, Gfx.GfxCamera);
        _debugLayout.LoadContent(_debugGameRenderTarget, this, _config);
    }

    protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        if (_runOneFrame)
        {
            _runOneFrame = false;
            _config.Paused = true;
        }

        // Toggle debug mode
        if (JoyPad.CheckSingle(Keys.Escape))
        {
            _config.Debug = !_config.Debug;

            // Refresh sizes
            if (_config.Debug)
                _debugLayout.EnableDebugMode();
            else
                SizeGameToWindow();
        }

        // Toggle pause
        if (JoyPad.Check(Keys.LeftControl) && JoyPad.CheckSingle(Keys.P))
        {
            _config.Paused = !_config.Paused;
        }

        // Run one frame
        if (JoyPad.Check(Keys.LeftControl) && JoyPad.CheckSingle(Keys.F))
        {
            _config.Paused = false;
            _runOneFrame = true;
        }

        // Toggle showing debug collision screen
        if (JoyPad.CheckSingle(Keys.T))
        {
            TgxPlayfield2D playfield = Frame.GetComponent<TgxPlayfield2D>();
            
            if (playfield != null)
                playfield.PhysicalLayer.DebugScreen.IsEnabled = !playfield.PhysicalLayer.DebugScreen.IsEnabled;
        }

        JoyPad.Scan();
        UpdateGameZoom(gameTime);
        UpdateGameScroll();
        StepEngine();

        base.Update(gameTime);
    }

    protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
    {
        if (_config.Debug)
            _debugGameRenderTarget.BeginRender();

        // Clear screen
        GraphicsDevice.Clear(Color.Black);

        // Draw screen
        _gfxRenderer.Begin();
        Gfx.Draw(_gfxRenderer);
        if (_config.Debug)
            _debugLayout.DrawGame(_gfxRenderer);
        _gfxRenderer.End();

        if (_config.Debug)
        {
            _debugGameRenderTarget.EndRender();

            // Draw debug layout
            _debugLayout.Draw(gameTime);
        }

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            _context?.Dispose();
    }

    #endregion
}