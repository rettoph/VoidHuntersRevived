using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.Collections;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    public sealed class WorldEntity : FarseerEntity<World>
    {
        #region Static Attributes
        public static Int32 WallWidth { get; } = 10;
        #endregion

        #region Private Fields
        private Vector2 _size;
        private Queue<Body> _walls;
        #endregion

        #region Public Attributes
        public World Master => this.master;
        public World Slave => this.slave;
        public World Live => this.live;

        public Vector2 Size
        {
            get => _size;
            set => this.OnSizeChanged.InvokeIfChanged(_size != value, this, ref _size, value);
        }
        #endregion

        #region Events
        public event OnEventDelegate<WorldEntity, Vector2> OnSizeChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _walls = new Queue<Body>();

            this.OnSizeChanged += this.HandleSizeChanged;

            this.Size = new Vector2(128, 128);

            this.UpdateOrder = -100;
        }

        protected override void Release()
        {
            base.Release();

            this.OnSizeChanged -= this.HandleSizeChanged;
        }
        #endregion

        #region Factory Methods
        protected override World BuildMaster(ServiceProvider provider)
            => new World(Vector2.Zero);

        protected override World BuildSlave(ServiceProvider provider)
            => new World(Vector2.Zero);
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Do(w =>
            {
                w.Step((Single)gameTime.ElapsedGameTime.TotalSeconds);
            });
        }
        #endregion

        #region Event Handlers
        private void HandleSizeChanged(WorldEntity sender, Vector2 arg)
        {
            this.log.Info($"World Size Changed => {this.Size}");

            // Destroy any pre-existing walls...
            while (_walls.Any())
                _walls.Dequeue().Dispose();

            // Create brand new walls...
            this.Do(w => _walls.Enqueue(BodyFactory.CreateRectangle(w, WorldEntity.WallWidth, this.Size.Y + (WorldEntity.WallWidth * 2), 0f, new Vector2(-WorldEntity.WallWidth / 2, this.Size.Y / 2), 0, BodyType.Static)));
            this.Do(w => _walls.Enqueue(BodyFactory.CreateRectangle(w, WorldEntity.WallWidth, this.Size.Y + (WorldEntity.WallWidth * 2), 0f, new Vector2(this.Size.X + (WorldEntity.WallWidth / 2), this.Size.Y / 2), 0, BodyType.Static)));
            this.Do(w => _walls.Enqueue(BodyFactory.CreateRectangle(w, this.Size.X, WorldEntity.WallWidth, 0f, new Vector2(this.Size.X / 2, -WorldEntity.WallWidth / 2), 0, BodyType.Static)));
            this.Do(w => _walls.Enqueue(BodyFactory.CreateRectangle(w, this.Size.X, WorldEntity.WallWidth, 0f, new Vector2(this.Size.X / 2, this.Size.Y + (WorldEntity.WallWidth / 2)), 0, BodyType.Static)));
            
            _walls.ForEach(b =>
            { // Setup wall collisions
                b.CollisionCategories = Categories.BorderCollisionCategories;
                b.CollidesWith = Categories.BorderCollidesWith;
                b.IgnoreCCDWith = Categories.BorderIgnoreCCDWith;

                b.Restitution = 1f;
                b.Friction = 0f;
            });
        }
        #endregion
    }
}
