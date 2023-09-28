#define VS_SHADERMODEL vs_5_0 
#define PS_SHADERMODEL ps_5_0 

static const int IsTraceFlag = 1 << 0;
static const int IsOuterFlag = 1 << 1;

matrix WorldViewProjection;
float4 PrimaryColor;
float4 SecondaryColor;
float Zoom;

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

float4 MainPS(VertexShaderOutput input) : SV_Target0
{    
    float depth = 1.0f - abs(input.Depth);
    
    if (depth < Zoom)
    {
        return float4(0, 0, 0, 0);
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