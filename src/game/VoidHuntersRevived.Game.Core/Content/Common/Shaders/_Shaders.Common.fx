/// ------------------------------------------------------------
/// WARNING:
/// Updating this file will not change the source file hash at
/// .fx.cache.json
/// When making changes to this file you must manually clear the
/// cache for the changes to be picked up and applied
/// ------------------------------------------------------------

#pragma once

struct Complex
{
    float Real;
    float Imaginary;
};

struct Transform2D
{
    Complex Rotation;
    float2 Position;
};

Transform2D DecodeTransform(float4 packed)
{
    Transform2D result;
    result.Rotation.Real = packed.x;
    result.Rotation.Imaginary = packed.y;
    result.Position = packed.zw;
    return result;
}

float2 transform(float2 target, Transform2D transform)
{
    float2 result;
    //result.x = (target.x * transform.Rotation.Real) + (target.y * -transform.Rotation.Imaginary) + transform.Position.x;
    //result.y = (target.x * transform.Rotation.Imaginary) + (target.y * transform.Rotation.Real) + transform.Position.y;

    result.x = (target.x * transform.Rotation.Real) + (target.y * (-transform.Rotation.Imaginary)) + transform.Position.x;
    result.y = (target.x * transform.Rotation.Imaginary) + (target.y * transform.Rotation.Real) + transform.Position.y;
    
    return result;
}

// Transform2D transform(Transform2D left, Transform2D right)
// {
//     Transform2D result;
//     result.Position.x = (left.Position.x * right.Rotation.Real) - (left.Position.y * right.Rotation.Imaginary) + right.Position.x;
//     result.Position.y = (left.Position.x * right.Rotation.Imaginary) + (left.Position.y * right.Rotation.Real) + right.Position.y;
//     result.Rotation.Real = (left.Rotation.Real * right.Rotation.Real) - (left.Rotation.Imaginary * right.Rotation.Imaginary);
//     result.Rotation.Imaginary = (left.Rotation.Real * right.Rotation.Imaginary) + (left.Rotation.Imaginary * right.Rotation.Real);
// 
//     return result;
// }