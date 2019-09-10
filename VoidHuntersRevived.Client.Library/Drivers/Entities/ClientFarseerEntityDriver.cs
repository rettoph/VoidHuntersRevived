using FarseerPhysics.Dynamics;
using GalacticFighters.Client.Library.Utilities;
using GalacticFighters.Library.Entities;
using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities
{
    [IsDriver(typeof(FarseerEntity), 90)]
    internal sealed class ClientFarseerEntityDriver : Driver<FarseerEntity>
    {
        #region Private Fields
        private ServerRender _server;
        private List<Fixture> _fixtures;
        #endregion

        #region Constructor
        public ClientFarseerEntityDriver(ServerRender server, FarseerEntity driven) : base(driven)
        {
            _server = server;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind required events...
            this.driven.Events.TryAdd<Body>("body:created", this.HandleBodyCreated);
            this.driven.Events.TryAdd<Body>("body:destroyed", this.HandleBodyDestroyed);
            this.driven.Events.TryAdd<Fixture>("fixture:created", this.HandleFixtureCreated);
            this.driven.Events.TryAdd<Fixture>("fixture:destroyed", this.HandleFixtureDestroyed);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the entities main body is created, we must clone a duplicate on the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="body"></param>
        private void HandleBodyCreated(object sender, Body body)
        {
            // Create a clone of the farseer entities body within the server render
            _server.CloneBody(body);
        }

        /// <summary>
        /// When the entities body is destroyed, we must destroy the clone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="body"></param>
        private void HandleBodyDestroyed(object sender, Body body)
        {
            // Destroy the server render for the driven entity
            _server.DestroyBody(body);
        }

        /// <summary>
        /// When a fixture is created, create a clone of it within the server render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fixture"></param>
        private void HandleFixtureCreated(object sender, Fixture fixture)
        {
            _server.CloneFixture(fixture);
        }

        /// <summary>
        /// When a fixture is destroyed, destroy its clone within the server render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fixture"></param>
        private void HandleFixtureDestroyed(object sender, Fixture fixture)
        {
            _server.DestroyFixture(fixture);
        }
        #endregion
    }
}
