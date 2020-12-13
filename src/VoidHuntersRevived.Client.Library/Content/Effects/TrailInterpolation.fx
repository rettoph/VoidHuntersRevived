#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VertexShaderInput
{
	float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 WorldPosition : TEXCOORD0;
    float RayLength : TEXCOORD1;
    float2 Port : TEXCOORD2;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    nointerpolation float4 Color : COLOR0;
    float RayLength : TEXCOORD1;
    float SegmentLength : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    output.RayLength = input.RayLength;
    output.SegmentLength = distance(input.Port, input.WorldPosition);
    
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float ratio = input.SegmentLength / input.RayLength;
    float alpha = 1 - abs(ratio - 0.5) * 2;
    
    return input.Color * float4(1, 1, 1, alpha);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};