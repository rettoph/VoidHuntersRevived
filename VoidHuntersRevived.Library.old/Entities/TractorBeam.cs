using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorBeam : FarseerEntity, ITractorBeam
    {
        public ITractorableEntity SelectedEntity { get; private set; }
        public Vector2 Position { get; set; }

        private WeldJoint _joint;
        private MainScene _scene;

        public event EventHandler<ITractorBeam> OnSelect;
        public event EventHandler<ITractorBeam> OnRelease;

        public TractorBeam(IPlayer player, EntityInfo info, IGame game) : base(info, game)
        {
            this.SelectedEntity = null;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = this.Scene as MainScene;

            this.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
            this.Body.SleepingAllowed = false;
        }

        public void TryRelease()
        {
            if(this.SelectedEntity != null)
            {
                this.SelectedEntity.Released();
                this.SelectedEntity = null;
                this.World.RemoveJoint(_joint);

                this.OnRelease?.Invoke(this, this);
            }
        }
        public void TrySelect(ITractorableEntity entity)
        {
            if (this.SelectedEntity == null && entity.CanBeSelectedBy(this))
            {
                this.SelectedEntity = entity;
                this.SelectedEntity.SelectedBy(this);
                this.OnSelect?.Invoke(this, this);

                this.SelectedEntity.Body.Position = this.Body.Position;
                this.SelectedEntity.SetEnabled(true);

                _joint = JointFactory.CreateWeldJoint(
                    this.World,
                    this.Body,
                    this.SelectedEntity.Body,
                    this.Body.LocalCenter,
                    this.SelectedEntity.Body.LocalCenter);

                _joint.DampingRatio = 1000f;
                _joint.FrequencyHz = 100f;
            }
        }


        public void Read(NetIncomingMessage im)
        {
            this.Body.Position = im.ReadVector2();

            if (im.ReadBoolean())
            { // The next boolean indicates wether or not the tractor beam has an object selected
                var selectedId = im.ReadInt64();

                if (this.SelectedEntity == null)
                { // We only need to do anything if the tractor beam doesnt current have a selection
                    var target = _scene.NetworkEntities.GetById(selectedId);

                    // Only bother trying if the input entity is a tractorable object
                    if (target is ITractorableEntity)
                        this.TrySelect(target as ITractorableEntity);
                }
            }
            else
            {
                if (this.SelectedEntity != null)
                { // try to release the input entity
                    this.TryRelease();
                }
            }
        }

        public void Write(NetOutgoingMessage om)
        {
            om.Write(this.Body.Position);

            if (this.SelectedEntity == null)
            {
                om.Write(false);
            }
            else
            {
                om.Write(true);
                om.Write(this.SelectedEntity.Id);
            }
        }
    }
}
