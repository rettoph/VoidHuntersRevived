using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Client.Library.Drivers.Players
{
    internal sealed class LocalUserPlayerDriver : Driver<UserPlayer>
    {
        #region Lifecycle Methods
        protected override void Initialize(UserPlayer driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);
        }
        #endregion
    }
}
