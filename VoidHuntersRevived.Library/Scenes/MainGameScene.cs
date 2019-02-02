using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Scenes;

namespace VoidHuntersRevived.Library.Scenes
{
    /// <summary>
    /// The main game scee will controll the entirety of
    /// actual gameplay. The main menu and lobby will reside
    /// in other scenes
    /// </summary>
    public class MainGameScene : NetworkScene
    {
        #region Private Fields
        private INetworkEntity _dirtyNetworkEntity;
        private NetOutgoingMessage _om;
        #endregion

        #region Public Attributes
        public World World { get; private set; }
        public IGroup Group { get; private set; }
        #endregion

        #region Constructors
        public MainGameScene(
            IPeer peer,
            IServiceProvider provider,
            IGame game) : base(provider, game)
        {
            // By default the main game will take place in group 420
            this.Group = peer.Groups.GetById(420);

            this.SetEnabled(true);
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            // Create a new farseer world
            this.World = new World(Vector2.Zero);
        }
        #endregion

        #region Frame Methods
        public override void Update(GameTime gameTime)
        {
            // Update incoming messages first
            this.Group.Update();

            // Step world forward second
            this.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000); 

            // Update the entities third
            base.Update(gameTime);

            // Finally, push any dirty network objects to the conneced peer
            while (this.NetworkEntities.DirtyNetworkEntityQueue.Count > 0)
            {
                _dirtyNetworkEntity = this.NetworkEntities.DirtyNetworkEntityQueue.Dequeue();

                // Construct an update message
                _om = this.Group.CreateMessage("update");
                _dirtyNetworkEntity.Write(_om);

                this.Group.SendMessage(
                    _om,
                    NetDeliveryMethod.UnreliableSequenced);

                _dirtyNetworkEntity.Dirty = false;
            }
        }
        #endregion
    }
}
