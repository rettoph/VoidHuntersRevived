#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler s0;
float2 texelSize = float2(0.0f, 0.0f);
float4 target = float4(1, 1, 1, 1);
float strength = 0.99f;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

bool check(float x, float y, float modifier)
{
	float2 texel = texelSize * modifier;

	if (tex2D(s0, float2(x + texel.x, y)).a == 0) {
		return true;
	}
	else if (tex2D(s0, float2(x - texel.x, y)).a == 0) {
		return true;
	}
	else if (tex2D(s0, float2(x, y + texel.y)).a == 0) {
		return true;
	}
	else if (tex2D(s0, float2(x, y - texel.y)).a == 0) {
		return true;
	}

	return false;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(s0, input.TextureCoordinates);

	if (color.a != 0 && (check(input.TextureCoordinates.x, input.TextureCoordinates.y, 1) || check(input.TextureCoordinates.x, input.TextureCoordinates.y, 2)))
	{
		color = lerp(color, target, strength);
	}


	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};