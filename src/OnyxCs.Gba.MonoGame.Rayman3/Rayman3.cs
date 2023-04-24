using System;
using System.IO;
using System.Text.Json;
using BinarySerializer;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameTime = Microsoft.Xna.Framework.GameTime;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace OnyxCs.Gba.MonoGame.Rayman3;

public class Rayman3 : Game
{
    public Rayman3()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += Window_ClientSizeChanged;

        // Force frame-rate to 60
        SetFramerate(60);
    }

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

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        _gbaGame.MonoGameVram.SetSize(GraphicsDevice, Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
    }

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GbaGameComponent _gbaGame;
    private GameConfig _config;

    protected override void Initialize()
    {
        LoadConfig();
        SetWindowSize((int)(_config.Width * _config.Scale), (int)(_config.Height * _config.Scale));

        string romFilePath = Path.GetFullPath(_config.RomFile);
        string romDir = Path.GetDirectoryName(romFilePath);
        string romFileName = Path.GetFileName(romFilePath);

        ISerializerLogger serializerLogger = _config.SerializerLogFile != null 
            ? new FileSerializerLogger(_config.SerializerLogFile)
            : null;

        // TODO: Add a logger
        // TODO: Disable caching and instead only cache resources from root table
        Context context = new(romDir, serializerLogger: serializerLogger);

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

            context.AddFile(new StreamFile(context, romFileName, new MemoryStream(data), mode: VirtualFileMode.Maintain));
        }

        using (context)
        {
            OffsetTable offsetTable = FileFactory.Read<OffsetTable>(context, romFileName);
            context.AddSettings(new OnyxGbaSettings()
            {
                RootTable = offsetTable
            });
        }

        MonoGameVram vram = new(GraphicsDevice, _config.Width, _config.Height);

        _gbaGame = new GbaGameComponent(this, context, new Gba.Rayman3.Rayman3(vram, new MonoGameJoyPad()), vram);

        Components.Add(_gbaGame);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add custom update code

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Clear screen
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw screen
        _gbaGame.MonoGameVram.Draw(_spriteBatch, Vector2.Zero, new Vector2(_config.Scale));
        
        _spriteBatch.End();
            
        base.Draw(gameTime);
    }
}