using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Helpers;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Configurations
{
    public sealed class JointableConfiguration : IShipPartComponentConfiguration
    {
        public Vector2 LocalCenter { get; set; }
        public JointConfiguration[] Joints { get; set; }

        public JointableConfiguration() : this(Vector2.Zero)
        {

        }
        public JointableConfiguration(Vector2 localCenter, params JointConfiguration[] joints)
        {
            this.LocalCenter = localCenter;
            this.Joints = joints;
        }

        public void Initialize(string path, IServiceProvider services)
        {
            //
        }

        public void AttachComponentTo(Entity entity)
        {
            entity.Attach(new Jointable(this, entity));
        }

        public static JointableConfiguration Polygon(int sides)
        {
            var vertexAngles = PolygonHelper.CalculateVertexAngles(sides).ToArray();
            var joints = new List<JointConfiguration>();

            var start = vertexAngles[0];
            var end = vertexAngles[1];

            for(int i=1; i<sides + 1; i++)
            {
                end = vertexAngles[i % vertexAngles.Length];

                joints.Add(new JointConfiguration(
                    rotation: end.Angle + MathHelper.PiOver2,
                    position: (end.Vertex + start.Vertex) / 2));

                start = end;
            }


            return new JointableConfiguration(
                localCenter: joints.Select(x => x.Position).Average(),
                joints: joints.ToArray());
        }
    }
}
