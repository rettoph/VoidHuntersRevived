using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    /// <summary>
    /// The world entity driver primarily responsible for handling
    /// network messaging across all game authorization methods.
    /// </summary>
    internal sealed class WorldEntityNetworkDriver : NetworkEntityNetworkDriver<WorldEntity>
    {
        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            this.driven.Actions.Set("update:size", this.ReadSize);
            this.AddAction("update:size", this.SkipSize, (GameAuthorization.Minimum, this.ReadSize));
        }

        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            this.driven.OnWrite += this.WriteSize;
            this.driven.OnSizeChanged += this.HandleSizeChanged;
        }

        protected override void ReleaseFull()
        {
            base.ReleaseFull();

            this.driven.OnWrite -= this.WriteSize;
            this.driven.OnSizeChanged -= this.HandleSizeChanged;
        }
        #endregion

        #region Message Handlers
        private void WriteSize(NetOutgoingMessage om)
            => om.Write("update:size", m =>
            {
                m.Write(this.driven.Size);
            });

        private void ReadSize(NetIncomingMessage im)
            => this.driven.Size = im.ReadVector2();

        private void SkipSize(NetIncomingMessage im)
            => im.Position += 64;
        #endregion

        #region Event Handlers
        private void HandleSizeChanged(WorldEntity sender, Vector2 arg)
            => this.WriteSize(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 2));
        #endregion
    }
}
