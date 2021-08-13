using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Components
{
    public class FrameableDrawComponent<TFrameable, TVertexType> : DrawComponent<TFrameable, TVertexType>
        where TFrameable : class, IFrameable
        where TVertexType : struct, IVertexType
    {
        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.OnDraw += this.Draw;
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            this.Entity.OnDraw -= this.Draw;
        }
        #endregion

        #region Frame Methods
        protected virtual void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }
        #endregion
    }

    public class FrameableDrawComponent<TFrameable> : FrameableDrawComponent<TFrameable, VertexPositionColor>
        where TFrameable : class, IFrameable
    {

    }
}
