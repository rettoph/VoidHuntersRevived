using Guppy;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.Collections.Concurrent;
using VoidHuntersRevived.Server.Utilities;
using xxHashSharp;

namespace VoidHuntersRevived.Library.Scenes
{
    /// <summary>
    /// NetworkScene's are scenes that will
    /// automatically track NetworkEntity instances
    /// and send server Create, Update, & Remove
    /// messages when neccessary. These actions
    /// are implemented via custom drivers found
    /// within the Client and Server projects
    /// respectively.
    /// </summary>
    public class NetworkScene : Scene
    {
        #region Private Fields
        /// <summary>
        /// A queue of all unhandled recieved actions.
        /// </summary>
        private ConcurrentQueue<NetIncomingMessage> _actions;
        private NetIncomingMessage _im;
        private VitalsManager _vitals;
        private Dictionary<UInt32, Type> _hashedTypes;
        private Dictionary<Type, UInt32> _typedHashes;
        #endregion

        #region Protected Properties
        protected Double actionCount { get; private set; }
        #endregion

        #region Internal Properties
        #endregion

        #region Public Properties
        public Group Group { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _vitals = provider.GetRequiredService<VitalsManager>();
            _actions = new ConcurrentQueue<NetIncomingMessage>();

            _hashedTypes = AssemblyHelper.GetTypesAssignableFrom<NetworkEntity>().ToDictionary(
                keySelector: t => xxHash.CalculateHash(Encoding.UTF8.GetBytes(t.AssemblyQualifiedName)));
            _typedHashes = _hashedTypes.ToDictionary(
                keySelector: kvp => kvp.Value,
                elementSelector: kvp => kvp.Key);
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Update the group utalized by the scopes vitals manager
            _vitals.SetGroup(this.Group);

            this.actionCount = 0;

            // TODO: Fix this. Shouldnt use ? but needs to for texture generator to work right now.
            this.Group?.Messages.TryAdd("entity:action", this.HandleNetworkEntityActionMessage);
        }

        public override void Dispose()
        {
            base.Dispose();

            _vitals.SetGroup(null);
            _actions.Clear();

            this.Group.Messages.TryRemove("entity:action", this.HandleNetworkEntityActionMessage);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            // Update the internal group
            this.Group.TryUpdate(gameTime);

            base.Update(gameTime);

            // Parse any new messages
            while (_actions.Any())
                if (_actions.TryDequeue(out _im))
                    this.entities.GetById<NetworkEntity>(_im.ReadGuid())?.Actions.TryInvoke(this, _im.ReadUInt32(), _im);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Return the hashed value of a network entity type.
        /// Useful for passing that info to the connected peer.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public UInt32 GetHashFromType(Type type)
        {
            return _typedHashes[type];
        }

        /// <summary>
        /// Convert a hash back into a type instance.
        /// Useful for reading a NetworkEntity type value
        /// sent from the peer.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Type GetTypeFromHash(UInt32 hash)
        {
            return _hashedTypes[hash];
        }
        #endregion

        #region Message Handlers
        private void HandleNetworkEntityActionMessage(object sender, NetIncomingMessage arg)
        {
            this.actionCount++;
            _actions.Enqueue(arg);
        }
        #endregion
    }
}
