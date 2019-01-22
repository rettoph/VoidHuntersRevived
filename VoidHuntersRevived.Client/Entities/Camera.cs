using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Entities
{
    /// <summary>
    /// Special class used specifically for camera operations
    /// This can dynamically be used to convert a farseer world
    /// into a screen world renderable by xna, allowing all entity
    /// drawing to be done directly to xna
    /// 
    /// The camera can be given a custom "Driver" that is used
    /// to update its position every frame
    /// </summary>
    public class Camera : Entity
    {
        #region Attributes
        protected Vector2 position;
        protected Single rotation;
        public Vector2 Position
        {
            get { return this.position; }
        }
        public Single Rotation
        {
            get { return this.rotation; }
        }

        public Single Zoom { get; set; }
        public RectangleF Bounds { get; protected set; }
        public RectangleF VisibleArea { get; protected set; }
        public Matrix Transform { get; protected set; }
        public Matrix Projection { get; protected set; }
        public Matrix InverseViewMatrix { get; protected set; }
        public BasicEffect BasicEffect { get; protected set; }

        private Vector2 _boundsX, _boundsY, _boundsSize;

        private GameWindow _window;
        #endregion

        public FarseerEntity Follow { get; set; }

        public Camera(GameWindow window, EntityInfo info, IGame game) : base(info, game)
        {
            _window = window;

            // By default the camera cannot be rendered
            this.Visible = false;
            this.Enabled = true;
            this.UpdateOrder = 100;
            // Set some defaults
            this.Zoom = 1f;
            this.position = Vector2.Zero;
            // Create a new basic effect to handle the farseer coordinate transformation
            this.BasicEffect = new BasicEffect(this.Game.Graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = true
            };

            this.UpdateBounds();
            _window.ClientSizeChanged += this.HandleClientSizeChanged;
        }

        #region Initializable Implementation
        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Set the bounds values based on the output graphics device
        /// </summary>
        private void UpdateBounds()
        {
            this.Bounds = new RectangleF(
                x: 0,
                y: 0,
                width: ConvertUnits.ToSimUnits(this.Game.Graphics.GraphicsDevice.Viewport.Bounds.Width),
                height: ConvertUnits.ToSimUnits(this.Game.Graphics.GraphicsDevice.Viewport.Bounds.Height));

            _boundsSize = new Vector2(this.Bounds.Width, this.Bounds.Height);
            _boundsX = new Vector2(this.Bounds.Width, 0);
            _boundsY = new Vector2(0, this.Bounds.Height);
        }

        private void UpdateVisibleArea()
        {
            this.InverseViewMatrix = Matrix.Invert(this.Transform);

            var tl = Vector2.Transform(Vector2.Zero, this.InverseViewMatrix);
            var tr = Vector2.Transform(_boundsX, this.InverseViewMatrix);
            var bl = Vector2.Transform(_boundsY, this.InverseViewMatrix);
            var br = Vector2.Transform(_boundsSize, this.InverseViewMatrix);

            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));

            this.VisibleArea = new RectangleF(min.X, min.Y, (max.X - min.X), (max.Y - min.Y));
        }

        private void UpdateMatrix()
        {

            this.Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                Matrix.CreateScale(Zoom) *
                Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));


            this.Projection =
                Matrix.CreateOrthographicOffCenter(
                    this.Position.X - this.Bounds.Width / 2,
                    this.Position.X + this.Bounds.Width / 2,
                    this.Position.Y + this.Bounds.Height / 2,
                    this.Position.Y - this.Bounds.Height / 2,
                    0f,
                    1f) *
                Matrix.CreateScale(Zoom);

            this.UpdateVisibleArea();
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if(this.Follow != null)
            {
                this.position = this.Follow.Body.Position;
            }

            this.UpdateMatrix();

            this.BasicEffect.Projection = this.Projection;
            this.BasicEffect.View = Matrix.Identity;
            this.BasicEffect.CurrentTechnique.Passes[0].Apply();
        }
        #endregion

        #region Event Handlers
        private void HandleClientSizeChanged(object sender, EventArgs e)
        {
            this.UpdateBounds();
        }
        #endregion
    }
}
