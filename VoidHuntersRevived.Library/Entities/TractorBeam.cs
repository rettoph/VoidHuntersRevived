using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;
using System.Linq;
using FarseerPhysics.Dynamics.Joints;
using VoidHuntersRevived.Library.Utilities.ConnectionNodes;

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorBeam : FarseerEntity
    {
        #region Private Fields
        private Single _reach;
        private Guid _selectedFocusedId;
        private Fixture _sensor;
        private HashSet<ShipPart> _contacts;
        #endregion

        #region Public Attributes
        public Vector2 Offset { get; private set; }
        public Player Player { get; private set; }
        public ShipPart Selected { get; private set; }
        public WeldJoint Joint { get; private set; }
        #endregion

        #region Events
        public event EventHandler<Vector2> OnOffsetChanged;
        public event EventHandler<ShipPart> OnSelected;
        public event EventHandler<ShipPart> OnReleased;
        #endregion

        #region Constructors
        public TractorBeam(Player player, EntityConfiguration configuration, VoidHuntersWorldScene scene, IServiceProvider provider, ILogger logger) : base(configuration, provider)
        {
            this.Player = player;
        }
        public TractorBeam(Guid id, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(id, configuration, provider)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _reach = 25;
            _contacts = new HashSet<ShipPart>();

            this.IsSensor = true;
            this.SleepingAllowed = false;
            this.Focused.Add();

            _sensor = this.CreateFixture(new CircleShape(5f, 0f));
            this.SleepingAllowed = false;
            this.SetUpdateOrder(110);

            this.world.ContactManager.BeginContact += this.HandleBeginContact;
            this.world.ContactManager.EndContact += this.HandleEndContact;
        }
        #endregion

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            this.UpdatePosition();
        }
        #endregion

        #region Utility Methods
        public void SetOffset(Vector2 offset)
        {
            if (offset.Length() > _reach)
            {
                offset.Normalize();
                offset *= _reach;
            }

            if (this.Offset != offset)
            {
                this.Offset = offset;
                this.Awake = true;

                this.OnOffsetChanged?.Invoke(this, this.Offset);
            }

            // Immediately update the tractor beam position
            this.UpdatePosition();
        }

        public void Select(ShipPart target = null)
        {
            // First, ensure that the target is valid.
            if (target == null)
                target = this.GetOver();

            if (target != null)
            { // Only proceed if the target passes validation
                // Ensure that the old ship part gets released
                if (this.Selected != null)
                    this.Release();

                if (target.Root.IsBridge) // If the target is part of the current ship... detatch it.
                    target.DetatchFrom();

                // Select the new target
                this.Selected = target;
                _selectedFocusedId = this.Selected.Focused.Add();
                this.Joint = JointFactory.CreateWeldJoint(
                    this.world,
                    this.GetBody(),
                    this.Selected.GetBody(),
                    this.LocalCenter,
                    this.Selected.LocalCenter);

                this.OnSelected?.Invoke(this, this.Selected);
            }
            else
            {
                this.OnSelected?.Invoke(this, null);
            }
        }

        public void Release()
        {
            if (this.Selected != null)
            {
                this.world.RemoveJoint(this.Joint);

                var oldSelected = this.Selected;
                this.Selected.Focused.Remove(_selectedFocusedId);
                this.Selected.Position = this.Position - Vector2.Transform(this.Selected.LocalCenter, Matrix.CreateRotationZ(this.Selected.Rotation));
                this.Selected.Dirty = true;
                this.Selected = null;

                // The tractor beam is probably still over the item, so add it back to the contact manager
                _contacts.Add(oldSelected);

                this.OnReleased?.Invoke(this, oldSelected);
            }
        }

        private Boolean ValidateTarget(ShipPart target)
        {
            return (target != null) && ((!target.Root.IsBridge) || (target.Root.BridgeFor == this.Player && !target.IsBridge));
        }

        private void UpdatePosition()
        {
            if (this.Player.Bridge != null)
                this.SetTransform(this.Player.Bridge.WorldCenter + this.Offset, 0);
        }

        /// <summary>
        /// Get the current ship-part the tractor beam is currently over
        /// </summary>
        /// <returns></returns>
        public ShipPart GetOver()
        {
            var over = _contacts
                .Where(c => this.ValidateTarget(c))
                .OrderBy(c => Vector2.Distance(this.Position, c.Root.Position + Vector2.Transform(Vector2.Zero, c.LocalTransformation * Matrix.CreateRotationZ(c.Root.Rotation))))
                .FirstOrDefault();

            if (over != null && !over.IsRoot && !over.Root.IsBridge)
                return over.Root;

            return over;
        }
        #endregion

        #region Event Handlers
        private bool HandleBeginContact(Contact contact)
        {
            if(contact.FixtureA == _sensor && contact.FixtureB.UserData is ShipPart)
            {
                _contacts.Add(contact.FixtureB.UserData as ShipPart);
            }
            else if(contact.FixtureB == _sensor && contact.FixtureA.UserData is ShipPart)
            {
                _contacts.Add(contact.FixtureA.UserData as ShipPart);
            }

            return true;
        }

        private void HandleEndContact(Contact contact)
        {
            if (contact.FixtureA == _sensor && contact.FixtureB.UserData is ShipPart)
            {
                _contacts.Remove(contact.FixtureB.UserData as ShipPart);
            }
            else if (contact.FixtureB == _sensor && contact.FixtureA.UserData is ShipPart)
            {
                _contacts.Remove(contact.FixtureA.UserData as ShipPart);
            }
        }
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            base.read(im);

            this.Player = this.entities.GetById(im.ReadGuid()) as Player;
        }

        protected override void write(NetOutgoingMessage om)
        {
            base.write(om);

            om.Write(this.Player.Id);
        }
        #endregion
    }
}
