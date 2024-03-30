#define VS_SHADERMODEL vs_5_0 
#define PS_SHADERMODEL ps_5_0 

#include "_VisibleShared.fx"

Texture2D<float4> AccumTexture : register(t0);
SamplerState AccumTextureSampler : register(s0);

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
    float Depth : TEXCOORD0;
    float2 TextureCoordinates : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderStaticInput staticInput, uint instanceID : SV_InstanceID, in VertexShaderInstanceInput instanceInput)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = TransformStaticPosition(staticInput.Position, instanceInput.LocalTranformation);
    
    // Convert to normalized device coordinates
    output.TextureCoordinates = output.Position.xy / output.Position.w;
    output.TextureCoordinates = float2(output.TextureCoordinates.x + 1, 1 - output.TextureCoordinates.y);
    output.TextureCoordinates /= 2;
    
    if ((staticInput.Flags & IsTraceFlag) == 0)
    {
        output.Color = UnpackColor(instanceInput.PrimaryColor);
        output.Depth = -100;
    }
    else
    {
        output.Color = UnpackColor(instanceInput.SecondaryColor);
        
        if ((staticInput.Flags & IsOuterFlag) == 0)
        {
            output.Depth = 0.0f;
        }
        else
        {
            output.Depth = 1.0f;
        }
    }
    
    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    float4 top = input.Color;
    
    if (input.Depth > -100)
    {
        float depth = 1 - abs(input.Depth);
        
        if (depth < TraceScale)
        {
            discard;
        }
        else if (depth < TraceDiffusionScale)
        {
            top.a *= TraceDiffusionAlpha(depth);
        }
    }
    
    float4 accum = AccumTexture.Sample(AccumTextureSampler, input.TextureCoordinates);
    
    // Alpha channel is:
    // (layers * 1000) + alpha;
    float layers = round(accum.a / 1000);
    float alpha = accum.a % 1000;
    
    // Divide the accum colors by the total number of layers
    float4 avg = float4(accum.rgb, alpha) / layers;
    
    return float4(((top.rgb + (avg.rgb * avg.a)) / 2), avg.a);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};