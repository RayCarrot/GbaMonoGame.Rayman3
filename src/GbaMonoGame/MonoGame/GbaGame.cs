using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game = BinarySerializer.Ubisoft.GbaEngine.Game;

namespace GbaMonoGame;

// TODO: Create crash screen in case of exception, during update or load
public abstract class GbaGame : Microsoft.Xna.Framework.Game
{
    #region Constructor

    protected GbaGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _updateTimeStopWatch = new Stopwatch();
        _gameInstallations = new List<GameInstallation>();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Force frame-rate to 60. The GBA framerate is actually 59.727500569606, but we do 60.
        SetFramerate(60);
    }

    #endregion

    #region Private Fields

    private const string InstalledGamesDirName = "Games";

    private readonly GraphicsDeviceManager _graphics;
    private readonly Stopwatch _updateTimeStopWatch;
    private readonly List<GameInstallation> _gameInstallations;

    private SpriteBatch _spriteBatch;
    private Texture2D _gbaIcon;
    private Texture2D _nGageIcon;
    private SpriteFont _font;
    private GfxRenderer _gfxRenderer;
    private MenuManager _menu;
    private DebugLayout _debugLayout;
    private GameRenderTarget _debugGameRenderTarget;
    private PerformanceDebugWindow _performanceWindow;
    private int _selectedGameInstallationIndex;
    private GameInstallation _selectedGameInstallation;
    private int _skippedDraws = -1;
    private float _fps = 60;
    private bool _showMenu;
    private bool _isChangingResolution;
    private Task _loadingGameInstallationTask;

    #endregion

    #region Protected Properties

    protected abstract Game Game { get; }
    protected abstract string Title { get; }
    protected abstract int SoundBankResourceId { get; }
    protected abstract Dictionary<int, string> SongTable { get; }

    #endregion

    #region Public Properties

    public bool HasLoadedGameInstallation => _selectedGameInstallation != null && _loadingGameInstallationTask == null;
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

    private void Menu_Closed(object sender, EventArgs e)
    {
        _showMenu = false;
        Resume();
    }

    private void GbaGame_Exiting(object sender, EventArgs e)
    {
        if (!HasLoadedGameInstallation)
            return;

        // Save window state
        SaveWindowState();

        // Save config
        Engine.SaveConfig();

        // Unload the engine
        Engine.Unload();
    }

    #endregion

    #region Private Methods

    private void SetFramerate(float fps)
    {
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1 / fps);
    }

    private void SetResolution(Point size, bool isFullscreen)
    {
        // A bit convoluted code for setting if it's fullscreen or not. But this seems to reduce flickering.

        _isChangingResolution = true;

        if (!isFullscreen && _graphics.IsFullScreen)
            _graphics.IsFullScreen = false;

        _graphics.PreferredBackBufferWidth = size.X;
        _graphics.PreferredBackBufferHeight = size.Y;
        _graphics.ApplyChanges();

        if (isFullscreen && !_graphics.IsFullScreen)
        {
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        if (HasLoadedGameInstallation)
            Engine.GameWindow.Resize(size.ToVector2());

        _isChangingResolution = false;
    }

    private Point GetResolution()
    {
        return new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
    }

    private void SizeGameToWindow()
    {
        if (!HasLoadedGameInstallation)
            return;

        Engine.GameWindow.Resize(GetResolution().ToVector2(), JoyPad.Check(Keys.LeftShift) && !_isChangingResolution, changeScreenSizeCallback: x => SetResolution(x.ToRoundedPoint(), false));
        SaveWindowState();
        Engine.SaveConfig();
    }

    private void LoadEngine(GameInstallation gameInstallation)
    {
        if (_loadingGameInstallationTask == null)
        {
            _selectedGameInstallation = gameInstallation;

            // TODO: If this throws the exception might get swallowed
            // Load the selected game installation
            _loadingGameInstallationTask = Task.Run(() => Engine.LoadGameInstallation(_selectedGameInstallation));
        }
        else if (_loadingGameInstallationTask.IsCompleted)
        {
            _loadingGameInstallationTask = null;

            // Load the MonoGame part of the engine
            GameWindow gameWindow = new(Engine.Settings);
            gameWindow.SetRequestedResolution(Engine.Config.InternalResolution?.ToVector2());
            Engine.LoadMonoGame(GraphicsDevice, Content, new ScreenCamera(gameWindow), gameWindow);

            // Load engine sounds and fonts
            SoundEventsManager.Load(SoundBankResourceId, SongTable);
            FontManager.Load(Engine.Loader.Font8, Engine.Loader.Font16, Engine.Loader.Font32);

            // Load window
            ApplyDisplayConfig();

            // Load the initial engine frame
            FrameManager.SetNextFrame(CreateInitialFrame());

            // Load the game
            LoadGame();

            // Load the renderer
            _gfxRenderer = new GfxRenderer(_spriteBatch, Engine.GameWindow);
            _debugGameRenderTarget = new GameRenderTarget(GraphicsDevice, Engine.GameWindow);

            // Load the menu
            _menu = new MenuManager();
            _menu.Closed += Menu_Closed;

            // Load the debug layout
            _debugLayout = new DebugLayout();
            _debugLayout.AddWindow(new GameDebugWindow(_debugGameRenderTarget));
            _debugLayout.AddWindow(_performanceWindow = new PerformanceDebugWindow());
            _debugLayout.AddWindow(new LoggerDebugWindow());
            _debugLayout.AddWindow(new GfxDebugWindow());
            _debugLayout.AddWindow(new SoundDebugWindow());
            _debugLayout.AddMenu(new WindowsDebugMenu());
            AddDebugWindowsAndMenus(_debugLayout);
            _debugLayout.LoadContent(this);
        }
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
    protected virtual void LoadGame() { }
    protected virtual void AddDebugWindowsAndMenus(DebugLayout debugLayout) { }

    protected override void Initialize()
    {
        Window.Title = Title;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += Window_ClientSizeChanged;

        Exiting += GbaGame_Exiting;

        Engine.LoadConfig();

        ApplyDisplayConfig();

        // Find all installed games
        foreach (string gameDir in Directory.EnumerateDirectories(InstalledGamesDirName))
        {
            foreach (string gameFile in Directory.EnumerateFiles(gameDir))
            {
                if (gameFile.EndsWith(".gba", StringComparison.InvariantCultureIgnoreCase))
                {
                    _gameInstallations.Add(new GameInstallation(gameDir, gameFile, Path.ChangeExtension(gameFile, ".sav"), Game, Platform.GBA));
                    break;
                }
                else if (gameFile.EndsWith(".app", StringComparison.InvariantCultureIgnoreCase))
                {
                    _gameInstallations.Add(new GameInstallation(gameDir, gameFile, Path.Combine(gameDir, "save.dat"), Game, Platform.NGage));
                    break;
                }
            }
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _gbaIcon = Content.Load<Texture2D>("GBA");
        _nGageIcon = Content.Load<Texture2D>("N-Gage");
        _font = Content.Load<SpriteFont>("Font");

        // If there's only one game installation we load that directly
        if (_gameInstallations.Count == 1)
            LoadEngine(_gameInstallations[0]);

        base.LoadContent();
    }

    protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        base.Update(gameTime);

        JoyPad.Scan();

        // Toggle full-screen
        if (JoyPad.Check(Keys.LeftAlt) && JoyPad.CheckSingle(Keys.Enter))
        {
            Engine.Config.IsFullscreen = !_graphics.IsFullScreen;
            SaveWindowState();
            Engine.SaveConfig();
            ApplyDisplayConfig();
        }

        // Update mouse visibility
        IsMouseVisible = !_graphics.IsFullScreen || DebugMode;

        // Select game installation if not loaded yet
        if (!HasLoadedGameInstallation)
        {
            if (_gameInstallations.Count == 0)
                return;

            // If loading, keep calling LoadEngine until done
            if (_loadingGameInstallationTask != null)
            {
                LoadEngine(_selectedGameInstallation);
            }
            else
            {
                // Use arrow keys to select game
                if (JoyPad.CheckSingle(Keys.Up))
                {
                    _selectedGameInstallationIndex--;
                    if (_selectedGameInstallationIndex < 0)
                        _selectedGameInstallationIndex = _gameInstallations.Count - 1;
                }
                else if (JoyPad.CheckSingle(Keys.Down))
                {
                    _selectedGameInstallationIndex++;
                    if (_selectedGameInstallationIndex > _gameInstallations.Count - 1)
                        _selectedGameInstallationIndex = 0;
                }

                // Select with space or enter (but not when alt is pressed due to fullscreen toggle)
                if (!JoyPad.Check(Keys.LeftAlt) && (JoyPad.CheckSingle(Keys.Space) || JoyPad.CheckSingle(Keys.Enter)))
                    LoadEngine(_gameInstallations[_selectedGameInstallationIndex]);
            }
            
            return;
        }

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
        if (!_menu.IsTransitioningOut && JoyPad.CheckSingle(Keys.Escape))
        {
            if (!_showMenu)
            {
                Pause();
                _menu.Open(new MainMenu(this));
                _showMenu = true;
            }
            else
            {
                _menu.Close();
            }
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
        // Draw game selection if not loaded yet
        if (!HasLoadedGameInstallation)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            const int xPos = 80;
            const int yPos = 60;

            // TODO: Make this look a bit nicer? Have it scale better at different resolution by creating a matrix when rendering based on the screen size.
            if (_gameInstallations.Count == 0)
            {
                _spriteBatch.DrawString(_font, "No games were found", new Vector2(xPos, yPos), Color.White);
            }
            else if (_loadingGameInstallationTask != null)
            {
                _spriteBatch.DrawString(_font, "Loading...", new Vector2(xPos, yPos), Color.White);
            }
            else
            {
                _spriteBatch.DrawString(_font, "Select game to play", new Vector2(xPos, yPos), Color.White);

                for (int i = 0; i < _gameInstallations.Count; i++)
                {
                    if (_gameInstallations[i].Platform == Platform.GBA)
                        _spriteBatch.Draw(_gbaIcon, new Vector2(xPos, yPos + 60 + i * 30), Color.White);
                    else if (_gameInstallations[i].Platform == Platform.NGage) 
                        _spriteBatch.Draw(_nGageIcon, new Vector2(xPos, yPos + 60 + i * 30), Color.White);

                    _spriteBatch.DrawString(
                        spriteFont: _font, 
                        text: Path.GetFileName(_gameInstallations[i].Directory), 
                        position: new Vector2(xPos + 80, yPos + 60 + i * 30), 
                        color: i == _selectedGameInstallationIndex ? Color.Yellow : Color.White);
                }
            }

            _spriteBatch.End();
            return;
        }

        _fps = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

        _skippedDraws = -1;

        if (DebugMode)
            _debugGameRenderTarget.BeginRender();

        // Clear screen
        GraphicsDevice.Clear(Color.Black);

        // Draw screen
        Gfx.Draw(_gfxRenderer);
        if (DebugMode && !IsPaused)
            _performanceWindow.AddDrawCalls(GraphicsDevice.Metrics.DrawCount);
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

    public void SaveWindowState()
    {
        if (!HasLoadedGameInstallation || _graphics.IsFullScreen)
            return;

        switch (Engine.Settings.Platform)
        {
            case Platform.GBA:
                Engine.Config.GbaWindowPosition = Window.Position;
                Engine.Config.GbaWindowResolution = GetResolution();
                break;

            case Platform.NGage:
                Engine.Config.NGageWindowPosition = Window.Position;
                Engine.Config.NGageWindowResolution = GetResolution();
                break;

            default:
                throw new UnsupportedPlatformException();
        }
    }

    public void ApplyDisplayConfig()
    {
        if (Engine.Config.IsFullscreen)
        {
            SetResolution(Engine.Config.FullscreenResolution, true);
        }
        else
        {
            Point? pos;
            Point res;

            if (HasLoadedGameInstallation)
            {
                pos = Engine.Settings.Platform switch
                {
                    Platform.GBA => Engine.Config.GbaWindowPosition,
                    Platform.NGage => Engine.Config.NGageWindowPosition,
                    _ => throw new UnsupportedPlatformException()
                };
                res = Engine.Settings.Platform switch
                {
                    Platform.GBA => Engine.Config.GbaWindowResolution,
                    Platform.NGage => Engine.Config.NGageWindowResolution,
                    _ => throw new UnsupportedPlatformException()
                };
            }
            else
            {
                // We don't know the platform yet, so default to the gba position and resolution since it'll be the most common one
                pos = Engine.Config.GbaWindowPosition;
                res = Engine.Config.GbaWindowResolution;
            }

            if (pos != null)
                Window.Position = pos.Value;

            SetResolution(res, false);
        }
    }

    public void Pause()
    {
        IsPaused = true;
        SoundEventsManager.PauseAllSongs();
    }

    public void Resume()
    {
        IsPaused = false;
        SoundEventsManager.ResumeAllSongs();
    }

    #endregion
}