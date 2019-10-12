using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Utilities
{
    /// <summary>
    /// Manages server side farseer clones.
    /// The clones in this class are rigidly placed
    /// and updated. The client side source objects
    /// simply lerp towards the server clones.
    /// 
    /// Each clone may be access via the source
    /// fixture or body.
    /// </summary>
    public sealed class ServerRender
    {
        #region Private Fields
        private Dictionary<Int32, Body> _bodies;
        private Dictionary<Int32, Fixture> _fixtures;
        #endregion

        #region Public Fields
        public World World { get; private set; }
        #endregion

        #region Constructor
        public ServerRender(World world)
        {
            _bodies = new Dictionary<Int32, Body>();
            _fixtures = new Dictionary<Int32, Fixture>();

            // Clone a new instance of the world.
            this.World = new World(world.Gravity);
        }
        #endregion

        #region Clone Methods
        /// <summary>
        /// Clone a recieved body and add it to the clone world
        /// </summary>
        /// <param name="source"></param>
        public Body CloneBody(Body source, Boolean deep = false)
        {
            if(deep)
                _bodies.Add(source.BodyId, source.DeepClone(this.World));
            else
                _bodies.Add(source.BodyId, source.Clone(this.World));

            return _bodies[source.BodyId];
        }

        /// <summary>
        /// Clone a recieved fixture and add it to the clone instance of its body.
        /// </summary>
        /// <param name="source"></param>
        public Fixture CloneFixture(Fixture source)
        {
            _fixtures.Add(source.FixtureId, source.CloneOnto(_bodies[source.Body.BodyId]));

            return _fixtures[source.FixtureId];
        }
        #endregion

        #region Destroy Methods
        /// <summary>
        /// Destroy a cloned body based on its source
        /// </summary>
        /// <param name="source"></param>
        public void DestroyBody(Body source)
        {
            _bodies[source.BodyId]?.Dispose();
            _bodies.Remove(source.BodyId);
        }

        /// <summary>
        /// Destroy a cloned fixture based on its source
        /// </summary>
        /// <param name="source"></param>
        public void DestroyFixture(Fixture source)
        {
            _fixtures[source.FixtureId].Dispose();
            _fixtures.Remove(source.FixtureId);
        }
        #endregion

        #region GetBody Methods
        public Body GetBodyById(Int32 sourceId)
        {
            return _bodies[sourceId];
        }

        public Body GetBody(Body source)
        {
            return this.GetBodyById(source.BodyId);
        }
        #endregion

        #region GetFixture Methods
        public Fixture GetFixtureById(Int32 sourceId)
        {
            return _fixtures[sourceId];
        }

        public Fixture GetFixture(Fixture source)
        {
            return this.GetFixtureById(source.FixtureId);
        }
        #endregion
    }
}
