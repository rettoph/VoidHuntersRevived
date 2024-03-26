#define VS_SHADERMODEL vs_5_0 
#define PS_SHADERMODEL ps_5_0 

static const uint RMask = 0x000000ff;
static const uint GMask = 0x0000ff00;
static const uint BMask = 0x00ff0000;
static const uint AMask = 0xff000000;
static const uint IsTraceFlag = 0x00000001;
static const uint IsOuterFlag = 0x00000002;

matrix WorldViewProjection;

float TraceScale;
float TraceDiffusionScale;

Texture2D AccumTexture : register(t0);
SamplerState AccumTextureSampler : register(s0);

struct VertexShaderStaticInput
{
    float3 Position : POSITION0;
    uint Flags : BLENDINDICES0;
};

struct VertexShaderInstanceInput
{
    matrix LocalTranformation : BLENDWEIGHT0;
    uint PrimaryColor : COLOR0;
    uint SecondaryColor : COLOR1;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
    float Depth : TEXCOORD0;
    float2 TextureCoordinates : TEXCOORD1;
};

float ByteToFloat(uint byte)
{
    return ((float) byte) / ((float) 255);
}

float4 UnpackColor(uint packed)
{
    return float4(
        ByteToFloat((packed & RMask) >> 0),
        ByteToFloat((packed & GMask) >> 8),
        ByteToFloat((packed & BMask) >> 16),
        ByteToFloat((packed & AMask) >> 24));
}

VertexShaderOutput MainVS(in VertexShaderStaticInput staticInput, uint instanceID : SV_InstanceID, in VertexShaderInstanceInput instanceInput)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = mul(float4(staticInput.Position, 1), instanceInput.LocalTranformation);
    output.Position = mul(output.Position, WorldViewProjection);
    
    // Convert to normalized device coordinates
    output.TextureCoordinates = output.Position.xy / output.Position.w;
    output.TextureCoordinates = float2(output.TextureCoordinates.x + 1, 1 - output.TextureCoordinates.y);
    output.TextureCoordinates /= 2;
    
    if ((staticInput.Flags & IsTraceFlag) == 0)
    {
        output.Color = UnpackColor(instanceInput.PrimaryColor);
        output.Depth = 0.0f;
    }
    else
    {
        output.Color = UnpackColor(instanceInput.SecondaryColor);
        
        if ((staticInput.Flags & IsOuterFlag) == 0)
        {
            output.Depth = 0.0f;
        }
        else
        {
            output.Depth = 1.0f;
        }
    }
    
    return output;
}

float TraceDiffusionAlpha(float depth)
{
    return (depth - TraceScale) / (TraceDiffusionScale - TraceScale);
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    float depth = 1 - abs(input.Depth);
    
    float4 color = input.Color;
    if (depth < TraceScale)
    {
        discard;
    }
    else if (depth < TraceDiffusionScale)
    {
        color = input.Color * TraceDiffusionAlpha(depth);
    }

    float4 accum = AccumTexture.Sample(AccumTextureSampler, input.TextureCoordinates);
    
    if (accum.a < 2000)
    { // Indicates theres only 1 layer, as 2 layers would at least be 2000
        return input.Color;
    }

    // Alpha channel is:
    // (layers * 1000) + alpha;
    float layers = round(accum.a / 1000);
    float alpha = accum.a % 1000;
    
    // Divide the accum colors by the total number of layers
    float4 rgba = float4(accum.rgb, alpha) / layers;
    
    // Average the int color + avg.rgb, use the accum alpha avg
    return float4((color.rgb + (rgba.rgb * rgba.a)) / 2, rgba.a);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};