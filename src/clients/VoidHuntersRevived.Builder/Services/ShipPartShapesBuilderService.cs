﻿using Guppy;
using Guppy.DependencyInjection;
using Guppy.DependencyInjection.Descriptors;
using Guppy.Extensions.Utilities;
using Guppy.IO.Input;
using Guppy.IO.Services;
using Guppy.UI.Elements;
using Guppy.UI.Entities;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Builder.Contexts;
using VoidHuntersRevived.Builder.UI.Pages;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// The primary service used to create and edit the <see cref="ShipPartContext.InnerShapes"/>,
    /// <see cref="ShipPartContext.FemaleConnectionNodes"/> & <see cref="ShipPartContext.OuterHulls"/>
    /// values.
    /// </summary>
    public class ShipPartShapesBuilderService : ShipPartContextBuilderService
    {
        #region Enums
        public enum ShipPartShapesBuilderStatus
        {
            None,
            BuildingShape,
            EditingShape,
            DraggingShape
        }
        #endregion

        #region Private Fields
        private ShipPartShapesBuilderPage _page;
        private ShipPartShapeBuilderService _builder;
        private Camera2D _camera;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private Synchronizer _synchronizer;
        private ShipPartService _shipParts;
        private ShipPartRenderService _shipPartRenderService;
        private MouseService _mouse;
        private WorldEntity _world;

        private BasicEffect _effect;
        private ShipPartShapesBuilderStatus _status;

        private ShipPart _demo;
        private List<ShapeContext> _shapes;
        private ShapeContext _editShape;
        private Vector2 _dragStartMouseWorldPosition;
        private Vector2 _dragStartTranslation;
        #endregion

        #region Protected Properties
        protected Vector2 mouseWorldPosition => _camera.Unproject(_mouse.Position);
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _camera);
            provider.Service(out _graphics);
            provider.Service(out _spriteBatch);
            provider.Service(out _primitiveBatch);
            provider.Service(out _synchronizer);
            provider.Service(out _shipParts);
            provider.Service(out _shipPartRenderService);
            provider.Service(out _mouse);
            provider.Service(out _world);
            provider.Service(out _builder, (b, p, c) => b.shapes = this);

            _shapes = new List<ShapeContext>();
            _effect = new BasicEffect(_graphics)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };
            
            // Create a brand new page for the defined main stage...
            _page = this.pages.Children.Create<ShipPartShapesBuilderPage>();
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            _page.AddShapeButton.OnClicked += this.HandleAddShapeButtonClicked;
            _builder.OnShapeBuilt += this.HandleShapeBuilt;
            _mouse.OnButtonStateChanged += this.HandleMouseButtonStateChanged;
        }

        protected override void Release()
        {
            base.Release();

            _page.AddShapeButton.OnClicked -= this.HandleAddShapeButtonClicked;
            _builder.OnShapeBuilt -= this.HandleShapeBuilt;
            _mouse.OnButtonStateChanged -= this.HandleMouseButtonStateChanged;

            _camera = null;
            _graphics = null;
            _spriteBatch = null;
            _primitiveBatch = null;
            _synchronizer = null;
            _shipParts = null;
            _shipPartRenderService = null;
            _mouse = null;

            _shapes = null;
            _effect.Dispose();
            _page.TryRelease();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_status == ShipPartShapesBuilderStatus.DraggingShape)
            { // We want to transform the current _editShape based on the mouse changes.
                _editShape.Translation = _dragStartTranslation - _dragStartMouseWorldPosition + this.mouseWorldPosition;

                var editPoints = _editShape.GetInterestPoints();
                var targetPoints = this.GetInterestPoints(_editShape);
                (Vector2 edit, Vector2 target, Single distance) nearest = (Vector2.Zero, Vector2.Zero, Single.MaxValue);

                foreach(Vector2 edit in editPoints)
                {
                    foreach(Vector2 target in targetPoints)
                    {
                        if(Vector2.Distance(edit, target) < nearest.distance)
                        {
                            nearest.edit = edit;
                            nearest.target = target;
                            nearest.distance = Vector2.Distance(edit, target);
                        }
                    }
                }

                if(nearest.distance < 0.25f)
                {
                    _editShape.Translation += nearest.target - nearest.edit;
                }
            }

            _shipPartRenderService.RemoveContext(_demo);
            _demo?.TryRelease();

            this.context.InnerShapes = _shapes.Select(s => new Vertices(s.GetVertices())).ToArray();
            this.context.OuterHulls = _shapes.Select(s => new Vertices(s.GetVertices())).ToArray();
            this.context.MaleConnectionNode = new ConnectionNodeContext();
            this.context.FemaleConnectionNodes = new ConnectionNodeContext[0];

            _demo = _shipParts.Create(this.context);
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

        /// <summary>
        /// Check to see if there is a shape under the current
        /// mouse position. If so, set the value to <see cref="_editShape"/>
        /// and return true. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        private Boolean TryUpdateEditShape()
        {
            var fixture = _world.Live.TestPoint(this.mouseWorldPosition);

            if (fixture == default)
                return false;

            _editShape = _shapes[fixture.Body.FixtureList.IndexOf(fixture)];
            return true;
        }

        protected internal override void Open(ShipPartContext context)
        {
            base.Open(context);

            // Open the default API page...
            this.pages.Open(_page);
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

        private void HandleMouseButtonStateChanged(InputManager sender, InputArgs args)
        {
            _synchronizer.Enqueue(gt =>
            {
                switch (_status)
                {
                    case ShipPartShapesBuilderStatus.None:
                    case ShipPartShapesBuilderStatus.EditingShape:
                        if (args.State == ButtonState.Pressed && this.TryUpdateEditShape())
                        { // There is a shape being selected!
                            _status = ShipPartShapesBuilderStatus.DraggingShape;
                            _dragStartMouseWorldPosition = this.mouseWorldPosition;
                            _dragStartTranslation = _editShape.Translation;
                        }
                        else
                            _status = ShipPartShapesBuilderStatus.None;
                        break;
                    case ShipPartShapesBuilderStatus.DraggingShape:
                        if (args.State == ButtonState.Released && this.TryUpdateEditShape())
                        { // There is a shape being selected!
                            _status = ShipPartShapesBuilderStatus.EditingShape;
                        }
                        else
                            _status = ShipPartShapesBuilderStatus.None;
                        break;
                    case ShipPartShapesBuilderStatus.BuildingShape:
                        // Do nothing.
                        break;
                }
            });
        }
        #endregion
    }
}
