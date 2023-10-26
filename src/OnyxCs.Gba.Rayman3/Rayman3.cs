using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using BinarySerializer;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public class Rayman3 : Game
{
    #region Constructor

    public Rayman3()
    {
        _graphics = new GraphicsDeviceManager(this);
        _debugRenderer = new DebugRenderer();
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
    private readonly DebugRenderer _debugRenderer;
    private SpriteBatch _spriteBatch;
    private GfxRenderer _renderer;
    private GameConfig _config;
    private Context _context;

    #endregion

    #region Public Properties

    public bool IsEnginePaused { get; set; }

    #endregion

    #region Event Handlers

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        Gfx.ScreenSize = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
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

    private void LoadRom()
    {
        string romFilePath = Path.GetFullPath(_config.RomFile);
        string romDir = Path.GetDirectoryName(romFilePath);
        string romFileName = Path.GetFileName(romFilePath);

        ISerializerLogger serializerLogger = _config.SerializerLogFile != null
            ? new FileSerializerLogger(_config.SerializerLogFile)
            : null;

        // TODO: Add a logger
        // TODO: Disable caching and instead only cache resources from root table
        _context = new Context(romDir!, serializerLogger: serializerLogger);

        using (FileStream file = File.OpenRead(romFilePath))
        {
            // TODO: Don't hard-code this
            const int dataOffset = 0x29beec;
            const int dataLength = 0x55FFB4;

            file.Position = dataOffset;
            byte[] data = new byte[dataLength];
            int read = file.Read(data, 0, data.Length);

            if (read != data.Length)
                throw new EndOfStreamException();

            _context.AddFile(new StreamFile(_context, romFileName, new MemoryStream(data), mode: VirtualFileMode.Maintain));
        }

        using (_context)
        {
            OffsetTable offsetTable = FileFactory.Read<OffsetTable>(_context, romFileName);
            _context.AddSettings(new OnyxGbaSettings()
            {
                RootTable = offsetTable
            });
        }

        Storage.SetContext(_context);
    }

    private void LoadEngine()
    {
        Gfx.GraphicsDevice = GraphicsDevice;
        Gfx.ScreenSize = new Vector2(_config.Width, _config.Height);
        ObjectFactory.Init(new Dictionary<ActorType, ObjectFactory.CreateActor>()
        {
            { ActorType.Rayman, (id, resource) => new Rayman(id, resource) },

            { ActorType.Piranha, (id, resource) => new Piranha(id, resource) },
        });
    }

    private void StepEngine()
    {
        try
        {
            // The game doesn't clear sprites here, but rather in places such as the animation player. For us this
            // however makes more sense, so we always start each frame fresh.
            Gfx.ClearSprites();

            JoyPad.Scan();
            FrameManager.Step();
            GameTime.Update();
        }
        catch
        {
            _context.Dispose();
            throw;
        }
    }

    #endregion

    #region Protected Methods

    protected override void Initialize()
    {
        LoadConfig();
        SetWindowSize((int)(_config.Width * _config.Scale), (int)(_config.Height * _config.Scale));
        LoadRom();
        LoadEngine();

        FrameManager.SetNextFrame(new Intro());

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _renderer = new GfxRenderer(_spriteBatch, Matrix.CreateScale(_config.Scale));
        _debugRenderer.LoadContent(this);
    }

    protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {

        _debugRenderer.Update(gameTime);

        if (!IsEnginePaused)
            StepEngine();

        base.Update(gameTime);
    }

    protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
    {
        // Clear screen
        GraphicsDevice.Clear(Color.Black);

        // Draw screen
        _renderer.Begin();
        Gfx.Draw(_renderer);
        _renderer.End();

        // Draw debug layout
        _debugRenderer.Draw(gameTime);

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