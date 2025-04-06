/// ------------------------------------------------------------
/// WARNING:
/// Updating this file will not change the source file hash at
/// .fx.cache.json
/// When making changes to this file you must manually clear the
/// cache for th changes to be picked up and applied
/// ------------------------------------------------------------

#pragma once

#include "_Shaders.Common.fx"

static const uint RMask = 0x000000ff;
static const uint GMask = 0x0000ff00;
static const uint BMask = 0x00ff0000;
static const uint AMask = 0xff000000;

matrix WorldViewProjection;

float TraceScale;
float TraceDiffusionScale;

struct VertexShaderStaticInputFlags
{
    bool IsTrace;
    bool1x3 Undefined;
};

struct VertexShaderStaticInput
{
    VertexShaderStaticInputFlags Flags : BLENDINDICES0;
    float2 Position : POSITION0;
};

struct VertexShaderInstanceInput
{
    float4 LocalTranformation_Packed : POSITION1;
    float4 PrimaryColor : COLOR0;
    float4 SecondaryColor : COLOR1;
};

float4 TransformStaticPosition(float2 localPosition, Transform2D transformation)
{
    float4 result = float4(transform(localPosition, transformation), 0.f, 1.f);
    result = mul(result, WorldViewProjection);

    result.z = 0.f;
    result.w = 1.f;

    return result;
}

float4 GetColor(bool isTrace, float4 primaryColor, float4 secondaryColor)
{
    if (isTrace == true)
    {
        return secondaryColor;
    }
    else
    {
        return primaryColor;
    }
}