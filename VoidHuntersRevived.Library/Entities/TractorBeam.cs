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

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorBeam : FarseerEntity
    {
        #region Private Fields
        private EntityCollection _entities;
        private Single _reach;
        private Guid _selectedFocusedId;
        private Fixture _sensor;
        private List<ShipPart> _contacts;
        private World _world;
        private WeldJoint _joint;
        #endregion

        #region Public Attributes
        public Vector2 Offset { get; private set; }
        public Player Player { get; private set; }
        public ShipPart Selected { get; private set; }
        #endregion

        #region Events
        public event EventHandler<Vector2> OnOffsetChanged;
        public event EventHandler<ShipPart> OnSelected;
        public event EventHandler<ShipPart> OnReleased;
        #endregion

        #region Constructors
        public TractorBeam(Player player, EntityCollection entities, EntityConfiguration configuration, VoidHuntersWorldScene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
            _entities = entities;

            this.Player = player;
        }
        public TractorBeam(Guid id, EntityCollection entities, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(id, configuration, scene, provider, logger)
        {
            _entities = entities;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _reach = 25;
            _contacts = new List<ShipPart>();
            _world = (this.scene as VoidHuntersWorldScene).World;

            this.IsSensor = true;

            _sensor = this.CreateFixture(new CircleShape(1f, 1f));

            _world.ContactManager.BeginContact += this.HandleBeginContact;
            _world.ContactManager.EndContact += this.HandleEndContact;
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Player.Bridge != null)
                this.Position = this.Player.Bridge.WorldCenter + this.Offset;
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
        }

        public void Select(ShipPart target = null)
        {
            // First, ensure that the target is valid.
            if(target == null)
            {
                target = _contacts
                    .Where(c => this.ValidateTarget(c))
                    .OrderBy(c => Vector2.Distance(this.WorldCenter, c.WorldCenter))
                    .FirstOrDefault();
            }

            if (this.ValidateTarget(target))
            { // Only proceed if the target passes validation
                // Ensure that the old ship part gets released
                if (this.Selected != null)
                    this.Release();

                // Select the new target
                this.Selected = target;
                _selectedFocusedId = this.Selected.Focused.Add();
                _joint = JointFactory.CreateWeldJoint(
                    _world,
                    this.GetBody(),
                    this.Selected.GetBody(),
                    this.LocalCenter,
                    this.Selected.LocalCenter);

                this.OnSelected?.Invoke(this, this.Selected);
            }
        }

        public void Release()
        {
            if (this.Selected != null)
            {
                _world.RemoveJoint(_joint);

                var oldSelected = this.Selected;
                this.Selected.Focused.Remove(_selectedFocusedId);
                this.Selected = null;

                this.OnReleased?.Invoke(this, oldSelected);
            }
        }

        private Boolean ValidateTarget(ShipPart target)
        {
            return target != null;
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

            this.Player = _entities.GetById(im.ReadGuid()) as Player;
        }

        protected override void write(NetOutgoingMessage om)
        {
            base.write(om);

            om.Write(this.Player.Id);
        }
        #endregion
    }
}
