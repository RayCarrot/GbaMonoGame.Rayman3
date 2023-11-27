using System;
using System.Linq;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game = BinarySerializer.Onyx.Gba.Game;

namespace OnyxCs.Gba;

public abstract class GbaGame : Microsoft.Xna.Framework.Game
{
    #region Constructor

    protected GbaGame(Game game)
    {
        _game = game;
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

    private readonly Game _game;
    private readonly GraphicsDeviceManager _graphics;
    private readonly DebugLayout _debugLayout;

    private SpriteBatch _spriteBatch;
    private GfxRenderer _gfxRenderer;
    private GameRenderTarget _debugGameRenderTarget;

    #endregion

    #region Public Properties

    public bool RunSingleFrame { get; set; }
    public bool IsPaused { get; set; }
    public bool DebugMode { get; set; }

    #endregion

    #region Event Handlers

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        if (!DebugMode)
            SizeGameToWindow();
    }

    #endregion

    #region Private Methods

    private void SetFramerate(float fps)
    {
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1 / fps);
    }

    private void SetWindowSize(Point size)
    {
        _graphics.PreferredBackBufferWidth = size.X;
        _graphics.PreferredBackBufferHeight = size.Y;
        _graphics.ApplyChanges();
    }

    private void SizeGameToWindow()
    {
        Engine.ScreenCamera.ResizeScreen(Window.ClientBounds.Size, JoyPad.Check(Keys.LeftShift), changeScreenSizeCallback: SetWindowSize);
    }

    private void StepEngine()
    {
        if (IsPaused)
            return;

        try
        {
            FrameManager.Step();
        }
        catch
        {
            Engine.Unload();
            throw;
        }
    }

    #endregion

    #region Protected Methods

    protected abstract Frame CreateInitialFrame();
    protected virtual void AddDebugWindowsAndMenus(DebugLayout debugLayout) { }

    protected override void Initialize()
    {
        Engine.LoadConfig(Environment.GetCommandLineArgs().FirstOrDefault(x => x.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase)));
        Engine.LoadRom(Engine.Config.RomFile, Engine.Config.SerializerLogFile, _game);

        Engine.LoadMonoGame(GraphicsDevice, Content, new ScreenCamera());

        // TODO: Save window size in config, as well as if maximized etc.
        Point windowSize = new(1920, 1080);
        Engine.ScreenCamera.ResizeScreen(windowSize, maintainScreenRatio: true, changeScreenSizeCallback: SetWindowSize);

        FrameManager.SetNextFrame(CreateInitialFrame());

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _gfxRenderer = new GfxRenderer(_spriteBatch, Engine.ScreenCamera);
        _debugGameRenderTarget = new GameRenderTarget(GraphicsDevice, Engine.ScreenCamera);

        _debugLayout.AddWindow(new GameDebugWindow(_debugGameRenderTarget));
        _debugLayout.AddWindow(new LoggerDebugWindow());
        _debugLayout.AddWindow(new GfxDebugWindow());
        _debugLayout.AddMenu(new WindowsDebugMenu());
        AddDebugWindowsAndMenus(_debugLayout);

        _debugLayout.LoadContent(this);

        base.LoadContent();
    }

    protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        base.Update(gameTime);

        JoyPad.Scan();

        if (RunSingleFrame)
        {
            RunSingleFrame = false;
            IsPaused = true;
        }

        // Toggle debug mode
        if (JoyPad.CheckSingle(Keys.Escape))
        {
            DebugMode = !DebugMode;

            // Refresh sizes
            if (DebugMode)
                _debugLayout.GetWindow<GameDebugWindow>()?.RefreshSize();
            else
                SizeGameToWindow();
        }

        // Toggle pause
        if (JoyPad.Check(Keys.LeftControl) && JoyPad.CheckSingle(Keys.P))
        {
            IsPaused = !IsPaused;
        }

        // Run one frame
        if (JoyPad.Check(Keys.LeftControl) && JoyPad.CheckSingle(Keys.F))
        {
            IsPaused = false;
            RunSingleFrame = true;
        }

        StepEngine();
    }

    protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
    {
        if (DebugMode)
            _debugGameRenderTarget.BeginRender();

        // Clear screen
        GraphicsDevice.Clear(Color.Black);

        // Draw screen
        _gfxRenderer.Begin();
        Gfx.Draw(_gfxRenderer);
        if (DebugMode)
            _debugLayout.DrawGame(_gfxRenderer);
        _gfxRenderer.End();

        if (DebugMode)
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
            Engine.Unload();
    }

    #endregion
}