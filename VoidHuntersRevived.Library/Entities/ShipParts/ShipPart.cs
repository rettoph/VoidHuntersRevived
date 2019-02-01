using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Providers;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Entities.Connections.Nodes;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.MetaData;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart : TractorableEntity
    {
        private Quaternion _rotationOffset;
        private Vector3 _scaleOffset;
        private Vector3 _translateOffset;

        protected ShipPartData ShipPartData { get; private set; }
        public Matrix TransformationOffsetMatrix { get; private set; }
        public Quaternion RotationOffset { get { return _rotationOffset; } }
        public Vector3 ScaleOffset { get { return _scaleOffset; } }
        public Vector3 TranslateOffset { get { return _translateOffset; } }

        public IPlayer BridgeFor { get; set; }

        public MaleConnectionNode MaleConnectionNode { get; private set; }
        public FemaleConnectionNode[] FemaleConnectionNodes { get; private set; }

        public Boolean Ghost { get; private set; }

        // The rootmost part of the current ship parts chain
        public ShipPart Root
        {
            get
            {
                return (this.MaleConnectionNode.Connection == null ? this : this.MaleConnectionNode.Connection.FemaleNode.Owner.Root);
            }
        }

        // The shipparts immediate parent (if any)
        public ShipPart Parent
        {
            get
            {
                return (this.MaleConnectionNode.Connection == null ? null : this.MaleConnectionNode.Connection.FemaleNode.Owner);
            }
        }

        // Represents the current fixture of the ship part.
        // This is used to interface with the cursor
        protected Shape CurrentShape;

        #region Constructors
        public ShipPart(EntityInfo info, IGame game) : base(info, game, "entity:ship_part_driver")
        {
            this.Construct(info.Data as ShipPartData);
        }
        public ShipPart(Int64 id, EntityInfo info, IGame game) : base(id, info, game, "entity:ship_part_driver")
        {
            this.Construct(info.Data as ShipPartData);
        }
        private void Construct(ShipPartData data)
        {
            this.ShipPartData = data;

            this.UpdateOrder = 100;

            this.Enabled = true;
        }
        #endregion

        protected override void Initialize()
        {
            base.Initialize();

            this.CurrentShape = this.CreateShape();
            this.Body.CreateFixture(this.CurrentShape);


            // Create the male connection node
            this.MaleConnectionNode = this.Scene.Entities.Create<MaleConnectionNode>("entity:connection_node:male", null, this.ShipPartData.MaleConnection, this);

            // Create the female connection nodes
            this.FemaleConnectionNodes = this.ShipPartData.FemaleConnections
                .Select(fcnd =>
                {
                    return this.Scene.Entities.Create<FemaleConnectionNode>("entity:connection_node:female", null, fcnd, this);
                })
                .ToArray();
            /*
            var fixture = this.Body.CreateFixture(new PolygonShape(new Vertices(new Vector2[] {
                new Vector2(-0.5f, -0.5f),
                new Vector2(0.5f, -0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-0.5f, 0.5f)
            }), 10f));

            var shape = fixture.Shape as PolygonShape;
            shape.Clone();
            */

            this.MaleConnectionNode.OnConnected += this.HandleMaleNodeConnected;
            this.MaleConnectionNode.OnDisconnected += this.HandleMaleNodeDisconneced;
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            this.Body.BodyType = BodyType.Dynamic;
            this.Body.Restitution = 1f;
            this.Body.Friction = 0f;
            this.Body.LinearDamping = 1f;
            this.Body.AngularDamping = 2f;

            this.SetGhost(true);
            this.UpdateOffsetFields();
        }

        /// <summary>
        /// Ghosted shipparts are "detached freefloating" parts. The will only collide
        /// with the world boundries. Nothing else, not even themselves.
        /// </summary>
        /// <param name="ghost"></param>
        public void SetGhost(Boolean ghost)
        {
            if (ghost != this.Ghost)
            {
                if (ghost)
                {
                    this.Body.CollidesWith = Category.Cat1;
                    this.Body.CollisionCategories = Category.Cat2;
                    this.Body.SleepingAllowed = true;
                    this.Body.Mass = 0f;
                    this.Body.IsBullet = false;
                    this.SetEnabled(false);

                    this.Ghost = ghost;
                }
                else
                {
                    this.Body.CollidesWith = Category.Cat1 | Category.Cat3;
                    this.Body.CollisionCategories = Category.Cat3;
                    this.Body.SleepingAllowed = false;
                    this.Body.Mass = 10f;
                    this.Body.IsBullet = true;
                    this.SetEnabled(true);

                    this.Ghost = ghost;
                }
            }
        }

        #region Create Shape Methods
        protected Shape CreateShape()
        {
            return new PolygonShape(new Vertices(this.ShipPartData.Vertices), 0.1f);
        }
        public Shape CreateShape(Matrix transformation)
        {
            var vertices = new Vertices(this.ShipPartData.Vertices);
            vertices.Transform(ref transformation);

            return new PolygonShape(vertices, 0.1f);
        }
        #endregion

        #region Attatch Methods
        public Boolean AttatchTo(FemaleConnectionNode female)
        {
            if (female.Connection != null)
                this.Game.Logger.LogCritical($"Unable to connect nodes, female connection already exists!");
            else if (this.MaleConnectionNode.Connection != null)
                this.Game.Logger.LogCritical("Unable to connect nodes, male connection already exists!");
            else
            { // Create a new connection instance
                this.Scene.Entities.Create<NodeConnection>("entity:connection:node", null, female, this.MaleConnectionNode);

                return true;
            }

            return false;
        }

        public override bool CanBeSelectedBy(ITractorBeam tractorBeam)
        {
            if (base.CanBeSelectedBy(tractorBeam))
            {
                return this.BridgeFor == null;
            }

            return false;
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.UpdateOffsetFields();
        }

        /// <summary>
        /// Offset fields are values that contain relative offsets to the
        /// parts current parent (if any)
        /// </summary>
        public void UpdateOffsetFields()
        {
            if(this.Parent == null)
            {
                this.TransformationOffsetMatrix = Matrix.CreateRotationZ(this.Body.Rotation);
            }
            else
            {
                this.TransformationOffsetMatrix =
                    (this.MaleConnectionNode.TranslationMatrix *
                    this.MaleConnectionNode.Connection.FemaleNode.TranslationMatrix)
                    * this.Parent.TransformationOffsetMatrix;
            }

            this.TransformationOffsetMatrix.Decompose(out _scaleOffset, out _rotationOffset, out _translateOffset);

            // Update the parts children rotational values too
            foreach (FemaleConnectionNode femaleNode in this.FemaleConnectionNodes)
                if (femaleNode.Connection != null)
                    femaleNode.Connection.MaleNode.Owner.UpdateOffsetFields();
        }

        /// <summary>
        /// Return a list of all available female connection nodes found
        /// within the current hull piece (and any of its children)
        /// </summary>
        /// <returns></returns>
        public List<FemaleConnectionNode> GetAvailabaleFemaleConnectioNodes(List<FemaleConnectionNode> addTo = null)
        {
            if (addTo == null)
                addTo = new List<FemaleConnectionNode>();

            foreach (FemaleConnectionNode female in this.FemaleConnectionNodes)
            {
                if (female.Connection == null)
                    addTo.Add(female);
                else if (female.Connection.MaleNode.Owner is ShipPart)
                    female.Connection.MaleNode.Owner.GetAvailabaleFemaleConnectioNodes(addTo);
            }

            return addTo;
        }
    }
}
