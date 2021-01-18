using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Builder.Services;
using VoidHuntersRevived.Builder.UI.Pages;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Enums;
using Guppy.Extensions.System;
using VoidHuntersRevived.Library.Attributes;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ServiceProvider = Guppy.DependencyInjection.ServiceProvider;
using Guppy.UI.Elements;

namespace VoidHuntersRevived.Builder.Scenes
{
    public class BuilderScene : GraphicsGameScene
    {
        #region Private Fields
        private ServiceProvider _provider;

        /// <summary>
        /// The primary context instance currently being constructed.
        /// </summary>
        private ShipPartContext _context;

        /// <summary>
        /// The API referenceable page to insert 
        /// a builder services UI. Only visible once
        /// a context type has been selected.
        /// </summary>
        private ShipPartContextBuilderPage _builderPage;

        /// <summary>
        /// The hidden page to render when a user needs to select a new
        /// context type.
        /// </summary>
        private ShipPartContextTypeSelectorPage _contextSelectorPage;

        /// <summary>
        /// A list of all services to pageinate through when constructing
        /// a new context
        /// </summary>
        private List<ShipPartContextBuilderService> _services;

        private Int32 _serviceIndex;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _provider = provider;

            this.settings.Set<NetworkAuthorization>(NetworkAuthorization.Master);
            this.settings.Set<HostType>(HostType.Local);

            this.Entities.Create<WorldEntity>((w, p, c) =>
            { // Create an empty world
                w.Size = new Vector2(Chunk.Size, Chunk.Size);
            });

            _contextSelectorPage = this.stage.Content.Children.Create<ShipPartContextTypeSelectorPage>();
            _builderPage = this.stage.Content.Children.Create<ShipPartContextBuilderPage>();

            _contextSelectorPage.OnContextTypeSelected += this.HandleContextTypeSelected;
            _builderPage.NextButton.OnClicked += this.HandleNextButtonClicked;
            _builderPage.PrevButton.OnClicked += this.HandlePrevButtonClicked;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.camera.ZoomTo(100f);
            this.camera.MoveTo((Vector2.One * Chunk.Size) / 2);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion

        #region Helper Methods
        private void OpenService(Int32 delta)
        {
            _serviceIndex += delta;

            if(_serviceIndex < 0)
            { // We want to open the ContextType selector again.
                this.stage.Content.Open(_contextSelectorPage);
            }

            // Ensure that the navigation buttons are updated.
            if (_serviceIndex <= 0)
                _builderPage.PrevButton.Value = $"< ContextType";
            else
                _builderPage.PrevButton.Value = $"< {_services[_serviceIndex - 1].GetType().Name}";

            if (_serviceIndex >= _services.Count - 1)
                _builderPage.NextButton.Value = $"Save >";
            else
                _builderPage.NextButton.Value = $"{_services[_serviceIndex + 1].GetType().Name} >";
        }
        #endregion

        #region Event Handlers
        private void HandleContextTypeSelected(ShipPartContextTypeSelectorPage sender, Type contextType)
        {
            _serviceIndex = 0;
            _context = ActivatorUtilities.CreateInstance(_provider, contextType, "demo") as ShipPartContext;
            _services = AssemblyHelper.Types.GetTypesAssignableFrom<ShipPartContextBuilderService>()
                .Select(t => _provider.GetService(t) as ShipPartContextBuilderService)
                .Where(s => s != default)
                .ToList();

            this.stage.Content.Open(_builderPage);

            this.OpenService(0);
        }

        private void HandleNextButtonClicked(Element sender)
            => this.OpenService(1);

        private void HandlePrevButtonClicked(Element sender)
            => this.OpenService(-1);
        #endregion
    }
}