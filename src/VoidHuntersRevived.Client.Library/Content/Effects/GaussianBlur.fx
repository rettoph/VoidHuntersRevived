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

float2 InverseResolution;
float StreakLength;

static const float Kernel[5] =
{
	1.0 / 24.0,
	4.0 / 24.0,
	6.0 / 24.0,
	4.0 / 24.0,
	1.0 / 24.0
};


float4 VerticalePS(float2 Coords : TEXCOORD0) : COLOR
{	
	float4 color = 0;
	
	float2 offset = float2(StreakLength * InverseResolution.x, StreakLength * InverseResolution.y);
	
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(00, -2) * offset) * Kernel[0];
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(00, -1) * offset) * Kernel[1];
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(00, 00) * offset) * Kernel[2];
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(00, 01) * offset) * Kernel[3];
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(00, 02) * offset) * Kernel[4];
	
	return color;
}

float4 HorizontalPS(float2 Coords : TEXCOORD0) : COLOR
{
	float4 color = 0;
	
	float2 offset = float2(StreakLength * InverseResolution.x, StreakLength * InverseResolution.y);
	
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(-2, 00) * offset) * Kernel[0];
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(-1, 00) * offset) * Kernel[1];
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(00, 00) * offset) * Kernel[2];
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(01, 00) * offset) * Kernel[3];
	color += SpriteTexture.Sample(SpriteTextureSampler, Coords + float2(02, 00) * offset) * Kernel[4];
	
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL HorizontalPS();
	}
	pass P1
	{
		PixelShader = compile PS_SHADERMODEL VerticalePS();
	}
};