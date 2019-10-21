using Guppy;
using Guppy.Extensions.Collection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Utilities.Controllers
{
    /// <summary>
    /// Managed controllers will automatically return their components
    /// into the approproate chunk on removal
    /// </summary>
    /// <typeparam name="TControlled"></typeparam>
    public class FarseerEntityController<TControlled> : Controller<TControlled>
        where TControlled : FarseerEntity
    {
        private ChunkCollection _chunks;

        public FarseerEntityController(ChunkCollection chunks)
        {
            _chunks = chunks;
        }

        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.CollidesWith = Categories.PassiveCollidesWith;
            this.CollisionCategories = Categories.PassiveCollisionCategories;
            this.IgnoreCCDWith = Categories.PassiveIgnoreCCDWith;
        }

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.Components.ForEach(e => e.TryDraw(gameTime));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Components.ForEach(e => e.TryUpdate(gameTime));
        }
        #endregion
    }
}
