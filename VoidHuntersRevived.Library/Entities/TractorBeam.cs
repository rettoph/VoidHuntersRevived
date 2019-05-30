using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
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
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Entities
{
    public class TractorBeam : FarseerEntity
    {
        private EntityCollection _entities;
        private Single _reach;

        public Vector2 Offset { get; private set; }
        public Player Player { get; private set; }

        public event EventHandler<Vector2> OnOffsetChanged;

        public TractorBeam(Player player, EntityCollection entities, EntityConfiguration configuration, VoidHuntersWorldScene scene, IServiceProvider provider, ILogger logger) : base(configuration, scene, provider, logger)
        {
            _entities = entities;

            this.Player = player;
        }
        public TractorBeam(Guid id, EntityCollection entities, EntityConfiguration configuration, Scene scene, IServiceProvider provider, ILogger logger) : base(id, configuration, scene, provider, logger)
        {
            _entities = entities;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _reach = 25;

            this.IsSensor = true;

            this.CreateFixture(new CircleShape(1f, 1f));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Player.Bridge != null)
                this.Position = this.Player.Bridge.WorldCenter + this.Offset;
        }

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
    }
}
