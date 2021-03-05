using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class ExplosionMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<Explosion>
    {
        #region Lifecycle Methods
        protected override void Initialize(Explosion driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            this.driven.OnPostUpdate += this.PostUpdate;
        }

        protected override void Release(Explosion driven)
        {
            base.Release(driven);

            this.driven.OnPostUpdate -= this.PostUpdate;
        }


        protected override void InitializeRemote(Explosion driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.MessageHandlers[MessageType.Setup].OnWrite += this.driven.WriteContext;
        }

        protected override void ReleaseRemote(Explosion driven)
        {
            base.ReleaseRemote(driven);

            this.driven.MessageHandlers[MessageType.Setup].OnWrite -= this.driven.WriteContext;
        }
        #endregion

        #region Frame Methods
        private void PostUpdate(GameTime gameTime)
        {
            if (this.driven.Age >= this.driven.Context.MaxAge)
                this.driven.TryRelease(); // The explosion has reached its end of life.
        }
        #endregion
    }
}
