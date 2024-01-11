namespace GbaMonoGame;

public static class MathHelpers
{
    public static float Mod(float x, float m)
    {
        float r = x % m;
        return r < 0 ? r + m : r;
    }
}