using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using Guppy.Extensions.Utilities;
using VoidHuntersRevived.Client.Library.Effects;
using Guppy.Extensions.Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    internal sealed class TrailSegment : Frameable
    {
        #region Static Properties
        /// <summary>
        /// The amount a single trail segment should spread 
        /// per second.
        /// </summary>
        public static Single Spread { get; set; } = 4;
        #endregion

        #region Private Fields
        /// <summary>
        /// The relative spread valud calculated based on the 
        /// current segment orientation.
        /// </summary>
        private Vector2 _spread;

        private VertexTrailSegment[] _vertices;

        private PrimitiveBatch<VertexTrailSegment, TrailInterpolationEffect> _primitiveBatch;
        #endregion

        #region Public Properties
        public Double Age { get; private set; }
        public Vector2 Position { get; internal set; }
        public Vector2 Velocity { get; internal set; }
        public Single Rotation { get; internal set; }
        public Color BaseColor { get; internal set; }
        public Single SpreadModifier { get; internal set; }
        public TrailSegment OlderSibling { get; internal set; }

        public Color Color { get; private set; }
        public Vector2 StarboardSpread { get; private set; }
        public Vector2 PortSpread { get; private set; }
        public Vector2 Starboard { get; private set; }
        public Vector2 Port { get; private set; }

        public Single CurrentSpread { get; private set; }
        #endregion

        #region Constructors
        internal TrailSegment()
        {

        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            provider.Service(out _primitiveBatch);

            _vertices = new VertexTrailSegment[2];
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Age = 0;
            this.StarboardSpread = Vector2.Zero;
            this.PortSpread = Vector2.Zero;

            _spread = Vector2.Transform(Vector2.UnitX * TrailSegment.Spread * this.SpreadModifier, Matrix.CreateRotationZ(this.Rotation + MathHelper.PiOver2));
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Age += gameTime.ElapsedGameTime.TotalSeconds;
            this.Position += this.Velocity * (Single)gameTime.ElapsedGameTime.TotalSeconds;

            this.StarboardSpread += _spread * (Single)gameTime.ElapsedGameTime.TotalSeconds;
            this.PortSpread -= _spread * (Single)gameTime.ElapsedGameTime.TotalSeconds;

            this.Starboard = this.Position + this.StarboardSpread;
            this.Port = this.Position + this.PortSpread;

            this.CurrentSpread = Vector2.Distance(this.Port, this.Starboard);

            this.Color = Color.Lerp(this.BaseColor, Color.Transparent, Math.Min(1, (Single)(this.Age / Trail.MaxSegmentAge)));

            // Update the internal quad vertices...
            _vertices[0].Position = new Vector4(this.Starboard, 0, 1);
            _vertices[0].Color = this.Color.ToVector4();
            _vertices[0].RayLength = this.CurrentSpread;
            _vertices[0].Port = this.Port;

            _vertices[1].Position = new Vector4(this.Port, 0, 1);
            _vertices[1].Color = this.Color.ToVector4();
            _vertices[1].RayLength = this.CurrentSpread;
            _vertices[1].Port = this.Port;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.DrawTriangle(
                v1: ref this.OlderSibling._vertices[0],
                v2: ref this.OlderSibling._vertices[1],
                v3: ref this._vertices[1]);

            _primitiveBatch.DrawTriangle(
                v1: ref this._vertices[0],
                v2: ref this.OlderSibling._vertices[0],
                v3: ref this._vertices[1]);

            // _primitiveBatch.DrawTriangle(
            //     v1: new VertexTrailSegment()
            //     {
            //         Position = this.OlderSibling.Port,
            //         Color = this.Color,
            //     },
            //     v2: new VertexTrailSegment()
            //     {
            //         Position = this.Position,
            //         Color = this.Color,
            //     },
            //     v3: new VertexTrailSegment()
            //     {
            //         Position = this.OlderSibling.Position,
            //         Color = this.Color,
            //     });
            // 
            // _primitiveBatch.DrawTriangle(
            //     v1: new VertexTrailSegment()
            //     {
            //         Position = this.Port,
            //         Color = this.Color,
            //     },
            //     v2: new VertexTrailSegment()
            //     {
            //         Position = this.Position,
            //         Color = this.Color,
            //     },
            //     v3: new VertexTrailSegment()
            //     {
            //         Position = this.OlderSibling.Port,
            //         Color = this.Color,
            //     });
        }
        #endregion
    }
}
