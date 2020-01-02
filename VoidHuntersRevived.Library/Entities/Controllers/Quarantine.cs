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

        private Dictionary<Guid, Quarantined> _quarantinees;
        private Queue<FarseerEntity> _clean;
        private FarseerEntity _entity;
        private ActionTimer _cleanTimer;

        internal ChunkCollection chunks { get; set; }

        #region Constructor
        public Quarantine()
        {
            _quarantinees = new Dictionary<Guid, Quarantined>();
            _clean = new Queue<FarseerEntity>();
            _cleanTimer = new ActionTimer(1000);
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.OnAdded += this.HandleEntityAdded;
            this.OnRemoved += this.HandleEntityRemoved;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.OnAdded -= this.HandleEntityAdded;
            this.OnRemoved -= this.HandleEntityRemoved;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update each quarantinee...
            _quarantinees.Values.ForEach(q =>
            {
                // Update the quarentinee
                q.Update(gameTime);

                if (q.Clean)
                    _clean.Enqueue(q.Component);
            });

            // Remove all clean entitys & add them directly into their chunk...
            _cleanTimer.Update(
                gameTime: gameTime,
                filter: triggered => triggered && _clean.Any(),
                action: () =>
                {
                    while (_clean.Any())
                        this.chunks.Get((_entity = _clean.Dequeue())).Add(_entity);
                });
            
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

        #region Event Handlers
        private void HandleEntityAdded(Object sender, FarseerEntity entity)
        {
            _quarantinees.Add(entity.Id, new Quarantined(entity));
        }

        private void HandleEntityRemoved(Object sender, FarseerEntity entity)
        {
            _quarantinees.Remove(entity.Id);
        }
        #endregion
    }
}
