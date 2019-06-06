﻿using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.ConnectionNodes
{
    public abstract class ConnectionNode : Entity
    {
        protected readonly ShipPart parent;

        public readonly Single LocalRotation;
        public readonly Vector2 LocalPosition;

        public Vector2 WorldPosition
        {
            get
            {
                return this.parent.Position + Vector2.Transform(this.LocalPosition, this.OffsetMatrix);
            }
        }
        public Single WorldRotation
        {
            get
            {
                return this.parent.Rotation + this.LocalRotation;
            }
        }

        public Matrix OffsetMatrix
        {
            get
            {
                return Matrix.CreateRotationZ(this.parent.Rotation);
            }
        }

        #region Constructors
        public ConnectionNode(ShipPart parent, Single rotation, Vector2 position, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
            this.parent = parent;

            this.LocalRotation = rotation;
            this.LocalPosition = position;

            this.SetUpdateOrder(200);
        }
        #endregion
    }
}
