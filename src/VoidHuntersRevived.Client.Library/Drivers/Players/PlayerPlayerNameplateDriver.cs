using Guppy;
using Guppy.DependencyInjection;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Windows.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Windows.Library.Drivers.Players
{
    /// <summary>
    /// Simple driver that'll create a PlayerNameplate instance for
    /// every in game player.
    /// </summary>
    internal sealed class PlayerPlayerNameplateDriver : Driver<Player>
    {
        #region Private Fields
        private PlayerNameplate _nameplate;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Player driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            _nameplate = provider.GetService<EntityList>().Create<PlayerNameplate>((nameplate, p, c) =>
            {
                nameplate.player = this.driven;
            });
        }

        protected override void Release(Player driven)
        {
            base.Release(driven);

            _nameplate.TryRelease();

            _nameplate = null;
        }
        #endregion
    }
}
