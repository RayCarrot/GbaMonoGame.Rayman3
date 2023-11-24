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

    #endregion

    #region Rom

    public static Context Context { get; private set; }
    public static Loader Loader { get; private set; }
    public static OnyxGbaSettings Settings { get; private set; }

    #endregion

    #region MonoGame

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
    public static ScreenCamera ScreenCamera { get; set; }

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
        romFilePath = Path.GetFullPath(romFilePath);
        string romDir = Path.GetDirectoryName(romFilePath);
        string romFileName = Path.GetFileName(romFilePath);

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
        Context = new Context(romDir!, serializerLogger: serializerLogger);
        Settings = new OnyxGbaSettings { Game = game, Platform = platform };
        Context.AddSettings(Settings);

        if (platform == Platform.GBA)
        {
            GbaLoader loader = new(Context);
            loader.LoadFiles(romFileName, cache: true);
            loader.LoadRomHeader(romFileName);

            string gameCode = loader.RomHeader.GameCode;

            Context.AddPreDefinedPointers(Settings.Game switch
            {
                Game.Rayman3 when gameCode is "AYZP" => DefinedPointers.Rayman3_GBA_EU,
                //Game.Rayman3 when gameCode is "AYZE" => DefinedPointers.Rayman3_GBA_US, // TODO: Support US version
                _ => throw new Exception($"Unsupported game {Settings.Game} and/or code {gameCode}")
            });

            loader.LoadData(romFileName);
            Loader = loader;
        }
        else if (platform == Platform.NGage)
        {
            string dataFileName = Path.ChangeExtension(romFileName, ".dat");

            NGageLoader loader = new(Context);
            loader.LoadFiles(romFileName, dataFileName, cache: true);

            Context.AddPreDefinedPointers(Settings.Game switch
            {
                Game.Rayman3 => DefinedPointers.Rayman3_NGage,
                _ => throw new Exception($"Unsupported game {Settings.Game}")
            });

            loader.LoadData(romFileName, dataFileName);
            Loader = loader;
        }
    }

    internal static void LoadMonoGame(GraphicsDevice graphicsDevice, ContentManager contentManager, ScreenCamera screenCamera)
    {
        GraphicsDevice = graphicsDevice;
        ContentManager = contentManager;
        ScreenCamera = screenCamera;
    }

    internal static void Unload()
    {
        Context?.Dispose();
    }

    #endregion
}