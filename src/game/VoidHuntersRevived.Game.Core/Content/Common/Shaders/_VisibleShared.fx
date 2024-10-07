/// ------------------------------------------------------------
/// WARNING:
/// Updating this file will not change the source file hash at
/// .fx.cache.json
/// When making changes to this file you must manually clear the
/// cache for th changes to be picked up and applied
/// ------------------------------------------------------------

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
    uint PrimaryColor : COLOR0;
    uint SecondaryColor : COLOR1;
    float Z : COLOR2;
    matrix LocalTranformation : BLENDWEIGHT0;
};

float ByteToFloat(uint byte)
{
    return ((float) byte) / ((float) 255);
}

float4 UnpackColor(uint packed)
{
    return float4(
        ByteToFloat((packed & RMask) >> 0),
        ByteToFloat((packed & GMask) >> 8),
        ByteToFloat((packed & BMask) >> 16),
        ByteToFloat((packed & AMask) >> 24));
}

float4 TransformStaticPosition(float2 position, float z, float4x4 transformation)
{
    float4 result = mul(float4(position, z, 1.f), transformation);
    result = mul(result, WorldViewProjection);
    
    return result;
}

float4 GetColor(bool isTrace, uint primaryColor, uint secondaryColor)
{
    if (isTrace == true)
    {
        return UnpackColor(secondaryColor);
    }
    else
    {
        return UnpackColor(primaryColor);
    }
}