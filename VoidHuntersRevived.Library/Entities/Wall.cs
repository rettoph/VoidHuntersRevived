using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Library.Entities
{
    public class Wall : FarseerEntity
    {
        private Body[] _rectangles;
        private Body _anchor;
        private Single _theta;
        private Single _distance;
        private Boolean _shrink;

        public Wall(EntityInfo info, IGame game) : base(info, game)
        {
            _rectangles = new Body[0];

            _shrink = false;
            _distance = 2;
        }

        /// <summary>
        /// Create a new wall configured to the inputed width and height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Configure(Single width, Single height, Single thickness = 1)
        {
            // Clear any pre-existing walls from the world
            foreach (Body body in _rectangles)
                this.World.RemoveBody(body);


            var halfWidth = width / 2;
            var halfHeight = height / 2;
            var halfThickness = thickness / 2;

            _rectangles = new Body[4];

            _rectangles[0] = BodyFactory.CreateRectangle(
                this.World,
                width + (thickness * 2),
                thickness,
                10f,
                new Vector2(0, -(halfHeight + halfThickness)),
                0,
                BodyType.Kinematic);

            _rectangles[1] = BodyFactory.CreateRectangle(
                this.World,
                thickness,
                height,
                10f,
                new Vector2((halfWidth + halfThickness), 0),
                0,
                BodyType.Kinematic);

            _rectangles[2] = BodyFactory.CreateRectangle(
                this.World,
                width + (thickness * 2),
                thickness,
                10f,
                new Vector2(0, (halfHeight + halfThickness)),
                0,
                BodyType.Kinematic);

            _rectangles[3] = BodyFactory.CreateRectangle(
                this.World,
                thickness,
                height,
                10f,
                new Vector2(-(halfWidth + halfThickness), 0),
                0,
                BodyType.Kinematic);

            foreach (Body body in _rectangles)
            {
                body.Restitution = 1f;
                body.Friction = 0;
                body.FixtureList[0].Restitution = 1f;
                body.FixtureList[0].Friction = 0f;

                body.OnCollision += this.HandleCollision;
            }
        }

        private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            fixtureB.Body.LinearVelocity -= contact.Manifold.LocalNormal;
            return true;
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if (_distance <= 0.0f || _distance >= 3)
                _shrink = !_shrink;

            var speed = 0.02f;
            _distance += (float)((_shrink ? -1f : 1f) * (8 * (speed / (Math.PI * 2))));

            _theta += speed;

            _rectangles[0].Rotation = _theta;
            _rectangles[0].Position = new Vector2((float)Math.Cos(_theta + 1.5708) * _distance, (float)Math.Sin(_theta + 1.5708) * _distance);
            
            _rectangles[2].Rotation = _theta;
            _rectangles[2].Position = new Vector2((float)Math.Cos(_theta - 1.5708) * _distance, (float)Math.Sin(_theta - 1.5708) * _distance);


            _rectangles[1].Rotation = _theta;
            _rectangles[1].Position = new Vector2((float)Math.Cos(_theta) * _distance, (float)Math.Sin(_theta) * _distance);

            _rectangles[3].Rotation = _theta;
            _rectangles[3].Position = new Vector2((float)Math.Cos(_theta + Math.PI) * _distance, (float)Math.Sin(_theta + Math.PI) * _distance);
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void HandleRemovedFromScene(object sender, ISceneObject e)
        {
            // Clear any pre-existing walls from the world
            foreach (Body body in _rectangles)
                this.World.RemoveBody(body);
        }
    }
}
