﻿using FarseerPhysics.Collision.Shapes;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    public struct ShipPartConfiguration : IEntityData
    {
        public readonly Shape Shape;

        public ShipPartConfiguration(
            Shape shape)
        {
            this.Shape = shape;
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
