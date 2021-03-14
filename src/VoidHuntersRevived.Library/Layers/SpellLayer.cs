using Guppy.DependencyInjection;
using Guppy.Lists;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Layers
{
    public class SpellLayer : GameLayer
    {
        #region Private Fields
        private OrderableList<Spell> _spells;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _spells);
        }

        protected override void Release()
        {
            base.Release();

            _spells = null;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _spells.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spells.TryDraw(gameTime);
        }
        #endregion
    }
}
