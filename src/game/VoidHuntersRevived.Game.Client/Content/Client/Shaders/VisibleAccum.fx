#define VS_SHADERMODEL vs_5_0 
#define PS_SHADERMODEL ps_5_0 

#include "_VisibleShared.fx"

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
};

VertexShaderOutput MainVS(in VertexShaderStaticInput staticInput, uint instanceID : SV_InstanceID, in VertexShaderInstanceInput instanceInput)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = TransformStaticPosition(staticInput.Position, instanceInput.Z, instanceInput.LocalTranformation);
    output.Color = GetColor(staticInput.Flags.IsTrace, instanceInput.PrimaryColor, instanceInput.SecondaryColor);
    
    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    float4 output = input.Color;

    return output + float4(0.f, 0.f, 0.f, 1000.f);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};