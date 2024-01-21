using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame;

public class GameInstallation
{
    public GameInstallation(string directory, string gameFilePath, string saveFilePath, Game game, Platform platform)
    {
        Directory = directory;
        GameFilePath = gameFilePath;
        SaveFilePath = saveFilePath;
        Game = game;
        Platform = platform;
    }

    public string Directory { get; }
    public string GameFilePath { get; }
    public string SaveFilePath { get; }
    public Game Game { get; }
    public Platform Platform { get; }
}