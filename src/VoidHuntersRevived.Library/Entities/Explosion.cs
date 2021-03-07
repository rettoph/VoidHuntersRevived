using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using Guppy.Network.Extensions.Lidgren;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Collision;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// The primary exlosion class.
    /// </summary>
    public class Explosion : NetworkEntity
    {
        #region Private Fields
        private WorldEntity _world;
        private AABB _aabb;
        private HashSet<Guid> _targets;
        #endregion

        #region Public Properties
        public ExplosionContext Context { get; internal set; }

        /// <summary>
        /// The current age in seconds.
        /// </summary>
        public Single Age { get; set; } = 2f;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _world);

            _targets = new HashSet<Guid>();

            this.Age = 0;

            this.LayerGroup = VHR.LayersContexts.Explosion.Group.GetValue();
        }
        #endregion

        #region Lifecycle Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Age += (Single)gameTime.ElapsedGameTime.TotalSeconds;

            // Manage explosion effects...
            var agePercent = this.Age / this.Context.MaxAge;
            var radius = this.Context.MaxRadius * agePercent;

            _aabb.LowerBound = this.Context.StartPosition - new Vector2(radius);
            _aabb.UpperBound = this.Context.StartPosition + new Vector2(radius);

            _targets.Clear();
            _world.live.QueryAABB(fixture =>
            {
                if(fixture.Tag is ShipPart target && !_targets.Contains(target.Id))
                {
                    _targets.Add(target.Id);

                    float distance = Vector2.Distance(this.Context.StartPosition, target.Position);
                    float forcePercent = this.GetPercent(distance, radius);

                    Vector2 forceVector = this.Context.StartPosition - target.Position;

                    if (distance > 0)
                        forceVector *= 1f / Math.Max((float)Math.Sqrt(forceVector.X * forceVector.X + forceVector.Y * forceVector.Y), Single.Epsilon);
                    forceVector *= this.Context.MaxForcePerSecond * forcePercent;
                    forceVector *= 1 - agePercent;
                    forceVector *= -1;
                    forceVector *= (Single)gameTime.ElapsedGameTime.TotalSeconds;

                    target.ApplyForce(forceVector, this.Context.StartPosition);
                    target.TryApplyDamage(this.Context.MaxDamagePerSecond * forcePercent * (1 - agePercent) * (Single)gameTime.ElapsedGameTime.TotalSeconds);
                }
                return true;
            }, ref _aabb);
        }
        #endregion

        #region Helper Methods
        private float GetPercent(float distance, float radius)
        {
            //(1-(distance/radius))^power-1
            float percent = (float)Math.Pow(1 - ((distance - radius) / radius), 1) - 1;

            if (float.IsNaN(percent))
                return 0f;

            return MathUtils.Clamp(percent, 0f, 1f);
        }
        #endregion

        #region Network Methods
        internal void WriteContext(NetOutgoingMessage om)
        {
            om.Write(this.Context.StartPosition);
            om.Write(this.Context.StartVelocity);
            om.Write(this.Context.Color);
            om.Write(this.Context.MaxRadius);
            om.Write(this.Context.MaxForcePerSecond);
            om.Write(this.Context.MaxDamagePerSecond);
            om.Write(this.Context.MaxAge);
        }

        internal void ReadContext(NetIncomingMessage im)
        {
            this.Context = new ExplosionContext()
            {
                StartPosition = im.ReadVector2(),
                StartVelocity = im.ReadVector2(),
                Color = im.ReadColor(),
                MaxRadius = im.ReadSingle(),
                MaxForcePerSecond = im.ReadSingle(),
                MaxDamagePerSecond = im.ReadSingle(),
                MaxAge = im.ReadSingle()
            };
        }
        #endregion
    }
}
