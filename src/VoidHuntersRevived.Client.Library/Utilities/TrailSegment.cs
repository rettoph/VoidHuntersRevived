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

        public Single PortSlope { get; private set; }
        public Single StarboardSlope { get; private set; }
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

            this.Color = Color.Lerp(this.BaseColor, Color.Transparent, Math.Min(1, (Single)(this.Age * 2 / Trail.MaxSegmentAge)));
        
            if(this.OlderSibling == default)
            {
                this.StarboardSlope = 1;
                this.PortSlope = 1;
            }
            else
            {
                this.StarboardSlope = this.Position.Angle(this.OlderSibling.Starboard);
                this.PortSlope = this.Position.Angle(this.OlderSibling.Port);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.DrawTriangle(
                v1: new VertexTrailSegment()
                {
                    Position = this.OlderSibling.Starboard,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.StarboardSlope
                },
                v2: new VertexTrailSegment()
                {
                    Position = this.OlderSibling.Position,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.StarboardSlope
                },
                v3: new VertexTrailSegment()
                {
                    Position = this.Position,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.StarboardSlope
                });

            _primitiveBatch.DrawTriangle(
                v1: new VertexTrailSegment()
                {
                    Position = this.Starboard,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.StarboardSlope
                },
                v2: new VertexTrailSegment()
                {
                    Position = this.OlderSibling.Starboard,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.StarboardSlope
                },
                v3: new VertexTrailSegment()
                {
                    Position = this.Position,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.StarboardSlope
                });

            _primitiveBatch.DrawTriangle(
                v1: new VertexTrailSegment()
                {
                    Position = this.OlderSibling.Port,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.PortSlope
                },
                v2: new VertexTrailSegment()
                {
                    Position = this.Position,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.PortSlope
                },
                v3: new VertexTrailSegment()
                {
                    Position = this.OlderSibling.Position,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.PortSlope
                });

            _primitiveBatch.DrawTriangle(
                v1: new VertexTrailSegment()
                {
                    Position = this.Port,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.PortSlope
                },
                v2: new VertexTrailSegment()
                {
                    Position = this.Position,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.PortSlope
                },
                v3: new VertexTrailSegment()
                {
                    Position = this.OlderSibling.Port,
                    Color = this.Color,
                    SegmentStart = this.Position,
                    Slope = this.PortSlope
                });
        }
        #endregion
    }
}
