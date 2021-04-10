using Guppy.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Builder.Services;
using VoidHuntersRevived.Builder.UI.Pages;
using VoidHuntersRevived.Windows.Library.Scenes;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Enums;
using Guppy.Extensions.System;
using Guppy.Extensions.System.Reflection;
using VoidHuntersRevived.Library.Attributes;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Guppy.UI.Elements;
using VoidHuntersRevived.Builder.Attributes;

using ServiceProvider = Guppy.DependencyInjection.ServiceProvider;
using System.IO;
using VoidHuntersRevived.Library.Services;

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
        private ShipPartContextSelectorPage _contextSelectorPage;

        /// <summary>
        /// A list of all services to pageinate through when constructing
        /// a new context
        /// </summary>
        private List<ShipPartContextBuilderService> _services;
        private Int32 _serviceIndex;
        private ShipPartContextBuilderService _service;

        private Synchronizer _synchronizer;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _provider = provider;

            provider.Service(out _synchronizer);

            this.settings.Set<NetworkAuthorization>(NetworkAuthorization.Master);
            this.settings.Set<HostType>(HostType.Local);

            this.Entities.Create<WorldEntity>((w, p, c) =>
            { // Create an empty world
                w.Size = new Vector2(Chunk.Size, Chunk.Size);
            });

            _contextSelectorPage = this.stage.Content.Children.Create<ShipPartContextSelectorPage>();
            _builderPage = this.stage.Content.Children.Create<ShipPartContextBuilderPage>();

            _contextSelectorPage.OnContextSelected += this.HandleContextSelected;
            _builderPage.NextButton.OnClicked += this.HandleNextButtonClicked;
            _builderPage.PrevButton.OnClicked += this.HandlePrevButtonClicked;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.camera.ZoomTo(100f);
            this.camera.MoveTo((Vector2.One * Chunk.Size) / 2);

            // AUTO SAVE ALL CONTEXTS
            // var shipParts = provider.GetService<ShipPartService>();
            // foreach (ShipPartContext context in shipParts.Contexts.Values)
            //     context.Export($"Resources\\ShipParts\\{String.Join(".", context.Name.Split(Path.GetInvalidFileNameChars()))}.vhsp");
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _service?.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _service?.TryDraw(gameTime);
        }
        #endregion

        #region Helper Methods
        private void OpenService(Int32 delta)
        {
            _serviceIndex += delta;

            _service?.Close();
            if (_serviceIndex < 0)
            { // We want to open the ContextType selector again.
                if (_services != default)
                    _services.ForEach(s => s.TryRelease());

                this.stage.Content.Open(_contextSelectorPage);
            }
            else if(_serviceIndex >= _services.Count)
            { // Save & export...
                _context.Export($"Resources\\ShipParts\\{String.Join(".", _context.Name.Split(Path.GetInvalidFileNameChars()))}.vhsp");
                this.OpenService(-(_serviceIndex + 1));
            }
            else
            {
                _service = _services[_serviceIndex];
                _service.Open(_context);
            }

            // Ensure that the navigation buttons are updated.
            if (_serviceIndex <= 0)
                _builderPage.PrevButton.Value = $"< Context Types";
            else
                _builderPage.PrevButton.Value = $"< {_services[_serviceIndex - 1].GetType().GetAttribute<ShipPartContextBuilderServiceAttribute>().Title}";

            if (_serviceIndex >= _services.Count - 1)
                _builderPage.NextButton.Value = $"Save & Export >";
            else
                _builderPage.NextButton.Value = $"{_services[_serviceIndex + 1].GetType().GetAttribute<ShipPartContextBuilderServiceAttribute>().Title} >";
        }
        #endregion

        #region Event Handlers
        private void HandleContextSelected(ShipPartContextSelectorPage sender, ShipPartContext context)
        {
            _serviceIndex = 0;
            _context = context;
            _services = AssemblyHelper.Types.GetTypesWithAttribute<ShipPartContextBuilderService, ShipPartContextBuilderServiceAttribute>()
                .OrderBy(t => t.GetAttribute<ShipPartContextBuilderServiceAttribute>().Order)
                .Select(t => _provider.GetService(t) as ShipPartContextBuilderService)
                .Where(s => s != default)
                .ToList();

            this.stage.Content.Open(_builderPage);

            this.OpenService(0);
        }

        private void HandleNextButtonClicked(Element sender)
            => _synchronizer.Enqueue(gt => this.OpenService(1));

        private void HandlePrevButtonClicked(Element sender)
            => _synchronizer.Enqueue(gt => this.OpenService(-1));
        #endregion
    }
}