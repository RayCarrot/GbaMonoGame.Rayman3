#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// The original sprite texture (the first Texture2D always gets set to this)
extern Texture2D SpriteTexture;

// The palette parameters
extern Texture2D PaletteTexture;
extern float PaletteWidth;
extern float PaletteY;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

sampler2D PaletteTextureSampler = sampler_state
{
    Texture = <PaletteTexture>;

    // Do this to prevent interpolated values
    AddressU = clamp;
    AddressV = clamp;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Get the alpha value from the sprite texture. This is out palette index, in a range from 0-1.
    float colorIndex = tex2D(SpriteTextureSampler, input.TextureCoordinates).a;
    
    // Multiply by 255 to get a range from 0-255, thus getting the original byte value.
    colorIndex *= 255;
    
    // Divide by the palette width to get a range of 0-1, needed for the UV x coordinate.
    float paletteX = colorIndex / PaletteWidth;
    
    // Get the color from the palette texture.
    float4 paletteColor = tex2D(PaletteTextureSampler, float2(paletteX, PaletteY));
    
    // Return and multiply by the input color.
    return paletteColor * input.Color;
}

technique SpriteBlending
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};