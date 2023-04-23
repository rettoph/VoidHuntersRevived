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
    public class Node
    {
        public readonly NodeConfiguration Configuration;
        public readonly int EntityId;
        public readonly Degree[] Degrees;

        public Tree? Tree;
        public Vector2 LocalCenter;
        public Matrix LocalTransformation;
        public Matrix WorldTransformation;

        public Vector2 WorldPosition => Vector2.Transform(Vector2.Zero, this.WorldTransformation);
        public Vector2 CenterWorldPosition => Vector2.Transform(this.LocalCenter, this.WorldTransformation);

        public Node(NodeConfiguration configuration, int entityId)
        {
            this.Configuration = configuration;
            this.EntityId = entityId;
            this.Degrees = configuration.Degrees.Select((conf, idx) => new Degree(conf, this, idx)).ToArray();

            this.LocalCenter = configuration.Center;
            this.LocalTransformation = Matrix.Identity;
            this.WorldTransformation = Matrix.Identity;
        }

        public Degree? InDegree()
        {
            foreach (Degree degree in this.Degrees)
            {
                if (degree.Edge is null)
                {
                    continue;
                }

                if (degree.Edge.InDegree.Node == this)
                {
                    return degree;
                }
            }

            return null;
        }

        public IEnumerable<Degree> OutDegrees()
        {
            foreach(Degree degree in this.Degrees)
            {
                if(degree.Edge is null)
                {
                    continue;
                }

                if(degree.Edge.InDegree.Node == this)
                {
                    continue;
                }

                yield return degree;
            }
        }
    }
}
