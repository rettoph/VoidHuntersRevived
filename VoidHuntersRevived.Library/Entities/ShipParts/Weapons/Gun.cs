using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using Guppy;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
{
    /// <summary>
    /// Weapon primarily used to fire multiple projectiles
    /// </summary>
    public class Gun : Weapon
    {
        #region Private Attributes
        private Random _rand;
        private Byte[] _noise;
        #endregion

        #region Public Attributes
        public UInt32 FireCount { get; private set; }
        #endregion

        #region Constructors
        public Gun(Interval interval, World world) : base(interval, world)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.Events.Register<Projectile>("fired");
        }

        protected override void Initialize()
        {
            base.Initialize();

            _rand = new Random(this.Id.GetHashCode());
        }

        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
        }
        #endregion

        /// <summary>
        /// Create a new projectile
        /// </summary>
        public override void Fire()
        {
            var bullet = this.entities.Create<Projectile>("bullet", p =>
            {
                _noise = new Byte[16];
                _rand.NextBytes(_noise);
                p.SetId(new Guid(_noise));

                p.Weapon = this;
            });

            this.FireCount++;

            this.Events.TryInvoke<Projectile>(this, "fired", bullet);
        }
    }
}
