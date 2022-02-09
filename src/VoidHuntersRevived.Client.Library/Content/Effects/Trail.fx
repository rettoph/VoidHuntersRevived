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

struct VertexShaderInput
{
    float4 Color : COLOR0;
    float2 Position : TEXCOORD0;
    float2 Velocity : TEXCOORD1;
	float SpreadSpeed : TEXCOORD2;
	float SpreadDirection : TEXCOORD3;
	float CreatedTimestamp : TEXCOORD4;
	float MaxAge : TEXCOORD5;
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
	float agePercent = age / input.MaxAge;


	// Calculate the position of an accelerating body after a certain time.
	float2 acceleration = -input.Velocity / input.MaxAge;
	float2 startPosition2D = input.Position + input.Velocity * age + (0.5 * acceleration * pow(age, 2));
    float2 spread = float2(cos(input.SpreadDirection), sin(input.SpreadDirection)) * (input.SpreadSpeed * age);
    float2 worldPosition2D = startPosition2D + spread;

	output.Position = mul(float4(worldPosition2D, 0, 1), WorldViewProjection);
	output.Color = input.Color * float4(1, 1, 1, 1 - agePercent);
	output.StartPosition = startPosition2D;
	output.WorldPosition = worldPosition2D;
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