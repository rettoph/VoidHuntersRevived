using FarseerPhysics.Collision.Shapes;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    public struct ShipPartConfiguration : IEntityData
    {
        public readonly PolygonShape Shape;
        public readonly Vector3 MaleConnectionNode;

        public ShipPartConfiguration(
            PolygonShape shape,
            Vector3 maleConnectionNode)
        {
            this.Shape = shape;
            this.MaleConnectionNode = maleConnectionNode;
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
