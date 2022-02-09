using Guppy.EntityComponent.Enums;
using Guppy.EntityComponent.Interfaces;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Library.Graphics.Effects;
using VoidHuntersRevived.Client.Library.Graphics.Vertices;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.Graphics.Utilities
{
    /// <summary>
    /// A trail is a 'linked list' of TrailSegments.
    /// </summary>
    public class Trail : IDisposable
    {
        private Boolean _active;

        private Thruster _thruster;

        private TrailSegment _head;
        private TrailSegment _tail;

        public event OnEventDelegate<Trail> OnDisposed;

        internal Trail(Thruster thruster)
        {
            _thruster = thruster;
            _thruster.OnStatusChanged += this.HandleThrusterStatusChanged;
        }

        /// <summary>
        /// Activate the current trail, binding it to rhe recieved
        /// thruster.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Activate(GameTime gameTime)
        {
            if(this.TryBuildSegment(gameTime, out _head))
            {
                _tail = _head;
                _active = true;
            }
        }

        /// <summary>
        /// De-activate the trail. No more segments will be added and 
        /// once all existing segments are removed it will be disposed
        /// </summary>
        public void Deactivate()
        {
            if(!_active)
            {
                return;
            }

            _thruster.OnStatusChanged -= this.HandleThrusterStatusChanged;
            _thruster = default;

            _active = false;
        }

        public void Draw(GameTime gameTime, PrimitiveBatch<VertexTrail, TrailEffect> primitiveBatch)
        {
            if(_head is null)
            {
                this.Dispose();
                return;
            }

            this.TryBuildSegment(gameTime, out _tail.Next);
            _head.Draw(gameTime, primitiveBatch);
        }

        public void Update(GameTime gameTime)
        {
            if(_active && _tail.Next is not null)
            { // Append the tail... 
                _tail = _tail.Next;
            }

            while(_head is not null && gameTime.TotalGameTime.TotalSeconds - _head.CreatedTimestamp > _head.MaxAge)
            {   // Cut the head
                _head = _head.Next;
            }
        }

        private Boolean TryBuildSegment(GameTime gameTime, out TrailSegment segment)
        {
            if(_thruster == default)
            {
                segment = default;
                return false;
            }

            var currentTimestamp = (Single)gameTime.TotalGameTime.TotalSeconds;
            var maxAge = 3f;
            var spreadSpeed = 5f;
            var color = Color.Lerp(Color.Transparent, this._thruster.Chain.Color.Value, 0.5f);
            var position = _thruster.CalculateWorldPoint(this._thruster.Context.Centeroid);
            var rotation = _thruster.Chain.Body.LocalInstance.Rotation + _thruster.LocalRotation;
            var velocity = _thruster.Chain.Body.LocalInstance.LinearVelocity - _thruster.Context.Thrust.Rotate(rotation);

            segment = new TrailSegment()
            {
                CreatedTimestamp = gameTime.TotalGameTime.TotalSeconds,
                MaxAge = maxAge,
                Port = new VertexTrail()
                {
                    Position = position,
                    Velocity = velocity,
                    Color = color,
                    SpreadDirection = rotation - MathHelper.PiOver2,
                    SpreadSpeed = spreadSpeed,
                    CreatedTimestamp = currentTimestamp,
                    MaxAge = maxAge,
                },
                Starboard = new VertexTrail()
                {
                    Position = position,
                    Velocity = velocity,
                    Color = color,
                    SpreadDirection = rotation + MathHelper.PiOver2,
                    SpreadSpeed = spreadSpeed,
                    CreatedTimestamp = currentTimestamp,
                    MaxAge = maxAge,
                }
            };

            return true;
        }

        public void Dispose()
        {
            this.Deactivate();

            this.OnDisposed?.Invoke(this);
        }

        #region Event Handlers
        private void HandleThrusterStatusChanged(IService sender, ServiceStatus old, ServiceStatus value)
        {
            if(value == ServiceStatus.Uninitializing)
            {
                this.Deactivate();
            }
        }
        #endregion
    }
}
