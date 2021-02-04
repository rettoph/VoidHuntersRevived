using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Thrusters
{
    public class Thruster : RigidShipPart
    {
        #region Static Properties
        public static Single ImpulseModifierEpsilon { get; set; } = 0.001f;
        #endregion

        #region Private Fields
        private Single _impulseModifier;
        #endregion

        #region Public Properties
        public new ThrusterContext Context { get; private set; }

        /// <summary>
        /// The multipier applied to FullImpulse in order to
        /// calculate the current <see cref="Impulse"/>
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
        public Vector2 Impulse => this.Context.MaxImpulse * this.ImpulseModifier;

        /// <summary>
        /// A flags value of all active <see cref="Ship.Direction"/>
        /// values currently active within the current thruster.
        /// 
        /// When any directions are active then <see cref="ApplyThrust(ShipPart, GameTime)"/>
        /// will automatically be applied each frame.
        /// </summary>
        public Ship.Direction ActiveDirections { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// indicates that the current <see cref="Impulse"/> value
        /// has changed from either nothing or anything.
        /// </summary>
        public event OnEventDelegate<Thruster, Boolean> OnImpulse;
        #endregion

        #region Lifecycle Methods
        protected override void Create(ServiceProvider provider)
        {
            base.Create(provider);

            this.OnChainChanged += Thruster.HandleChainChanged;
        }

        protected override void PreRelease()
        {
            base.PreRelease();

            this.ActiveDirections = Ship.Direction.None;
            this.ImpulseModifier = 0f;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.OnChainChanged -= Thruster.HandleChainChanged;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Apply thrust to the internal fixture...
            if (this.ActiveDirections != Ship.Direction.None || this.ImpulseModifier > Thruster.ImpulseModifierEpsilon)
            {
                this.ImpulseModifier = MathHelper.Lerp(
                    value1: this.ImpulseModifier,
                    value2: this.ApplyThrust(this.Root, gameTime) ? 1 : 0,
                    amount: this.Context.ImpulseAcceleration * (Single)gameTime.ElapsedGameTime.TotalSeconds);
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
        /// <param name="gameTime"></param>
        public Boolean ApplyThrust(ShipPart root, GameTime gameTime)
        {
            if (this.ActiveDirections != Ship.Direction.None)
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
        /// Return an enumerable of <see cref="Ship.Direction"/>
        /// flags that the current <see cref="Thruster"/> will 
        /// move its owning <see cref="Chain.Ship"/>, if any.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Ship.Direction> GetDirections()
        {
            if (this.Chain?.Ship != default)
            {
                // Each andle of movement has a buffer sone on inclusivity
                var buffer = 0.01f;

                // The chain's center of mass
                var com = this.Root.LocalCenter;
                // The point acceleration is applied
                var ip = this.LocalCenter;
                // The impulse to be applied...
                var i = Vector2.Transform(this.Context.MaxImpulse, Matrix.CreateRotationZ(this.LocalRotation));
                // The point acceleration is targeting
                var it = ip + i;

                // The angle between the com and the acceleration point
                var ipr = MathHelper.WrapAngle((float)Math.Atan2(ip.Y - com.Y, ip.X - com.X));
                // The angle between the com and the acceleration target
                var itr = MathHelper.WrapAngle((float)Math.Atan2(it.Y - com.Y, it.X - com.X));
                // The angle between the acceleration point and the acceleration target
                var ipitr = MathHelper.WrapAngle((float)Math.Atan2(it.Y - ip.Y, it.X - ip.X));
                // The relative acceleration target rotation between the acceleration point and center of mass
                var ript = MathHelper.WrapAngle(ipitr - ipr);

                // Define some lower and upper bounds...
                var ipitr_lower = ipitr - buffer;
                var ipitr_upper = ipitr + buffer;

                // Check if the thruster moves the chain forward...
                if ((ipitr_upper < MathHelper.PiOver2 && ipitr_lower > -MathHelper.PiOver2))
                    yield return Ship.Direction.Forward;

                // Check if the thruster turns the chain right...
                if (ript > 0)
                    yield return Ship.Direction.TurnRight;

                // Check if the thruster moves the chain backward...
                if (ipitr_lower > MathHelper.PiOver2 || ipitr_upper < -MathHelper.PiOver2)
                    yield return Ship.Direction.Backward;

                // Check if the thruster turns the chain left...
                if (ript < 0)
                    yield return Ship.Direction.TurnLeft;

                // Check if the thruster moves the chain right...
                if (ipitr_lower < 0 && ript > -MathHelper.Pi)
                    yield return Ship.Direction.Right;

                // Check if the thruster moves the chain left...
                if (ipitr_lower > 0 && ipitr_upper < MathHelper.Pi)
                    yield return Ship.Direction.Left;
            }
        }

        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as ThrusterContext;
        }
        #endregion

        #region Event Handlers
        private static void HandleChainChanged(ShipPart sender, Chain old, Chain value)
        {
            if(sender is Thruster thruster)
                thruster.ActiveDirections = Ship.Direction.None;
        }
        #endregion
    }
}
