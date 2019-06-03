﻿using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Guppy.Collections;
using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    class ClientTractorBeamDriver : Driver
    {
        private TractorBeam _tractorBeam;
        private WeldJoint _joint;
        private World _world;
        private EntityCollection _entities;

        public ClientTractorBeamDriver(VoidHuntersClientWorldScene scene, EntityCollection entities, TractorBeam tractorBeam, IServiceProvider provider, ILogger logger) : base(tractorBeam, provider, logger)
        {
            _world = scene.ServerWorld;
            _tractorBeam = tractorBeam;
            _entities = entities;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            _tractorBeam.ActionHandlers["update:offset"] = this.HandleUpdateOffsetAction;
            _tractorBeam.ActionHandlers["select"] = this.HandleSelectAction;
            _tractorBeam.ActionHandlers["release"] = this.HandleReleaseAction;
            _tractorBeam.ActionHandlers["release:force"] = this.HandleReleaseForceAction;

            _tractorBeam.OnSelected += this.HandleSelected;
            _tractorBeam.OnReleased += this.HandleReleased;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #region Action Handlers
        private void HandleUpdateOffsetAction(NetIncomingMessage obj)
        {
            _tractorBeam.SetOffset(obj.ReadVector2());
        }

        private void HandleSelectAction(NetIncomingMessage obj)
        {
            _tractorBeam.Select(
                _entities.GetById(obj.ReadGuid()) as ShipPart);
        }

        private void HandleReleaseAction(NetIncomingMessage obj)
        {
            if (_tractorBeam.Selected?.Id == obj.ReadGuid())
                _tractorBeam.Release();
        }

        private void HandleReleaseForceAction(NetIncomingMessage obj)
        {
            if(_tractorBeam.Selected != null)
            {
                this.logger.LogWarning($"Recieving force release message for TractorBeam({_tractorBeam.Id})");
                _tractorBeam.Release();
            }
        }
        #endregion

        #region Event Handlers
        private void HandleSelected(object sender, ShipPart target)
        {
            if (target != null)
            {
                // When selected, a client side tractor beam should never sleep
                _tractorBeam.SleepingAllowed = false;

                // When the tractor beam selects an object we must create a new joint to simulate the changes
                var beamBody = ClientFarseerEntityDriver.ServerBody[_tractorBeam];
                var targetBody = ClientFarseerEntityDriver.ServerBody[target];

                _joint = JointFactory.CreateWeldJoint(
                    _world,
                    beamBody,
                    targetBody,
                    beamBody.LocalCenter,
                    targetBody.LocalCenter);
            }
        }

        private void HandleReleased(object sender, ShipPart e)
        {
            _tractorBeam.SleepingAllowed = true;

            _world.RemoveJoint(_joint);
        }
        #endregion
    }
}
