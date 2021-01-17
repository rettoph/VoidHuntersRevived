using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Builder.Services;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Builder.Scenes
{
    public class BuilderScene : GraphicsGameScene
    {
        #region Private Fields
        private ShipPartShapesBuilderService _builder;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.settings.Set<NetworkAuthorization>(NetworkAuthorization.Master);
            this.settings.Set<HostType>(HostType.Local);

            this.Entities.Create<WorldEntity>((w, p, c) =>
            { // Create an empty world
                w.Size = new Vector2(Chunk.Size, Chunk.Size);
            });
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.camera.ZoomTo(100f);
            this.camera.MoveTo((Vector2.One * Chunk.Size) / 2);

            provider.Service(out _builder);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _builder.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _builder.TryDraw(gameTime);
        }
        #endregion
    }
}