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

        private PrimitiveBatch _primitiveBatch;
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
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _primitiveBatch.DrawTriangle(
                c1: this.OlderSibling.Color,
                p1: this.OlderSibling.Starboard,
                c2: this.OlderSibling.Color,
                p2: this.OlderSibling.Position,
                c3: this.Color,
                p3: this.Position);

            _primitiveBatch.DrawTriangle(
                c1: this.Color,
                p1: this.Starboard,
                c2: this.OlderSibling.Color,
                p2: this.OlderSibling.Starboard,
                c3: this.Color,
                p3: this.Position);

            _primitiveBatch.DrawTriangle(
                c1: this.OlderSibling.Color,
                p1: this.OlderSibling.Port,
                c2: this.Color,
                p2: this.Position,
                c3: this.OlderSibling.Color,
                p3: this.OlderSibling.Position);

            _primitiveBatch.DrawTriangle(
                c1: this.Color,
                p1: this.Port,
                c2: this.Color,
                p2: this.Position,
                c3: this.OlderSibling.Color,
                p3: this.OlderSibling.Port);

            // _primitiveBatch.DrawTriangle(Color.Transparent, this.OlderSibling.Port, this.Color, this.Position, this.OlderSibling.Color, this.OlderSibling.Position);
            // _primitiveBatch.DrawTriangle(Color.Transparent, this.OlderSibling.Starboard, this.OlderSibling.Color, this.OlderSibling.Position, this.Color, this.Position);
            // 
            // _primitiveBatch.DrawTriangle(Color.Transparent, this.Port, this.Color, this.Position, Color.Transparent, this.OlderSibling.Port);
            // _primitiveBatch.DrawTriangle(this.Color, this.Position, Color.Transparent, this.Starboard, Color.Transparent, this.OlderSibling.Starboard);
        }
        #endregion
    }
}
