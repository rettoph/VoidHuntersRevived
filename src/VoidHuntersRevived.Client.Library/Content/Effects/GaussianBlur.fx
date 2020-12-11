// Pixel shader applies a one dimensional gaussian blur filter. This is used twice by the bloom postprocess, first to
// blur horizontally, and then again to blur vertically.

sampler s0; // from SpriteBatch

#define SAMPLE_COUNT 15

float2 _sampleOffsets[SAMPLE_COUNT];
float _sampleWeights[SAMPLE_COUNT];


float4 PixelShaderFunctionX(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;
    
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
        c += tex2D(s0, texCoord + float2(_sampleOffsets[i].x, 0)) * _sampleWeights[i];
    
    return c;
}

float4 PixelShaderFunctionY(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;
    
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
        c += tex2D(s0, texCoord + float2(0, _sampleOffsets[i].y)) * _sampleWeights[i];
    
    return c;
}


technique GaussianBlur
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunctionX();
    }
    pass Pass2
    {
        PixelShader = compile ps_2_0 PixelShaderFunctionY();
    }
}