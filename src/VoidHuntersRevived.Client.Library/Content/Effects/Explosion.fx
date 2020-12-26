﻿#if OPENGL
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
    float MaxRadius : TEXCOORD1;
    float Direction : TEXCOORD2;
    float Alpha : TEXCOORD3;
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
	VertexShaderOutput output = (VertexShaderOutput)0;

    float age = (CurrentTimestamp - input.CreatedTimestamp) / input.MaxAge;
    float2 position2d = input.Position + (float2(cos(input.Direction), sin(input.Direction)) * input.MaxRadius * age);
	
    output.Position = mul(float4(position2d, 0, 1), WorldViewProjection);
    output.Color = input.Color * float4(1, 1, 1, input.Alpha * (1 - age));
    
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