using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    /// <summary>
    /// Custom controller designed to contain
    /// a single chain, auto dispose of the root,
    /// and smoothly handle the transfer of remaining
    /// ShipParts back into the chunk.
    /// </summary>
    public class Explosion : SimpleController
    {
        #region Private Fields
        private ShipPart _component;
        private Vector2 _sourcePosition;
        private Vector2 _sourceVelocity;
        private Queue<FarseerEntity> _asleep;
        private ChunkCollection _chunks;
        #endregion

        #region Public Properties
        public Single Strength { get; set; } = 5f;
        #endregion

        #region Constructor
        public Explosion(ChunkCollection chunks)
        {
            _chunks = chunks;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _asleep = new Queue<FarseerEntity>();
        }

        public override void Dispose()
        {
            base.Dispose();

            _asleep.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (FarseerEntity entity in this.Components)
            {
                entity.TryUpdate(gameTime);

                if (!entity.Body.Awake)
                    _asleep.Enqueue(entity);
            }

            while (_asleep.Any()) // Add any sleeping entites back into the chunks
                _chunks.AddToChunk(_asleep.Dequeue());
        }
        

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.Components.TryDrawAll(gameTime);
        }
        #endregion

        #region Farseer Setup Methods
        public override void SetupBody(FarseerEntity component, Body body)
        {
            base.SetupBody(component, body);

            body.CollisionCategories = Categories.PassiveCollisionCategories;
            body.CollidesWith = Categories.PassiveCollidesWith;
            body.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;
            body.BodyType = BodyType.Dynamic;

            var offset = body.Position - _sourcePosition;
            body.ApplyForce(
                force: _sourceVelocity + Vector2.Transform(Vector2.UnitX * this.Strength, Matrix.CreateRotationZ((Single)Math.Atan2(offset.Y, offset.X))),
                point: _sourcePosition);
        }

        public override void UpdateBody(FarseerEntity component, Body body)
        {
            base.UpdateBody(component, body);
        }
        #endregion

        #region Helper Methods
        public override bool Add(FarseerEntity entity)
        {
            if(entity is ShipPart && base.Add(entity))
            {
                _component = entity as ShipPart;

                if(_component.IsRoot)
                { // If this is the root component, set the explosion source values....
                    _sourcePosition = _component.Position;
                    _sourceVelocity = _component.LinearVelocity;
                }

                return true;
            }

            return false;
        }

        public override bool Remove(FarseerEntity entity)
        {
            return base.Remove(entity);
        }
        #endregion
    }
}
