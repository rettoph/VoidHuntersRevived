using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities.Ammo;
using Guppy;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace GalacticFighters.Library.Entities.ShipParts.Weapons
{
    /// <summary>
    /// Weapon primarily used to fire multiple projectiles
    /// </summary>
    public class Gun : Weapon
    {
        #region Private Attributes
        private List<Projectile> _projectiles;
        private Queue<Projectile> _disposedBullets;
        private Random _rand;
        private Byte[] _noise;
        #endregion

        #region Public Attributes
        public IReadOnlyCollection<Projectile> Projectiles { get => _projectiles; }
        #endregion

        #region Constructors
        public Gun(World world) : base(world)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _projectiles = new List<Projectile>();
            _disposedBullets = new Queue<Projectile>();

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

            _projectiles.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update all internal bullets
            _projectiles.ForEach(b => b.TryUpdate(gameTime));

            // Clear all disposed bullets...
            while (_disposedBullets.Any())
                _projectiles.Remove(_disposedBullets.Dequeue());
        }
        #endregion

        /// <summary>
        /// Create a new projectile
        /// </summary>
        protected override void Fire()
        {
            var bullet = this.entities.Create<Projectile>("bullet", p =>
            {
                _noise = new Byte[16];
                _rand.NextBytes(_noise);
                p.SetId(new Guid(_noise));

                p.Gun = this;
                
                p.Events.TryAdd<Creatable>("disposing", (s, c) => _disposedBullets.Enqueue(c as Projectile));
            });
            _projectiles.Add(bullet);

            this.Events.TryInvoke<Projectile>(this, "fired", bullet);
        }
    }
}
