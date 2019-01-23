using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorBeam : FarseerEntity, ITractorBeam
    {
        public Ship Ship { get; private set; }
        public ITractorableEntity SelectedEntity { get; private set; }
        public Vector2 Position { get; set; }

        private WeldJoint _joint;

        public event EventHandler<ITractorBeam> OnSelect;
        public event EventHandler<ITractorBeam> OnRelease;

        public TractorBeam(Ship ship, EntityInfo info, IGame game) : base(info, game)
        {
            this.Ship = ship;
            this.SelectedEntity = null;
            this.Position = this.Ship.Body.Position;
        }

        protected override void Initialize()
        {
            base.Initialize();

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
    }
}
