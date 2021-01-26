using Guppy;
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
using Guppy.Extensions.System.Collections;
using Guppy.Extensions.Microsoft.Xna.Framework;
using VoidHuntersRevived.Builder.Attributes;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// The primary service used to create and edit the <see cref="ShipPartContext.InnerShapes"/>,
    /// <see cref="ShipPartContext.FemaleConnectionNodes"/> & <see cref="ShipPartContext.OuterHulls"/>
    /// values.
    /// </summary>
    [ShipPartContextBuilderService("Shape Designer", 0)]
    public class ShipPartShapesBuilderService : ShipPartContextBuilderService
    {
        #region Enums
        public enum ShipPartShapesBuilderStatus
        {
            None,
            BuildingShape,
            Editing
        }
        #endregion

        #region Private Fields
        private ShipPartShapesBuilderPage _page;
        private ShipPartShapeBuilderService _builder;
        private ShipPartShapeEditorService _editor;
        private ConnectionNodeEditorService _nodeEditor;
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
        private ConnectionNodeContext _male;
        private List<ConnectionNodeContext> _females;
        #endregion

        #region Public Properties
        public ShipPartShapesBuilderPage Page => _page;
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
            provider.Service(out _editor, (e, p, c) => e.shapes = this);
            provider.Service(out _nodeEditor, (e, p, c) => e.shapes = this);

            _male = new ConnectionNodeContext();
            _females = new List<ConnectionNodeContext>();
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
            _page.AddFemaleNodeButton.OnClicked += this.HandleAddFemaleNodeButtonClicked;
            _builder.OnShapeCompleted += this.HandleShapeCompleted;
            _mouse.OnButtonStateChanged += this.HandleMouseButtonStateChanged;
        }

        protected override void Release()
        {
            base.Release();

            _page.AddShapeButton.OnClicked -= this.HandleAddShapeButtonClicked;
            _page.AddFemaleNodeButton.OnClicked -= this.HandleAddFemaleNodeButtonClicked;
            _builder.OnShapeCompleted -= this.HandleShapeCompleted;
            _mouse.OnButtonStateChanged -= this.HandleMouseButtonStateChanged;

            _camera = null;
            _graphics = null;
            _spriteBatch = null;
            _primitiveBatch = null;
            _synchronizer = null;
            _shipParts = null;
            _shipPartRenderService = null;
            _mouse = null;

            _male = null;
            _females = null;
            _shapes = null;
            _effect.Dispose();
            _page.TryRelease();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_demo?.Status == Guppy.Enums.ServiceStatus.Ready)
            {
                _shipPartRenderService.RemoveContext(_demo);
                _demo?.TryRelease();
            }

            this.context.InnerShapes = _shapes.Select(s => new Vertices(s.GetVertices())).ToArray();
            this.context.OuterHulls = new Vertices[0];
            this.context.MaleConnectionNode = _male;
            this.context.FemaleConnectionNodes = _females.ToArray();

            _demo = _shipParts.Create(this.context);
            _demo.Position = _camera.Position;

            _builder.TryUpdate(gameTime);
            _editor.TryUpdate(gameTime);
            _nodeEditor.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _effect.View = _camera.View;
            _effect.World = _camera.World;
            _effect.Projection = _camera.Projection;

            _spriteBatch.Begin(effect: _effect);
            _primitiveBatch.Begin(_camera);

            foreach (Vector2 p in this.GetInterestPoints())
                _primitiveBatch.DrawCircle(
                    color: Color.Red, 
                    x: _camera.Position.X + p.X, 
                    y: _camera.Position.X + p.Y, 
                    radius: 0.03f, 
                    segments: 15);

            foreach(ConnectionNodeContext female in _females)
            {
                _primitiveBatch.DrawLine(
                    Color.Yellow,
                    _camera.Position + female.Position - (Vector2.UnitX * 0.1f).RotateTo(female.Rotation + MathHelper.PiOver2),
                    _camera.Position + female.Position + (Vector2.UnitX * 0.1f).RotateTo(female.Rotation + MathHelper.PiOver2));

                _primitiveBatch.DrawLine(
                    Color.Yellow,
                    _camera.Position + female.Position,
                    _camera.Position + female.Position + (Vector2.UnitX * 0.3f).RotateTo(female.Rotation + MathHelper.Pi));

                _primitiveBatch.DrawLine(
                    Color.Yellow,
                    _camera.Position + female.Position,
                    _camera.Position + female.Position + (Vector2.UnitX * 0.2f).RotateTo(female.Rotation + MathHelper.Pi + 0.5f));

                _primitiveBatch.DrawLine(
                    Color.Yellow,
                    _camera.Position + female.Position,
                    _camera.Position + female.Position + (Vector2.UnitX * 0.2f).RotateTo(female.Rotation - MathHelper.Pi - 0.5f));
            }

            _builder.TryDraw(gameTime);
            _editor.TryDraw(gameTime);
            _nodeEditor.TryDraw(gameTime);

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

            if (this.context.MaleConnectionNode != default)
                yield return this.context.MaleConnectionNode.Position;

            if(this.context.FemaleConnectionNodes != default)
                foreach (Vector2 pos in this.context.FemaleConnectionNodes.Select(n => n.Position))
                    yield return pos;
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
        /// Check to see if there is a node or shape under the current
        /// mouse position. If so, start editing & return true.
        /// </summary>
        /// <returns></returns>
        private Boolean TryStartEditing()
        {
            // First check for nodes!
            var node = this.TestPointForConnectionNode(this.mouseWorldPosition);

            if(node != default)
            {
                _editor.Stop();
                _nodeEditor.Start(node);
                return true;
            }

            // Next test for a shape.
            var shape = this.TestPointForShape(this.mouseWorldPosition);

            if(shape != default)
            {
                _nodeEditor.Stop();
                _editor.Start(shape);
                return true;
            }

            // Nothing found, return false.
            return false;
        }

        public ShapeContext TestPointForShape(Vector2 position)
        {
            var fixture = _world.Live.TestPoint(this.mouseWorldPosition);

            if (fixture == default)
                return default;

            return _shapes[fixture.Body.FixtureList.IndexOf(fixture)];
        }

        public ConnectionNodeContext  TestPointForConnectionNode(Vector2 position)
        {
            var localPosition = position - _camera.Position;
            var maxDistance = 0.1f;

            if (Vector2.Distance(_male.Position, localPosition) < maxDistance)
                return _male;

            foreach(ConnectionNodeContext female in _females)
                if (Vector2.Distance(female.Position, localPosition) < maxDistance)
                    return female;

            return default;
        }

        protected internal override void Open(ShipPartContext context)
        {
            base.Open(context);

            // Open the default API page...
            this.pages.Open(_page);
        }

        protected internal override void Close()
        {
            base.Close();

            if (_demo?.Status == Guppy.Enums.ServiceStatus.Ready)
            {
                _shipPartRenderService.RemoveContext(_demo);
                _demo?.TryRelease();
            }
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

        private void HandleShapeCompleted(ShipPartShapeBuilderService sender, ShapeContext shape)
        {
            _status = ShipPartShapesBuilderStatus.None;

            if (shape.Sides.Count < 2)
                return;

            _shapes.Add(shape);
        }

        private void HandleMouseButtonStateChanged(InputManager sender, InputArgs args)
        {
            _synchronizer.Enqueue(gt =>
            {
                switch (_status)
                {
                    case ShipPartShapesBuilderStatus.None:
                    case ShipPartShapesBuilderStatus.Editing:
                        if (args.State == ButtonState.Pressed && this.TryStartEditing())
                        { // There is a shape being selected!
                            _status = ShipPartShapesBuilderStatus.Editing;
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

        private void HandleAddFemaleNodeButtonClicked(Element sender)
        {
            _females.Add(new ConnectionNodeContext());
        }
        #endregion
    }
}
