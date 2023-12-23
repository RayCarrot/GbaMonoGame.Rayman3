﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game = BinarySerializer.Onyx.Gba.Game;

namespace OnyxCs.Gba;

public abstract class GbaGame : Microsoft.Xna.Framework.Game
{
    #region Constructor

    protected GbaGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _debugLayout = new DebugLayout();
        _updateTimeStopWatch = new Stopwatch();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += Window_ClientSizeChanged;

        // Force frame-rate to 60. The GBA framerate is actually 59.727500569606, but we do 60.
        SetFramerate(60);
    }

    #endregion

    #region Private Fields

    private readonly GraphicsDeviceManager _graphics;
    private readonly DebugLayout _debugLayout;
    private readonly Stopwatch _updateTimeStopWatch;

    private SpriteBatch _spriteBatch;
    private GfxRenderer _gfxRenderer;
    private GameMenu _menu;
    private GameRenderTarget _debugGameRenderTarget;
    private PerformanceDebugWindow _performanceWindow;
    private int _skippedDraws = -1;
    private float _fps = 60;
    private bool _isClosingMenu;
    private bool _showMenu;

    #endregion

    #region Protected Properties

    protected abstract Game Game { get; }
    protected abstract int SoundBankResourceId { get; }
    protected abstract Dictionary<int, string> SongTable { get; }

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
        Engine.GameWindow.Resize(Window.ClientBounds.Size, JoyPad.Check(Keys.LeftShift), changeScreenSizeCallback: SetWindowSize);
    }

    private void StepEngine()
    {
        if (IsPaused)
            return;

        try
        {
            if (DebugMode)
                _updateTimeStopWatch.Restart();

            FrameManager.Step();

            // If this frame did a load, and thus might have taken longer than 1/60th of a second, then
            // we disable fixed time step to avoid MonoGame repeatedly calling Update() to make up for
            // the lost time, and thus drop frames
            if (Engine.IsLoading)
                IsFixedTimeStep = false;

            if (DebugMode)
                _updateTimeStopWatch.Stop();
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
        Engine.LoadRom(Engine.Config.RomFile, Engine.Config.SerializerLogFile, Game);

        GameWindow gameWindow = new(Engine.Settings);
        Engine.LoadMonoGame(GraphicsDevice, Content, new ScreenCamera(gameWindow), gameWindow);

        SoundManager.Load(SoundBankResourceId, SongTable);
        FontManager.Load(Engine.Loader.Font8, Engine.Loader.Font16, Engine.Loader.Font32);

        // TODO: Save window size in config, as well as if maximized etc.
        Point windowSize = new((int)(Engine.GameWindow.GameResolution.X * 4), (int)(Engine.GameWindow.GameResolution.Y * 4));
        Engine.GameWindow.Resize(windowSize);
        SetWindowSize(windowSize);

        FrameManager.SetNextFrame(CreateInitialFrame());

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _gfxRenderer = new GfxRenderer(_spriteBatch, Engine.GameWindow);
        _debugGameRenderTarget = new GameRenderTarget(GraphicsDevice, Engine.GameWindow);

        _menu = new GameMenu();

        _debugLayout.AddWindow(new GameDebugWindow(_debugGameRenderTarget));
        _debugLayout.AddWindow(_performanceWindow = new PerformanceDebugWindow());
        _debugLayout.AddWindow(new LoggerDebugWindow());
        _debugLayout.AddWindow(new GfxDebugWindow());
        _debugLayout.AddWindow(new SoundDebugWindow());
        _debugLayout.AddMenu(new WindowsDebugMenu());
        AddDebugWindowsAndMenus(_debugLayout);

        _debugLayout.LoadContent(this);

        base.LoadContent();
    }

    protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        base.Update(gameTime);

        // If the previous frame was loading, then we disable it now
        if (Engine.IsLoading)
        {
            Engine.IsLoading = false;
            IsFixedTimeStep = true;
        }

        _skippedDraws++;

        if (_skippedDraws > 0)
            _fps = 0;

        if (DebugMode)
        {
            if (!IsPaused)
            {
                _performanceWindow.AddFps(_fps);
                _performanceWindow.AddSkippedDraws(_skippedDraws);
            }

            using Process p = Process.GetCurrentProcess();
            _performanceWindow.AddMemoryUsage(p.PrivateMemorySize64);
        }

        JoyPad.Scan();

        if (RunSingleFrame)
        {
            RunSingleFrame = false;
            Pause();
        }

        // Toggle debug mode
        if (JoyPad.CheckSingle(Keys.Tab))
        {
            DebugMode = !DebugMode;

            if (DebugMode)
            {
                foreach (DebugWindow window in _debugLayout.GetWindows())
                    window.OnWindowOpened();
            }
            else
            {
                foreach (DebugWindow window in _debugLayout.GetWindows())
                    window.OnWindowClosed();
                
                SizeGameToWindow();
            }
        }

        // Toggle pause
        if (!_showMenu && JoyPad.Check(Keys.LeftControl) && JoyPad.CheckSingle(Keys.P))
        {
            if (!IsPaused)
                Pause();
            else
                Resume();
        }

        // Toggle menu
        if (!_isClosingMenu && JoyPad.CheckSingle(Keys.Escape))
        {
            if (!_showMenu)
            {
                Pause();
                _menu.Open();
                _showMenu = true;
            }
            else
            {
                _isClosingMenu = true;
                _menu.Close();
            }
        }

        if (_isClosingMenu && !_menu.IsTransitioningOut)
        {
            _isClosingMenu = false;
            _showMenu = false;
            Resume();
        }

        // Run one frame
        if (!_showMenu && JoyPad.Check(Keys.LeftControl) && JoyPad.CheckSingle(Keys.F))
        {
            IsPaused = false;
            RunSingleFrame = true;
        }

        StepEngine();

        if (_showMenu)
            _menu.Update();

        if (DebugMode && !IsPaused)
            _performanceWindow.AddUpdateTime(_updateTimeStopWatch.ElapsedMilliseconds);
    }

    protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
    {
        _fps = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

        _skippedDraws = -1;

        if (DebugMode)
            _debugGameRenderTarget.BeginRender();

        // Clear screen
        GraphicsDevice.Clear(Color.Black);

        // Draw screen
        Gfx.Draw(_gfxRenderer);
        if (_showMenu)
            _menu.Draw(_gfxRenderer);
        if (DebugMode)
            _debugLayout.DrawGame(_gfxRenderer);
        _gfxRenderer.EndRender();

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

    #region Public Methods

    public void Pause()
    {
        IsPaused = true;
        SoundManager.Pause();
    }

    public void Resume()
    {
        IsPaused = false;
        SoundManager.Resume();
    }

    #endregion
}