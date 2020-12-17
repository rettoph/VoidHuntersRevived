#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
float CurrentTimestamp;
float MaxAge;
float SpreadSpeed;

struct VertexShaderInput
{
    float4 Color : COLOR0;
    float2 Position : TEXCOORD0;
    float SpreadDirection : TEXCOORD1;
    float CreatedTimestamp : TEXCOORD2;
    float2 ReverseImmpulse : TEXCOORD3;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    nointerpolation float4 Color : COLOR0;
    float2 WorldPosition : TEXCOORD0;
    float2 StartPosition : TEXCOORD1;
    float RayLength : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float age = CurrentTimestamp - input.CreatedTimestamp;
    float2 spread = float2(cos(input.SpreadDirection), sin(input.SpreadDirection)) * (SpreadSpeed * age);
    float4 position = mul(float4(input.Position + (input.ReverseImmpulse * age) + spread, 0, 1), WorldViewProjection);
    float4 startPosition = mul(float4(input.Position, 0, 1), WorldViewProjection);
    
    output.Position = position;
    output.Color = input.Color * float4(1, 1, 1, 1 - (age / MaxAge));
    output.WorldPosition = input.Position + spread;
    output.StartPosition = input.Position;
    output.RayLength = length(spread);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float alpha = 1 - (length(input.StartPosition - input.WorldPosition) / input.RayLength);
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