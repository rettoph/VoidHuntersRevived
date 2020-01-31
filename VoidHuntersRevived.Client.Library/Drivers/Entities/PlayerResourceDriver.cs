using Guppy;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.UI.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    [IsDriver(typeof(Ship))]
    internal sealed class PlayerResourceDriver : Driver<Ship>
    {
        #region Private Fields
        private EntityCollection _entities;
        private ResourceBar _resourceBar;
        #endregion

        #region Constructor
        public PlayerResourceDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _entities = provider.GetRequiredService<EntityCollection>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _resourceBar = _entities.Create<ResourceBar>(r => r.Ship = this.driven);
        }

        protected override void Dispose()
        {
            base.Dispose();

            _resourceBar.Dispose();
        }
        #endregion
    }
}
