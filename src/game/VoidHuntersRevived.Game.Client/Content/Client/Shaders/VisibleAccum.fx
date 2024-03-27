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
    
    if ((staticInput.Flags & IsTraceFlag) == 0)
    {
        output.Color = UnpackColor(instanceInput.PrimaryColor);
        output.Depth = -100;
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
    float4 output = input.Color;
    
    if (input.Depth > -100)
    {
        float depth = 1 - abs(input.Depth);
        
        if (depth < TraceScale)
        {
            discard;
        }
        else if (depth < TraceDiffusionScale)
        {
            output.a *= TraceDiffusionAlpha(depth);
        }
    }


    return output + float4(0, 0, 0, 1000);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};