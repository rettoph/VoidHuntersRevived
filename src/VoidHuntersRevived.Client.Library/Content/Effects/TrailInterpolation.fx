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
    nointerpolation float4 Color : COLOR0;
    float2 WorldPosition : TEXCOORD0;
    nointerpolation float2 SegmentStart : TEXCOORD1;
    nointerpolation float Slope : TEXCOORD2;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    nointerpolation float4 Color : COLOR0;
    float2 WorldPosition : TEXCOORD0;
    nointerpolation float2 SegmentStart : TEXCOORD1;
    nointerpolation float Slope : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    output.WorldPosition = input.WorldPosition;
    output.SegmentStart = input.SegmentStart;
    output.Slope = input.Slope;
	
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    if (input.WorldPosition.x == input.SegmentStart.x && input.WorldPosition.y == input.SegmentStart.y)
        return float4(1, 1, 1, 1);
    
    float angle = atan2(input.SegmentStart.y - input.WorldPosition.y, input.SegmentStart.x - input.WorldPosition.x);
    
    if (angle - input.Slope > 0)
        return float4(1, 0, 0, 1);
    else
        return float4(0, 0, 1, 1);

}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};