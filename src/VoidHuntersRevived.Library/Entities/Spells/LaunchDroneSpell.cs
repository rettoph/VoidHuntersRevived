using Guppy.DependencyInjection;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.Lists;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Entities.Spells
{
    public class LaunchDroneSpell : Spell
    {
        #region Private Fields
        private EntityList _entities;
        #endregion

        #region Public Properties
        public Vector2 Position { get; set; }
        public Single Rotation { get; set; }
        public Single MaxAge { get; set; }
        public String Type { get; set; }
        public Guid Team { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            provider.Service(out _entities);
        }

        protected override void Release()
        {
            base.Release();

            _entities = null;
        }
        #endregion
    }
}
