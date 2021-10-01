using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Threading.Utilities;
using Lidgren.Network;
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
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            // Setup the chain's body.
            this.Body = provider.GetService<AetherWorld>().CreateBody((body, _, _) =>
            {
                body.Do(b =>
                {
                    b.Tag = this;
                    b.BodyType = BodyType.Dynamic;
                    b.LinearDamping = 0.2f;
                    b.AngularDamping = 0.1f;
                    b.IgnoreGravity = true;
                });
            });
        }

        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);
        }


        protected override void Release()
        {
            base.Release();
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            this.Body.TryRelease();

            this.Body = default;
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

        #region Network Methods
        protected override void WriteWorldInfo(NetOutgoingMessage om)
        {
            this.Body.Instances[NetworkAuthorization.Master].WriteWorldInfo(om);
        }

        protected override void ReadWorldInfo(NetIncomingMessage im)
        {
            this.Body.Instances[NetworkAuthorization.Master].ReadWorldInfo(im);
        }
        #endregion
    }
}
