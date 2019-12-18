using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Library.Entities
{
    public class FarseerEntity : NetworkEntity 
    {
        #region Private Fields
        private Annex _annex;
        #endregion

        #region Protected Properties
        protected World world { get; private set; }
        protected ChunkCollection chunks { get; private set; }
        #endregion

        #region Public Properties
        public Body Body { get; private set; }
        public virtual Vector2 Position { get => this.Body.Position; }
        public virtual Single Rotation { get => this.Body.Rotation; }
        public virtual Vector2 LinearVelocity { get => this.Body.LinearVelocity; }
        public virtual Single AngularVelocity { get => this.Body.AngularVelocity; }
        public virtual Vector2 WorldCenter { get => this.Body.WorldCenter; }
        public virtual Vector2 LocalCenter { get => this.Body.LocalCenter; }
        public Controller Controller { get; private set; }
        /// <summary>
        /// Simple property that defines the current FarseerEntity's active
        /// state. Farseer entities that are no longe rin control of themselves
        /// should set this value to false.
        /// </summary>
        public virtual Boolean IsActive { get => true; }
        /// <summary>
        /// Indicates whether or not the current entity is currently
        /// moving.
        /// </summary>
        public virtual Boolean IsMoving { get => this.Body.LinearVelocity.Length() > Single.Epsilon && Math.Abs(this.Body.AngularVelocity) > Single.Epsilon; }
        #endregion

        #region Events
        public NetIncomingMessageDelegate ReadBodyVitals;
        public NetOutgoingMessageDelegate WriteBodyVitals;
        public event EventHandler<Controller> OnControllerChanged;
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _annex = provider.GetRequiredService<Annex>();

            this.world = provider.GetRequiredService<World>();
            this.chunks = provider.GetRequiredService<ChunkCollection>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Create a new body for this instance
            this.Body = this.CreateBody(this.world);
        }

        protected override void Initialize()
        {
            base.Initialize();

            // By default Farseer entities are not independently managed.
            // They must be added to a controller instance (chunks, ships, annex, ect)
            this.SetEnabled(false);
            this.SetVisible(false);
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            // Add the current farseer entity into its respective chunk
            if(this.Controller == null)
                this.chunks.AddToChunk(this);
        }

        public override void Dispose()
        {
            base.Dispose();

            this.Controller?.Remove(this);
            this.SetController(_annex);
            this.Body.Dispose(withFixtures: true);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Allow the controller to manipulate the internal body if needed
            if (this.Body.IsSolidEnabled())
                this.Controller.UpdateBody(this, this.Body);
        }
        #endregion

        #region Farseer Methods
        public virtual Body CreateBody(World world)
        {
            return BodyFactory.CreateBody(world, default, 0, BodyType.Static, this);
        }
        #endregion

        #region Helper Methods
        internal void SetController(Controller controller)
        {
            if (controller == default(Controller))
                throw new Exception("Unable to use null Controller. Please use the Annex instead.");

            if (controller != this.Controller)
            {
                // Auto remove the component from its old controller
                if (this.Controller != default(Controller))
                {
                    this.Controller.Remove(this);
                    this.Controller?.UpdateBody(this, this.Body);
                }

                this.Controller = controller;
                this.Controller?.SetupBody(this, this.Body);

                this.OnControllerChanged?.Invoke(this, this.Controller);
            }
        }
        #endregion

        #region Network Methods
        protected override void WriteSetup(NetOutgoingMessage om)
        {
            base.WriteSetup(om);

            this.Body.WriteVitals(om);
        }

        protected override void ReadSetup(NetIncomingMessage im)
        {
            base.ReadSetup(im);

            this.Body.ReadVitals(im);
        }

        #region Vitals Pings
        public override bool CanSendVitals(bool interval)
        {
            return this.IsActive && this.Body.IsSolidEnabled() && ((this.Body.Awake && interval) || this.Controller is Chunk);
        }

        protected override void ReadVitals(NetIncomingMessage im)
        {
            base.ReadVitals(im);

            // Trigger the read delegate & assume that its being taken care of
            // This probably takes place in the FarseerEntityClientDriver
            this.ReadBodyVitals?.Invoke(this, im);
        }

        protected override void WriteVitals(NetOutgoingMessage om)
        {
            base.WriteVitals(om);

            // Trigger the write delegate & assume that its being taken care of
            // This probably takes place in the FarseerEntityServerDriver
            this.WriteBodyVitals?.Invoke(this, om);
        }
        #endregion
        #endregion
    }
}
