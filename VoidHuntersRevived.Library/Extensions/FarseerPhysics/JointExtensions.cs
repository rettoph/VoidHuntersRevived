using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.FarseerPhysics
{
    public static class JointExtensions
    {
        public static Joint Clone(this Joint joint, World world, Body bodyA, Body bodyB = null)
        {
            switch (joint.JointType)
            {
                case JointType.Revolute:
                    break;
                case JointType.Prismatic:
                    break;
                case JointType.Distance:
                    break;
                case JointType.Pulley:
                    break;
                case JointType.Gear:
                    break;
                case JointType.Wheel:
                    break;
                case JointType.Weld:
                    break;
                case JointType.Friction:
                    break;
                case JointType.Rope:
                    break;
                case JointType.Motor:
                    break;
                case JointType.Angle:
                    break;
                case JointType.FixedMouse:
                    break;
                case JointType.FixedFriction:
                    return (joint as FrictionJoint).CloneFrictionJoint(world, bodyA, bodyB);
                default:
                    throw new Exception($"Unable to clone Joint => {joint.JointType}");
            }

            return null;
        }

        #region Fixed Angle Joint
        private static FixedMouseJoint CloneFixedMouseJoint(this FixedMouseJoint joint, World world, Body bodyA)
        {
            var clone = JointFactory.CreateFixedMouseJoint(world, bodyA, joint.WorldAnchorB);
            clone.Breakpoint = joint.Breakpoint;
            clone.CollideConnected = joint.CollideConnected;
            clone.DampingRatio = joint.DampingRatio;
            clone.Frequency = joint.Frequency;
            clone.MaxForce = joint.MaxForce;

            return clone;
        }
        #endregion

        #region Friction Joint
        private static FrictionJoint CloneFrictionJoint(this FrictionJoint joint, World world, Body bodyA, Body bodyB)
        {
            var clone              = JointFactory.CreateFrictionJoint(world, bodyA, bodyB, joint.LocalAnchorA);
            clone.Breakpoint       = joint.Breakpoint;
            clone.CollideConnected = joint.CollideConnected;
            clone.MaxForce         = joint.MaxForce;
            clone.MaxTorque        = joint.MaxTorque;

            return clone;
        }
        #endregion
    }
}
