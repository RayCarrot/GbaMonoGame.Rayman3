namespace GbaMonoGame.Rayman3;

public enum MultiplayerGameType
{
    RayTag = 0, // Tag Mode
    CatAndMouse = 1, // Burglar Mode
    Missile = 2, // Bumper Cars (GBA only)
    CaptureTheFlag = 3, // Capture the Flag (N-Gage only - set as mode 0 there, but we do 3 in order to support them all)
}