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
	float Direction : TEXCOORD2;
    float MaxRadius : TEXCOORD3;
    float CreatedTimestamp : TEXCOORD4;
    float MaxAge : TEXCOORD5;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	float age = min(input.MaxAge, CurrentTimestamp - input.CreatedTimestamp);
	float agePercent = age / input.MaxAge;
	
	// Calculate the position of an accelerating body after a certain time.
	float2 position2D = input.Position + input.Velocity * age + (0.5 * -input.Velocity * pow(age, 2));
	float4 color = lerp(input.Color, float4(0, 0, 0, 0), agePercent);
	
	if (input.MaxRadius > 0)
	{ // We only need to calculate the outward movement if radius is greater than 0
		float radius = input.MaxRadius * agePercent;
		position2D += float2(cos(input.Direction), sin(input.Direction)) * radius;
		color = float4(0, 0, 0, 0);
	}
	
	VertexShaderOutput output = (VertexShaderOutput) 0;

	output.Position = mul(float4(position2D, 0, 1), WorldViewProjection);
	output.Color = color;
    
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
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