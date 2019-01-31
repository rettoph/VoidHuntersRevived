using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Entities.Connections
{
    /// <summary>
    /// Represets a link between a tractor beam and itsselected item
    /// </summary>
    public class TractorBeamConnection : Entity
    {
        public TractorBeam TractorBeam { get; private set; }
        public ITractorableEntity Target { get; private set; }
        public ConnectionState State { get; private set; }

        private MainScene _scene;
        private WeldJoint _joint;

        public TractorBeamConnection(TractorBeam tractorBeam, ITractorableEntity target, EntityInfo info, IGame game) : base(info, game)
        {
            this.Enabled = false;
            this.Visible = false;

            this.State = ConnectionState.Connecting;

            this.TractorBeam = tractorBeam;
            this.Target = target;

            this.TractorBeam.Connect(this);
            this.Target.Connect(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as MainScene;

            _joint = JointFactory.CreateWeldJoint(
                _scene.World,
                this.TractorBeam.Body,
                this.Target.Body,
                this.TractorBeam.Body.LocalCenter,
                this.Target.Body.LocalCenter);

            this.State = ConnectionState.Connected;
        }

        public void Disconnect()
        {
            this.State = ConnectionState.Disconnecting;

            this.TractorBeam.Disconnect();
            this.Target.DisconnectTractorBeam();

            // Clear the weld joint
            _scene.World.RemoveJoint(_joint);

            this.State = ConnectionState.Disconnected;


            // Remove the entity
            this.Scene.Entities.Remove(this);
        }
    }
}
