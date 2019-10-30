using Guppy;
using Guppy.Extensions.Collection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Utilities.Controllers;

namespace VoidHuntersRevived.Library.Entities
{
    public class Explosion : NetworkEntity
    {
        #region Private Fields
        private Guid _sourceId;
        private Vector2 _sourceLinearVelocity;
        private Single _sourceAngularVelocity;
        private Vector2 _sourcePosition;
        private Single _strength;
        private Queue<ShipPart> _still;
        private ChunkCollection _chunks;
        private ShipPart _target;
        private Double _life;
        #endregion

        #region Internal Fields
        internal ShipPartController controller;
        #endregion

        #region Public Attributes
        public IEnumerable<ShipPart> Components { get => this.controller.Components; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _strength = 10;
            _still = new Queue<ShipPart>();
            _chunks = provider.GetRequiredService<ChunkCollection>();

            this.controller = provider.GetRequiredService<ShipPartController>();
            this.controller.CollidesWith = Categories.PassiveCollidesWith;
            this.controller.CollisionCategories = Categories.PassiveCollisionCategories;
            this.controller.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _life = 0;
        }

        public override void Dispose()
        {
            base.Dispose();

            // Ensure the controller is cleared
            this.controller.SyncChain(null);
        }
        #endregion

        #region Helper Methods
        public void SetSource(ShipPart target)
        {
            _sourceId = target.Id;
            _sourceLinearVelocity = target.LinearVelocity;
            _sourceAngularVelocity = target.AngularVelocity;
            _sourcePosition = target.WorldCenteroid;

            // Add all components to the explosion
            this.controller.SyncChain(target);

            // Dispose of all components that have less than 10 health
            foreach (ShipPart component in this.Components.ToList())
            {
                component.Health -= 5;

                if (component.Health <= 10)
                    component.Dispose();
                else if (component.IsRoot)
                {
                    component.SetVelocity(_sourceLinearVelocity, _sourceAngularVelocity);
                    component.ApplyForce(
                        Vector2.Transform(Vector2.UnitX * _strength,
                        Matrix.CreateRotationZ(
                            (Single)Math.Atan2(
                                component.Position.Y - _sourcePosition.Y,
                                component.Position.X - _sourcePosition.X))),
                        component.WorldCenteroid);
                }
            }
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.controller.TryUpdate(gameTime);
            _life += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_life > 5000)
            { // Each explosion must last at least 5 seconds
                this.Components.ForEach(c =>
                {
                    // If a component of the explosion is no longer moving, we can queue it up
                    // So it will be added back into the chunk
                    if (c.AngularVelocity == 0 && c.LinearVelocity.Length() == 0)
                    {
                        _still.Enqueue(c);
                    }  
                });

                // Add all entities into their respective chunks
                while (_still.Any())
                {
                    _target = _still.Dequeue();
                    _chunks.GetOrCreate(_target.Position.X, _target.Position.Y).TryAdd(_target);
                }


                // Auto remove the explosion when it is done
                if (!this.Components.Any())
                    this.Dispose();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.controller.TryDraw(gameTime);
        }
        #endregion

        #region Network Methods
        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            // Write the explosion source
            om.Write(this.Components.Count());
            this.Components.ForEach(c => om.Write(c.Id));
        }

        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            // Read the explosion source
            var components = im.ReadInt32();
            for (var i = 0; i < components; i++)
                this.controller.TryAdd(this.entities.GetById<ShipPart>(im.ReadGuid()));
        }
        #endregion
    }
}
