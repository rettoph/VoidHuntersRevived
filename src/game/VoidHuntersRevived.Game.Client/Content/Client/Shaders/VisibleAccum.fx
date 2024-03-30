#define VS_SHADERMODEL vs_5_0 
#define PS_SHADERMODEL ps_5_0 

#include "_VisibleShared.fx"

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
    float Depth : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderStaticInput staticInput, uint instanceID : SV_InstanceID, in VertexShaderInstanceInput instanceInput)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = TransformStaticPosition(staticInput.Position, instanceInput.LocalTranformation);
    
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
    float4 output = input.Color;
    
    if (input.Depth > -100)
    {
        float depth = 1 - abs(input.Depth);
        
        if (depth < TraceScale)
        {
            discard;
        }
        else if (depth < TraceDiffusionScale)
        {
            output.a *= TraceDiffusionAlpha(depth);
        }
    }


    return output + float4(0, 0, 0, 1000);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};