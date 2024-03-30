static const uint RMask = 0x000000ff;
static const uint GMask = 0x0000ff00;
static const uint BMask = 0x00ff0000;
static const uint AMask = 0xff000000;
static const uint IsTraceFlag = 0x00000001;
static const uint IsOuterFlag = 0x00000002;

matrix WorldViewProjection;

float TraceScale;
float TraceDiffusionScale;

struct VertexShaderStaticInput
{
    float3 Position : POSITION0;
    uint Flags : BLENDINDICES0;
};

struct VertexShaderInstanceInput
{
    matrix LocalTranformation : BLENDWEIGHT0;
    uint PrimaryColor : COLOR0;
    uint SecondaryColor : COLOR1;
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

float TraceDiffusionAlpha(float depth)
{
    return (depth - TraceScale) / (TraceDiffusionScale - TraceScale);
}

float4 TransformStaticPosition(float3 position, float4x4 instance)
{
    float4 result = mul(float4(position, 1), instance);
    result = mul(result, WorldViewProjection);
    
    return result;
}