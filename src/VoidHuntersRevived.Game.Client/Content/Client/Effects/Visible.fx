#define VS_SHADERMODEL vs_5_0 
#define PS_SHADERMODEL ps_5_0 

static const float BezierP1 = 1.0f;
static const float BezierP2 = 0.0f;

static const int IsTraceFlag = 1 << 0;
static const int IsOuterFlag = 1 << 1;

matrix WorldViewProjection;
float4 PrimaryColor;
float4 SecondaryColor;
float TraceScale;
float TraceDiffusionScale;

struct VertexShaderInput
{
    float2 Position : TEXCOORD0;
    int Flags : TEXCOORD1;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float Depth : TEXCOORD0;
};

static const float ThreeTimesBezierP1 = 3.0 * BezierP1;
static const float ThreeTimesBezierP2 = 3.0 * BezierP2;

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = mul(float4(input.Position, 0, 1), WorldViewProjection);

    if ((input.Flags & IsTraceFlag) == 0)
    {
        output.Color = PrimaryColor;
        output.Depth = 0.0f;
    }
    else
    {
        output.Color = SecondaryColor;
        
        if ((input.Flags & IsOuterFlag) == 0)
        {
            output.Depth = -1.0f;
        }
        else
        {
            output.Depth = 1.0f;
        }
    }
    

    return output;
}

float Bezier(float x)
{
    float oneMinusX = 1.0f - x;
    return (ThreeTimesBezierP1 * pow(oneMinusX, 2)) + (ThreeTimesBezierP2 * pow(x, 2) * oneMinusX) + pow(x, 3);
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
        return input.Color * float4(1, 1, 1, TraceDiffusionAlpha(depth));
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