using Guppy;
using Guppy.DependencyInjection;
using Guppy.DependencyInjection.Descriptors;
using Guppy.Extensions.Utilities;
using Guppy.UI.Elements;
using Guppy.UI.Entities;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Builder.Contexts;
using VoidHuntersRevived.Builder.UI.Pages;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// The primary service used to create and edit the <see cref="ShipPartContext.InnerShapes"/>,
    /// <see cref="ShipPartContext.FemaleConnectionNodes"/> & <see cref="ShipPartContext.OuterHulls"/>
    /// values.
    /// </summary>
    public sealed class ShipPartShapesBuilderService : Frameable
    {
        #region Enums
        public enum ShipPartShapesBuilderStatus
        {
            None,
            BuildingShape
        }
        #endregion

        #region Private Fields
        private Stage _stage;
        private ShipPartShapesBuilderPage _page;
        private ShipPartShapeBuilderService _builder;
        private Camera2D _camera;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private Synchronizer _synchronizer;
        private ShipPartService _shipParts;
        private ShipPartRenderService _shipPartRenderService;

        private BasicEffect _effect;
        private ShipPartShapesBuilderStatus _status;

        private ShipPart _demo;
        private List<ShapeContext> _shapes;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service("stage:main", out _stage);
            provider.Service(out _builder);
            provider.Service(out _camera);
            provider.Service(out _graphics);
            provider.Service(out _spriteBatch);
            provider.Service(out _primitiveBatch);
            provider.Service(out _synchronizer);
            provider.Service(out _shipParts);
            provider.Service(out _shipPartRenderService);

            _shapes = new List<ShapeContext>();
            _effect = new BasicEffect(_graphics)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };
            
            // Create a brand new page for the defined main stage...
            _page = _stage.Content.Children.Create<ShipPartShapesBuilderPage>();
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _page.AddShapeButton.OnClicked += this.HandleAddShapeButtonClicked;
            _builder.OnShapeBuilt += this.HandleShapeBuilt;
        }

        protected override void Release()
        {
            base.Release();

            _page.AddShapeButton.OnClicked -= this.HandleAddShapeButtonClicked;
            _builder.OnShapeBuilt -= this.HandleShapeBuilt;

            _stage = null;
            _camera = null;
            _graphics = null;
            _spriteBatch = null;
            _primitiveBatch = null;
            _synchronizer = null;
            _shipParts = null;
            _shipPartRenderService = null;


            _shapes = null;
            _effect.Dispose();
            _page.TryRelease();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _shipPartRenderService.RemoveContext(_demo);
            _demo?.TryRelease();

            var context = new RigidShipPartContext("demo")
            {
                InnerShapes = _shapes.Select(s => new Vertices(s.GetVertices())).ToArray(),
                OuterHulls = _shapes.Select(s => new Vertices(s.GetVertices())).ToArray(),
                MaleConnectionNode = new ConnectionNodeContext(),
                FemaleConnectionNodes = new ConnectionNodeContext[0]
            };
            _demo = _shipParts.Create(context);
            _demo.Position = _camera.Position;

            _builder.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _effect.View = _camera.View;
            _effect.World = _camera.World;
            _effect.Projection = _camera.Projection;

            _spriteBatch.Begin(effect: _effect);
            _primitiveBatch.Begin(_camera);

            var width = 0.05f;
            foreach (Vector2 p in this.GetInterestPoints())
                _primitiveBatch.DrawRectangleF(Color.Red, new System.Drawing.RectangleF()
                {
                    Height = width,
                    Width = width,
                    X = _camera.Position.X + p.X - width / 2,
                    Y = _camera.Position.Y + p.Y - width / 2
                });

            _builder.TryDraw(gameTime);
            
            _primitiveBatch.End();
            _spriteBatch.End();
        }
        #endregion

        #region API Methods
        /// <summary>
        /// Returns all interest points reguarding the current shape.
        /// Interest points represent points where we believe you may want
        /// to attach nodes or take edges.
        /// </summary>
        /// <param name="blacklist"></param>
        /// <returns></returns>
        public IEnumerable<Vector2> GetInterestPoints(params ShapeContext[] blacklist)
        {
            if(_status == ShipPartShapesBuilderStatus.BuildingShape && !blacklist.Contains(_builder.Shape))
            { // Return all verticies within the shape so far!
                foreach (Vector2 p in _builder.Shape?.GetInterestPoints())
                    yield return p;
            }

            foreach (ShapeContext s in _shapes)
                if(!blacklist.Contains(s))
                    foreach(Vector2 p in s.GetInterestPoints())
                        yield return p;
        }

        /// <summary>
        /// Return the closest interest point within the specified
        /// range. If no interest points are found the default 
        /// position will be returned instead.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <param name="blacklist"></param>
        /// <returns></returns>
        public Vector2 TryGetClosestInterestPoint(Vector2 position, Single within = 0.25f, params ShapeContext[] blacklist)
        {
            var interests = this.GetInterestPoints(blacklist);
            if(interests.Any())
            {
                var interest = interests.OrderBy(v => Vector2.Distance(v, position)).First();
                if (Vector2.Distance(position, interest) <= within)
                    return interest;
            }

            return position;
        }
        #endregion

        #region Event handlers
        private void HandleAddShapeButtonClicked(Element sender)
        {
            if (_status != ShipPartShapesBuilderStatus.None)
                return;

            _page.SetMessage("Click anywhere in space to start shape.");
            _page.SetSubMessage("Shift: Unlock Rotation | Ctrl: Unlock Length | Space: Disable Snap");
            _builder.Start(_camera.Position);
            _status = ShipPartShapesBuilderStatus.BuildingShape;
        }

        private void HandleShapeBuilt(ShipPartShapeBuilderService sender, ShapeContext shape)
        {
            _shapes.Add(shape);
            _status = ShipPartShapesBuilderStatus.None;
        }
        #endregion
    }
}
