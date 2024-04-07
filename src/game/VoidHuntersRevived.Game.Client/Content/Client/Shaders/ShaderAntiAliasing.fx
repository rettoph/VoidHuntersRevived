#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1

float2 Pixel;
Texture2D<float4> Texture : register(t0);
SamplerState TextureSampler : register(s0);

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

void TrySample(float4 sourceValue, float2 sourceCoord, int offsetX, int offsetY, inout float4 sum, inout float count)
{
    float4 result = Texture.Sample(TextureSampler, sourceCoord + float2(Pixel.x * offsetX, Pixel.y * offsetY));
    
    if (result.a == 0)
    {
        return;
    }
    
    sum += result;
    count++;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    float4 source = Texture.Sample(TextureSampler, input.TextureCoordinates);
    
    if (source.a != 0)
    {
        return source;
    }
    
    float4 sum = source;
    float count = 1;
    
    TrySample(source, input.TextureCoordinates, -1, -1, sum, count);
    TrySample(source, input.TextureCoordinates, -1,  0, sum, count);
    TrySample(source, input.TextureCoordinates, -1,  1, sum, count);
    
    TrySample(source, input.TextureCoordinates,  0, -1, sum, count);
    TrySample(source, input.TextureCoordinates,  0, 0, sum, count);
    TrySample(source, input.TextureCoordinates,  0, 1, sum, count);
    
    TrySample(source, input.TextureCoordinates,  1, -1, sum, count);
    TrySample(source, input.TextureCoordinates,  1, 0, sum, count);
    TrySample(source, input.TextureCoordinates,  1, 1, sum, count);
	
    return (sum / count) * input.Color;
}



technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};