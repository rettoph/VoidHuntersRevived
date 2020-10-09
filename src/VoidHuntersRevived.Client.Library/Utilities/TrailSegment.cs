using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Utilities
{
    internal sealed class TrailSegment
    {
        #region Static Fields
        public static Double MaxAge { get; private set; } = 2000;
        private static Vector3 Spread = Vector3.UnitX * 0.25f;

        /// <summary>
        /// Pixels per second
        /// </summary>
        private static Vector3 Speed = Vector3.UnitX;
        #endregion

        #region Private Fields
        private ServiceProvider _provider;
        private ServiceDescriptor _descriptor;
        private Trail _trail;
        private Single _direction;
        private Single _strength;
        private Vector3 _tangentDelta;
        private Color _initialColor;
        #endregion

        #region Public Attributes
        public Double Age { get; private set; }
        public Color Color { get; private set; }
        public Vector3 Port { get; private set; }
        public Vector3 Starboard { get; private set; }

        public VertexPositionColor PortVertexPositionColor => new VertexPositionColor(this.Port, this.Color);
        public VertexPositionColor StarboardVertexPositionColor => new VertexPositionColor(this.Starboard, this.Color);
        #endregion


        #region Lifecycle Methods
        internal TrailSegment(ServiceProvider provider)
        {
            _provider = provider;
            _descriptor = provider.GetServiceDescriptor<TrailSegment>();
        }

        public TrailSegment Initialize(Trail trail)
        {
            _trail = trail;

            this.Refresh();

            return this;
        }

        public void Dispose()
        {
            _descriptor.Factory.Return(this);
        }
        #endregion

        #region Frame Methods
        public void Update(GameTime gameTime)
        {
            var speed = _tangentDelta * (Single)gameTime.ElapsedGameTime.TotalSeconds;

            this.Age += gameTime.ElapsedGameTime.TotalMilliseconds;
            this.Port += speed;
            this.Starboard -= speed;
            this.Color = Color.Lerp(Color.Transparent, _initialColor, _strength * (1 - ((Single)(this.Age / TrailSegment.MaxAge))));
        }
        #endregion

        #region Helper Methods
        public void Refresh()
        {
            _direction = _trail.thruster.Rotation;
            _strength = _trail.thruster.Strength;
            _initialColor = _trail.thruster.Color;
            _tangentDelta = Vector3.Transform(TrailSegment.Speed, Matrix.CreateRotationZ(_direction + MathHelper.Pi - (MathHelper.Pi / 2)));
            this.Port = new Vector3(_trail.thruster.Position, 0) + Vector3.Transform(TrailSegment.Spread, Matrix.CreateRotationZ(_direction + MathHelper.PiOver2));
            this.Starboard = new Vector3(_trail.thruster.Position, 0) + Vector3.Transform(TrailSegment.Spread, Matrix.CreateRotationZ(_direction - MathHelper.PiOver2));
            this.Age = 0;
        }
        #endregion
    }
}
