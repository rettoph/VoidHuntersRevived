using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// A Ship acts as a main container for a collection of parts
    /// The bridge is the rootmost part of the ship and should not
    /// change during the ships lifespan (however the functionality
    /// is not 100% disabled)
    /// 
    /// It will manage global ship-wide functions like tractor beams,
    /// overal health & status, shooting, spells, and movement
    /// </summary>
    class Ship : Entity
    {
        public TractorBeam TractorBeam { get; private set; }
        public ShipPart Bridge { get; set; }

        public Ship(EntityInfo info, IGame game) : base(info, game)
        {
        }

        protected override void HandleAddedToScene(object sender, ISceneObject e)
        {
            base.HandleAddedToScene(sender, e);

            // Create a tractor beam
            this.TractorBeam = this.Scene.Entities.Create<TractorBeam>("entity:tractor_beam");
        }

        protected override void HandleRemovedFromScene(object sender, ISceneObject e)
        {
            base.HandleRemovedFromScene(sender, e);

            // Remove the tractor beam as well
            this.TractorBeam.Scene?.Entities.Remove(this.TractorBeam);
        }

        public override void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
