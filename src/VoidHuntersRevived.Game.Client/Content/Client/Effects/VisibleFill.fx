#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

static const int IsTraceFlag = 1 << 0;

matrix WorldViewProjection;
float4 Color;

struct VertexShaderInput
{
    float2 Position : TEXCOORD0;
    int Flags : TEXCOORD1;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = mul(float4(input.Position, 0, 1), WorldViewProjection);
    output.Color = Color;

    //if ((input.Flags & IsTraceFlag) == 0)
    //{
    //    output.Color = Color;
    //}
    //else
    //{
    //    output.Color = float4(1, 0, 0, 1);
    //}
    

    return output;
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
    }
};