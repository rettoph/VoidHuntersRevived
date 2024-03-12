#define VS_SHADERMODEL vs_5_0 
#define PS_SHADERMODEL ps_5_0 

static const uint RMask = 0x000000ff;
static const uint GMask = 0x0000ff00;
static const uint BMask = 0x00ff0000;
static const uint AMask = 0xff000000;
static const uint IsTraceFlag = 0x00000001;
static const uint IsOuterFlag = 0x00000002;

matrix View;
matrix Projection;

float TraceScale;
float TraceDiffusionScale;

struct VertexShaderStaticInput
{
    float2 Position : POSITION0;
    uint Flags : BLENDINDICES0;
};

struct VertexShaderInstanceInput
{
    matrix Transformation : BLENDWEIGHT0;
    float4 PrimaryColor : COLOR0;
    float4 SecondaryColor : COLOR1;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
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

VertexShaderOutput MainVS(in VertexShaderStaticInput staticInput, VertexShaderInstanceInput instanceInput)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = float4(staticInput.Position, 0, 1);
    output.Position = mul(output.Position, instanceInput.Transformation);
    output.Position = mul(output.Position, View);
    output.Position = mul(output.Position, Projection);
    // output.Position = mul(float4(staticInput.Position, 0, 1), WorldViewProjection);
    
    if ((staticInput.Flags & IsTraceFlag) == 0)
    {
        output.Color = instanceInput.PrimaryColor;
        output.Depth = 0.0f;
    }
    else
    {
        output.Color = instanceInput.SecondaryColor;
        
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

float4 MainPS(VertexShaderOutput input) : SV_Target0
{    
    float depth = 1 - abs(input.Depth);
    
    if (depth < TraceScale)
    {
        return float4(0, 0, 0, 0);
    }
    else if (depth < TraceDiffusionScale)
    {
        return input.Color * TraceDiffusionAlpha(depth);
    }


    return input.Color;
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};