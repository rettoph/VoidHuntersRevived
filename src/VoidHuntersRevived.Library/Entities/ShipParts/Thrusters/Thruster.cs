using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Thrusters
{
    public class Thruster : RigidShipPart
    {
        #region Static Properties
        public static Single StrengthAcceleration { get; set; } = 1f;
        public static Single ImpulseModifierEpsilon { get; set; } = 0.00001f;
        #endregion

        #region Private Fields
        private Single _impulseModifier;
        #endregion

        #region Public Properties
        /// <summary>
        /// The directions the current thruster will
        /// update its chain.
        /// </summary>
        public Ship.Direction Directions { get; private set; }
        /// <summary>
        /// Indicates if the thruster was activeted this frame.
        /// </summary>
        public Boolean Active => this.Chain.Ship != default(Ship) && (this.Chain.Ship.ActiveDirections & this.Directions) != 0;

        /// <summary>
        /// The maximum force per second 
        /// achievable by the current thruster.
        /// </summary>
        public Vector2 FullImpulse { get => Vector2.UnitX * 10f; }

        /// <summary>
        /// The multipier applied to FullImpulse in order to
        /// calculate the current
        /// </summary>
        public Single ImpulseModifier
        {
            get => _impulseModifier;
            set
            {
                if (_impulseModifier != value)
                {
                    if (value <= Thruster.ImpulseModifierEpsilon)
                    {
                        if(_impulseModifier != 0)
                        {
                            _impulseModifier = 0;
                            this.OnImpulse?.Invoke(this, false);
                        }
                    }
                    else if (_impulseModifier == 0)
                    {
                        _impulseModifier = value;
                        this.OnImpulse?.Invoke(this, true);
                    }
                    else
                    {
                        _impulseModifier = value;
                    }
                }
                
            }
        }

        /// <summary>
        /// The current impulse applied by the thruster.
        /// </summary>
        public Vector2 Impulse => this.FullImpulse * this.ImpulseModifier;

        /// <summary>
        /// The local point at which impulse will be applied.
        /// </summary>
        public Vector2 LocalImpulsePoint { get => Vector2.Transform(this.FullImpulse, Matrix.CreateRotationZ(this.LocalRotation)); }
        #endregion

        #region Events
        /// <summary>
        /// indicates that the current thruster's impulse value
        /// has changed from either nothing or anything.
        /// </summary>
        public event OnEventDelegate<Thruster, Boolean> OnImpulse;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.OnChainChanged += this.HandleChainChanged;
        }

        protected override void Release()
        {
            base.Release();

            this.OnChainChanged -= this.HandleChainChanged;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Apply thrust to the internal fixture...
            if (this.Active || this.ImpulseModifier > Thruster.ImpulseModifierEpsilon)
            {
                this.ImpulseModifier = MathHelper.Lerp(
                    value1: this.ImpulseModifier,
                    value2: this.ApplyThrust(this.Root) ? 1 : 0,
                    amount: Thruster.StrengthAcceleration * (Single)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Apply thrust to the inputed FarseerBody,
        /// assuming that it belongs to the current
        /// Thruster's chain.
        /// </summary>
        /// <param name="root"></param>
        public Boolean ApplyThrust(ShipPart root)
        {
            if (this.Active)
            { // Only apply any thrust if the current thruster is active...
                root.ApplyForce(
                    // Calculate the thrusters position on the recieved body...
                    forceGetter: b => this.Impulse.RotateTo(b.Rotation + this.LocalRotation),
                    // Calculate the thrust's world force relative to the recieved body...
                    pointGetter: b => b.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(b.Rotation)));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns all directions the current Thruster will
        /// move its chain when activated.
        /// </summary>
        /// <param name="directions"></param>
        /// <returns></returns>
        private Ship.Direction GetDirections()
        {
            // Each andle of movement has a buffer sone on inclusivity
            var buffer = 0.01f;

            // The chain's center of mass
            var com = this.Root.LocalCenter;
            // The point acceleration is applied
            var ap = this.LocalCenter;
            // The point acceleration is targeting
            var at = ap + this.LocalImpulsePoint;

            // The angle between the com and the acceleration point
            var apr = MathHelper.WrapAngle((float)Math.Atan2(ap.Y - com.Y, ap.X - com.X));
            // The angle between the com and the acceleration target
            var atr = MathHelper.WrapAngle((float)Math.Atan2(at.Y - com.Y, at.X - com.X));
            // The angle between the acceleration point and the acceleration target
            var apatr = MathHelper.WrapAngle((float)Math.Atan2(at.Y - ap.Y, at.X - ap.X));
            // The relative acceleration target rotation between the acceleration point and center of mass
            var ratr = MathHelper.WrapAngle(apatr - apr);

            var apatr_lower = apatr - buffer;
            var apatr_upper = apatr + buffer;

            Ship.Direction directions = Ship.Direction.None;

            // Check if the thruster moves the chain forward...
            if ((apatr_upper < MathHelper.PiOver2 && apatr_lower > -MathHelper.PiOver2))
                directions |= Ship.Direction.Forward;

            // Check if the thruster turns the chain right...
            if (ratr > 0)
                directions |= Ship.Direction.TurnRight;

            // Check if the thruster moves the chain backward...
            if (apatr_lower > MathHelper.PiOver2 || apatr_upper < -MathHelper.PiOver2)
                directions |= Ship.Direction.Backward;

            // Check if the thruster turns the chain left...
            if (ratr < 0)
                directions |= Ship.Direction.TurnLeft;

            // Check if the thruster moves the chain right...
            if (apatr_lower < 0 && ratr > -MathHelper.Pi)
                directions |= Ship.Direction.Right;

            // Check if the thruster moves the chain left...
            if (apatr_lower > 0 && apatr_upper < MathHelper.Pi)
                directions |= Ship.Direction.Left;

            return directions;
        }
        #endregion

        #region Event Handlers
        private void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            // Update the internal directions...
            this.Directions = this.GetDirections();

            if(old != default)
            {
                old.OnUpdate -= this.TryUpdate;
                old.OnShipPartAdded -= this.HandleChainShipPartAdded;
                old.OnShipPartRemoved -= this.HandleChainShipPartRemoved;
            }

            if(value != default(Chain) && value.Root != this)
            {
                value.OnUpdate += this.TryUpdate;
                value.OnShipPartAdded += this.HandleChainShipPartAdded;
                value.OnShipPartRemoved += this.HandleChainShipPartRemoved;
            }
        }

        private void HandleChainShipPartAdded(Chain sender, ShipPart args)
        {
            // Update the internal directions...
            this.Directions = this.GetDirections();
        }

        private void HandleChainShipPartRemoved(Chain sender, ShipPart args)
        {
            // Update the internal directions...
            this.Directions = this.GetDirections();
        }
        #endregion
    }
}
