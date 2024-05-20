using System;
using System.IO;
using BinarySerializer;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public static class Engine
{
    #region Paths

    public static string ConfigFilePath => "Config.json";
    public static string SerializerLogFilePath => "SerializerLog.txt";

    #endregion

    #region Engine

    public static GameConfig Config { get; private set; }
    public static Version Version => new(0, 0, 0);

    #endregion

    #region Rom

    public static GameInstallation GameInstallation { get; private set; }
    public static Context Context { get; private set; }
    public static Loader Loader { get; private set; }
    public static GbaEngineSettings Settings { get; private set; }

    #endregion

    #region MonoGame

    internal static bool IsLoading { get; set; }

    /// <summary>
    /// The graphics device to use for creating textures.
    /// </summary>
    public static GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// The content manager to load contents.
    /// </summary>
    public static ContentManager ContentManager { get; private set; }

    /// <summary>
    /// The screen camera to use when rendering the game.
    /// </summary>
    public static ScreenCamera ScreenCamera { get; private set; }

    public static GameViewPort GameViewPort { get; private set; }

    #endregion

    #region Cache

    // TODO: Show cache in debug layout
    public static Cache<Texture2D> TextureCache { get; } = new();
    public static Cache<Palette> PaletteCache { get; } = new();

    #endregion

    #region Private Static Methods

    private static void LoadRom()
    {
        ISerializerLogger serializerLogger = Config.WriteSerializerLog
            ? new FileSerializerLogger(SerializerLogFilePath)
            : null;

        Context = new Context(String.Empty, serializerLogger: serializerLogger, systemLogger: new BinarySerializerSystemLogger());
        Settings = new GbaEngineSettings { Game = GameInstallation.Game, Platform = GameInstallation.Platform };
        Context.AddSettings(Settings);

        if (GameInstallation.Platform == Platform.GBA)
        {
            GbaLoader loader = new(Context);
            loader.LoadFiles(GameInstallation.GameFilePath, cache: true);
            loader.LoadRomHeader(GameInstallation.GameFilePath);

            string gameCode = loader.RomHeader.GameCode;

            Context.AddPreDefinedPointers(Settings.Game switch
            {
                Game.Rayman3 when gameCode is "AYZP" => DefinedPointers.Rayman3_GBA_EU,
                //Game.Rayman3 when gameCode is "AYZE" => DefinedPointers.Rayman3_GBA_US, // TODO: Support US version
                _ => throw new Exception($"Unsupported game {Settings.Game} and/or code {gameCode}")
            });

            loader.LoadData(GameInstallation.GameFilePath);
            Loader = loader;
        }
        else if (GameInstallation.Platform == Platform.NGage)
        {
            string dataFileName = Path.ChangeExtension(GameInstallation.GameFilePath, ".dat");

            NGageLoader loader = new(Context);
            loader.LoadFiles(GameInstallation.GameFilePath, dataFileName, cache: true);

            Context.AddPreDefinedPointers(Settings.Game switch
            {
                Game.Rayman3 => DefinedPointers.Rayman3_NGage,
                _ => throw new Exception($"Unsupported game {Settings.Game}")
            });

            loader.LoadData(GameInstallation.GameFilePath, dataFileName);
            Loader = loader;
        }
        else
        {
            throw new UnsupportedPlatformException();
        }
    }

    #endregion

    #region Internal Static Methods

    internal static void LoadConfig()
    {
        Config = GameConfig.Load(ConfigFilePath);
    }

    internal static void SaveConfig()
    {
        Config.Save(ConfigFilePath);
    }

    internal static void LoadGameInstallation(GameInstallation gameInstallation)
    {
        GameInstallation = gameInstallation;
        LoadRom();
    }

    internal static void LoadMonoGame(GraphicsDevice graphicsDevice, ContentManager contentManager, ScreenCamera screenCamera, GameViewPort gameViewPort)
    {
        GraphicsDevice = graphicsDevice;
        ContentManager = contentManager;
        ScreenCamera = screenCamera;
        GameViewPort = gameViewPort;
    }

    internal static void Unload()
    {
        Context?.Dispose();
    }

    #endregion

    #region Public Static Methods

    public static void BeginLoad()
    {
        IsLoading = true;
    }

    #endregion
}