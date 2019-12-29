using FarseerPhysics.Dynamics;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    /// <summary>
    /// The Quarantine represents a controller
    /// that ship parts should be added to 
    /// if they are still moving when they
    /// are added into a chunk.
    /// 
    /// The quarentine will place a hold on
    /// the component for a minimum amount of
    /// time then return add item into its proper
    /// chunk if it is no longer moving.
    /// </summary>
    public sealed class Quarantine : Controller
    {
        private static Double CleanTime { get; set; } = 3000;

        private class Quarantined
        {
            public readonly FarseerEntity Component;
            public Double? Timestamp { get; private set; }
            public Boolean Clean { get; private set; }

            public Quarantined(FarseerEntity component)
            {
                this.Component = component;
                this.Clean = false;
            }

            public void Update(GameTime gameTime)
            {
                this.Component.TryUpdate(gameTime);

                if (this.Timestamp == null || this.Component.IsMoving)
                    this.Timestamp = gameTime.TotalGameTime.TotalMilliseconds;

                this.Clean = gameTime.TotalGameTime.TotalMilliseconds - this.Timestamp > Quarantine.CleanTime;
            }
        }

        private struct BufferAction
        {
            public Boolean Add;
            public FarseerEntity Entity;
        }

        private Dictionary<Guid, Quarantined> _quarantinees;
        private Queue<FarseerEntity> _clean;
        private Queue<BufferAction> _actions;
        private FarseerEntity _entity;
        private BufferAction _action;

        internal ChunkCollection chunks { get; set; }

        #region Constructor
        public Quarantine()
        {
            _quarantinees = new Dictionary<Guid, Quarantined>();
            _clean = new Queue<FarseerEntity>();
            _actions = new Queue<BufferAction>();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_actions.Any())
                if ((_action = _actions.Dequeue()).Add)
                    _quarantinees.Add(_action.Entity.Id, new Quarantined(_action.Entity));
                else
                    _quarantinees.Remove(_action.Entity.Id);

            // Update each quarantinee...
            _quarantinees.Values.ForEach(q =>
            {
                // Update the quarentinee
                q.Update(gameTime);

                if (q.Clean)
                    _clean.Enqueue(q.Component);
            });

            // Remove all clean entitys & add them directly into their chunk...
            while (_clean.Any())
                this.chunks.Get((_entity = _clean.Dequeue())).Add(_entity);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.Components.ForEach(c => c.TryDraw(gameTime));
        }
        #endregion

        #region Setup Methods
        /// <inheritdoc />
        public override void SetupBody(FarseerEntity component, Body body)
        {
            base.SetupBody(component, body);

            // Stop all body movement
            body.SetTransformIgnoreContacts(component.Position, component.Rotation);
            body.LinearVelocity = Vector2.Zero;
            body.AngularVelocity = 0;
            body.CollisionCategories = Categories.PassiveCollisionCategories;
            body.CollidesWith = Categories.PassiveCollidesWith;
            body.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;
            body.BodyType = BodyType.Dynamic;
        }
        #endregion

        #region Helper Methods
        public override bool Add(FarseerEntity entity)
        {
            if (base.Add(entity))
            {
                // _quarantinees.Add(entity.Id, new Quarantined(entity));
                _actions.Enqueue(new BufferAction()
                {
                    Add = true,
                    Entity = entity
                });

                return true;
            }

            return false;
        }

        public override bool Remove(FarseerEntity entity)
        {
            if (base.Remove(entity))
            {
                // _quarantinees.Remove(entity.Id);
                _actions.Enqueue(new BufferAction()
                {
                    Add = false,
                    Entity = entity
                });

                return true;
            }

            return false;
        }
        #endregion

    }
}
