using Guppy.EntityComponent.DependencyInjection;
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
        private Boolean _visible;
        public Boolean Visible {
            get => _visible;
            set
            {
                if(_visible == value)
                {
                    return;
                }

                if(_visible = value)
                {
                    this.Entity.OnDraw += this.Draw;
                }
                else
                {
                    this.Entity.OnDraw -= this.Draw;
                }
            }
        }
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Visible = true;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Visible = false;
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
