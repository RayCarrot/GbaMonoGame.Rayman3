using System;
using System.IO;
using System.Text.Json;
using BinarySerializer;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public static class Engine
{
    #region Engine

    public static GameConfig Config { get; private set; }
    public static Version Version => new(0, 0, 0);

    #endregion

    #region Rom

    public static Context Context { get; private set; }
    public static Loader Loader { get; private set; }
    public static OnyxGbaSettings Settings { get; private set; }

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

    public static GameWindow GameWindow { get; private set; }

    #endregion

    #region Internal Static Methods

    internal static void LoadConfig(string filePath)
    {
        if (String.IsNullOrWhiteSpace(filePath))
            throw new Exception("No config file has not been defined");

        if (!File.Exists(filePath))
            throw new Exception($"Config file {filePath} does not exist");

        Config = JsonSerializer.Deserialize<GameConfig>(File.ReadAllText(filePath));
    }

    internal static void LoadRom(string romFilePath, string serializerLogFilePath, Game game)
    {
        Platform platform;

        if (romFilePath.EndsWith(".gba", StringComparison.InvariantCultureIgnoreCase))
            platform = Platform.GBA;
        else if (romFilePath.EndsWith(".app", StringComparison.InvariantCultureIgnoreCase))
            platform = Platform.NGage;
        else
            throw new Exception($"Could not determine platform from ROM file {romFilePath}");

        ISerializerLogger serializerLogger = serializerLogFilePath != null
            ? new FileSerializerLogger(serializerLogFilePath)
            : null;

        // TODO: Add a logger
        Context = new Context(String.Empty, serializerLogger: serializerLogger);
        Settings = new OnyxGbaSettings { Game = game, Platform = platform };
        Context.AddSettings(Settings);

        if (platform == Platform.GBA)
        {
            GbaLoader loader = new(Context);
            loader.LoadFiles(romFilePath, cache: true);
            loader.LoadRomHeader(romFilePath);

            string gameCode = loader.RomHeader.GameCode;

            Context.AddPreDefinedPointers(Settings.Game switch
            {
                Game.Rayman3 when gameCode is "AYZP" => DefinedPointers.Rayman3_GBA_EU,
                //Game.Rayman3 when gameCode is "AYZE" => DefinedPointers.Rayman3_GBA_US, // TODO: Support US version
                _ => throw new Exception($"Unsupported game {Settings.Game} and/or code {gameCode}")
            });

            loader.LoadData(romFilePath);
            Loader = loader;
        }
        else if (platform == Platform.NGage)
        {
            string dataFileName = Path.ChangeExtension(romFilePath, ".dat");

            NGageLoader loader = new(Context);
            loader.LoadFiles(romFilePath, dataFileName, cache: true);

            Context.AddPreDefinedPointers(Settings.Game switch
            {
                Game.Rayman3 => DefinedPointers.Rayman3_NGage,
                _ => throw new Exception($"Unsupported game {Settings.Game}")
            });

            loader.LoadData(romFilePath, dataFileName);
            Loader = loader;
        }
    }

    internal static void LoadMonoGame(GraphicsDevice graphicsDevice, ContentManager contentManager, ScreenCamera screenCamera, GameWindow gameWindow)
    {
        GraphicsDevice = graphicsDevice;
        ContentManager = contentManager;
        ScreenCamera = screenCamera;
        GameWindow = gameWindow;
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