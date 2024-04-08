#define VS_SHADERMODEL vs_5_0 
#define PS_SHADERMODEL ps_5_0 

#include "_VisibleShared.fx"

Texture2D<float4> AccumTexture : register(t0);
SamplerState AccumTextureSampler : register(s0);

bool HideAccum;
bool HideTop;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
    float2 TextureCoordinates : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderStaticInput staticInput, uint instanceID : SV_InstanceID, in VertexShaderInstanceInput instanceInput)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = TransformStaticPosition(staticInput.Position, instanceInput.LocalTranformation);
    output.Color = GetColor(staticInput.Flags.x, instanceInput.PrimaryColor, instanceInput.SecondaryColor);
    
    // Convert to normalized device coordinates
    output.TextureCoordinates = output.Position.xy / output.Position.w;
    output.TextureCoordinates = float2(output.TextureCoordinates.x + 1, 1 - output.TextureCoordinates.y);
    output.TextureCoordinates /= 2;
    
    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    float4 sum = (float4) 0;
    int count = 0;
    
    if (HideTop == false)
    {
        float4 top = input.Color;
        sum += top;
        count++;
    }
    
    if (HideAccum == false)
    {
        float4 accum = AccumTexture.Sample(AccumTextureSampler, input.TextureCoordinates);

        if (accum.a > 2000)
        {
            // Alpha channel is:
            // (layers * 1000) + alpha;
            float layers = max(1, floor(accum.a / 1000));
            float alpha = accum.a % 1000;
    
            // Divide the accum colors by the total number of layers
            float4 avg = float4(accum.rgb, alpha) / layers;
        
            sum += avg;
            count++;   
        }
    }

    
    return sum / count;
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};