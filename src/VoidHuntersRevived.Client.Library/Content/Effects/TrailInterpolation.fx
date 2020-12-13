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
    float2 Port : TEXCOORD1;
    float2 Starboard : TEXCOORD2;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 WorldPosition : TEXCOORD0;
    float2 Port : TEXCOORD1;
    float2 Starboard : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    output.WorldPosition = input.WorldPosition;
    output.Port = input.Port;
    output.Starboard = input.Starboard;
	
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float length = distance(input.Port, input.Starboard);
    float position = distance(input.Port, input.WorldPosition);
    float ratio = position / length;
    float alpha = -4 * pow(ratio - 0.5, 2) + 1;
    
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