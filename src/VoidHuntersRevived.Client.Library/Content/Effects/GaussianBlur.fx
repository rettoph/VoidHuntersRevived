#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

SamplerState SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
	
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	MipFilter = LINEAR;

	AddressU = CLAMP;
	AddressV = CLAMP;
};

float InverseHeight;
float InverseWidth;
float Strength;


float4 VerticalePS(float2 Coords : TEXCOORD0) : COLOR
{	
	float4 color = 0;
	
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(00, -2) * Strength) * InverseHeight) * 0.0625;
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(00, -1) * Strength) * InverseHeight) * 0.25;
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(00, 00) * Strength) * InverseHeight) * 0.375;
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(00, 01) * Strength) * InverseHeight) * 0.25;
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(00, 02) * Strength) * InverseHeight) * 0.0625;
	
	return color;
}

float4 HorizontalPS(float2 Coords : TEXCOORD0) : COLOR
{
	float4 color = 0;
	
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(-2, 00) * Strength) * InverseWidth) * 0.0625;
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(-1, 00) * Strength) * InverseWidth) * 0.25;
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(00, 00) * Strength) * InverseWidth) * 0.375;
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(01, 00) * Strength) * InverseWidth) * 0.25;
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + (float2(02, 00) * Strength) * InverseWidth) * 0.0625;
	
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL VerticalePS();
	}
	pass P1
	{
		PixelShader = compile PS_SHADERMODEL HorizontalPS();
	}
};