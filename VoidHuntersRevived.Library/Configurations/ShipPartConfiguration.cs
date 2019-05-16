using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    public struct ShipPartConfiguration : IEntityData
    {
        public readonly Vector2[] Vertices;

        public ShipPartConfiguration(
            Vector2[] vertices)
        {
            this.Vertices = vertices;
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
