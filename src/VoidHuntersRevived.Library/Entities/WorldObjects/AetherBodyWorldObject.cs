using Guppy.EntityComponent.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network.Enums;
using Guppy.Threading.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Extensions.Aether;

namespace VoidHuntersRevived.Library.Entities.WorldObjects
{
    /// <summary>
    /// Implement basic scaffolding for a <see cref="IWorldObject"/> that bases it's
    /// world data off of a <see cref="AetherBody"/> instance.
    /// </summary>
    public abstract class AetherBodyWorldObject : WorldObject
    {
        #region Private Fields
        private Boolean _corpreal;
        #endregion

        #region Public Properties
        public override Vector2 Position => this.Body.LocalInstance.Position;

        public override Single Rotation => this.Body.LocalInstance.Rotation;

        public AetherBody Body { get; private set; }

        public Boolean Corporeal
        {
            get => _corpreal;
            set => this.OnCorporealChanged.InvokeIf(_corpreal != value, this, ref _corpreal, value);
        }
        #endregion

        #region Events
        public OnEventDelegate<AetherBodyWorldObject, Boolean> OnCorporealChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Body = provider.GetService<AetherWorld>().CreateBody((body, _, _) =>
            {
                body.Do(b =>
                {
                    b.Tag = this;
                    b.BodyType = BodyType.Dynamic;
                    b.LinearDamping = 0.2f;
                    b.AngularDamping = 0.5f;
                    b.IgnoreGravity = true;
                });
            });
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Body.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.Body.TryDraw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Body.TryUpdate(gameTime);
        }
        #endregion

        #region Helper Methods
        public override void SetTransform(Vector2 position, Single rotation)
        {
            this.Body.Instances[NetworkAuthorization.Master].SetTransformIgnoreContacts(ref position, rotation);
        }
        #endregion
    }
}
