using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    internal sealed class TrailSegment : Frameable
    {
        #region Public Properties
        public Double Age { get; private set; }
        public Vector2 Position { get; private set; }
        #endregion

        #region Constructors
        internal TrailSegment()
        {

        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Age += gameTime.ElapsedGameTime.TotalSeconds;
        }
        #endregion
    }
}
