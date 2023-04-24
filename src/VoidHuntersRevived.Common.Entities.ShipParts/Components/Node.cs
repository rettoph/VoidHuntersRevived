using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public partial class Node
    {
        private static int CurrentId;

        public readonly int Id;
        public readonly NodeConfiguration Configuration;
        public readonly int EntityId;
        public readonly Joint[] Joints;

        public Tree? Tree;
        public Vector2 LocalCenter;
        public Matrix LocalTransformation;
        public Matrix WorldTransformation;

        public Vector2 WorldPosition => Vector2.Transform(Vector2.Zero, this.WorldTransformation);
        public Vector2 CenterWorldPosition => Vector2.Transform(this.LocalCenter, this.WorldTransformation);

        public Node(NodeConfiguration configuration, int entityId)
        {
            this.Id = CurrentId++;
            this.Configuration = configuration;
            this.EntityId = entityId;
            this.Joints = configuration.Joints.Select((conf, idx) => new Joint(conf, this, idx)).ToArray();

            this.LocalCenter = configuration.Center;
            this.LocalTransformation = Matrix.Identity;
            this.WorldTransformation = Matrix.Identity;
        }

        /// <summary>
        /// This name is a bit confusing, but realize it is from the perspective of the current node.
        /// Each specific node may have many joints that are parents to other nodes, but only one child
        /// node.
        /// </summary>
        /// <returns></returns>
        public Joint? ChildJoint()
        {
            foreach (Joint joint in this.Joints)
            {
                if (joint.Link is null)
                {
                    continue;
                }

                if (joint.Link.Child.Node == this)
                {
                    return joint;
                }
            }

            return null;
        }

        /// <summary>
        /// Each node may have many nodes with children, these are those parent nodes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Joint> ParentJoints()
        {
            foreach(Joint joint in this.Joints)
            {
                if(joint.Link is null)
                {
                    continue;
                }

                if(joint.Link.Child.Node == this)
                {
                    continue;
                }

                yield return joint;
            }
        }
    }
}
