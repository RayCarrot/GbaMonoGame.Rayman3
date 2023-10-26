﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OnyxCs.Gba.Engine2d;

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
    private GameRenderTarget _debugGameRenderTarget;

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
        Gfx.GfxCamera.Resize(Window.ClientBounds.Size, JoyPad.Check(Keys.LeftShift), changeScreenSizeCallback: newSize =>
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
        Gfx.GfxCamera = new GfxCamera(_graphics, Window.ClientBounds.Size);
    }

    private void LoadGame()
    {
        ObjectFactory.Init(new Dictionary<ActorType, ObjectFactory.CreateActor>()
        {
            { ActorType.Rayman, (id, resource) => new Rayman(id, resource) },

            { ActorType.Piranha, (id, resource) => new Piranha(id, resource) },
        });

        FrameManager.SetNextFrame(new Intro());
    }

    private void StepEngine()
    {
        try
        {
            JoyPad.Scan();

            if (_config.Paused) 
                return;
            
            // The game doesn't clear sprites here, but rather in places such as the animation player. For us this
            // however makes more sense, so we always start each frame fresh.
            Gfx.ClearSprites();

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