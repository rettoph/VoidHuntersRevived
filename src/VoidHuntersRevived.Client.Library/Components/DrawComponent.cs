using Guppy;
using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Interfaces;
using Guppy.Interfaces;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Components
{
    public class DrawComponent<TEntity, TVertexType> : Component<TEntity>
        where TEntity : class, IEntity
        where TVertexType : struct, IVertexType
    {
        #region Private Fields
        private PrimitiveBatch<TVertexType> _primitiveBatch;
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        #endregion

        #region Protected Properties
        protected PrimitiveBatch<TVertexType> primitiveBatch => _primitiveBatch;
        protected SpriteBatch spriteBatch => _spriteBatch;
        protected Camera2D camera => _camera;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _primitiveBatch);
            provider.Service(out _spriteBatch);
            provider.Service(out _camera);
        }
        #endregion
    }

    public class DrawComponent<TEntity> : DrawComponent<TEntity, VertexPositionColor>
        where TEntity : class, IEntity
    {

    }
}
