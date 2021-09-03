using Guppy.DependencyInjection;
using Guppy.Network.Enums;
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
        #region Public Properties
        public override Vector2 Position => this.Body.LocalInstance.Position;

        public override Single Rotation => this.Body.LocalInstance.Rotation;

        public AetherBody Body { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            // Setup the chain's body.
            this.Body = provider.GetService<AetherWorld>().CreateBody((body, _, _) =>
            {
                body.Tag = this;
                body.BodyType = BodyType.Dynamic;
                body.LinearDamping = 0.2f;
                body.AngularDamping = 0.1f;
                body.IgnoreGravity = true;
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
