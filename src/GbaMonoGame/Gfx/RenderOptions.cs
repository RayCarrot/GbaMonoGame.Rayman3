using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public record struct RenderOptions(bool Alpha, Effect Shader, GfxCamera Camera);