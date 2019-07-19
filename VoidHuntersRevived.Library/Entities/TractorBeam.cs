using Guppy;
using Guppy.Configurations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// The tractor beam entity is a simple object
    /// used to pick up and release free floating ship
    /// parts. It is constantly bound to a ship, and 
    /// must always be a certain "offset" relative to
    /// the ship.
    /// </summary>
    public class TractorBeam : Entity
    {
        #region Public Attributes
        public readonly Ship Ship;
        public Vector2 Offset { get; private set; }
        public Vector2 Position { get { return this.Ship.Bridge.Position + this.Offset; } }
        #endregion

        #region Constructors
        public TractorBeam(Ship ship, EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
            this.Ship = ship;
        }
        #endregion
    }
}
