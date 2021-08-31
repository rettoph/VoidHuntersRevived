using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.DependencyInjection;
using Guppy.Network.Enums;
using Guppy.Utilities;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Entities.Aether
{
    /// <summary>
    /// An obect that wrap an Aether object, such as a
    /// world or body or fixture and contain 1 to 2 instances of those objects.
    /// Those instances represent the current client instance and the virtually
    /// simulated server instance. This allows <see cref="IWorldObject"/> to maintain
    /// and manipulate their internal data based on the current 
    /// <see cref="NetworkAuthorization"/> of the peer.
    /// </summary>
    public abstract class BaseAetherWrapper<TAetherObject> : Frameable
    {
        #region Private Fields
        /// <summary>
        /// An lookup table of all internal <see cref="TAetherObject"/> within,
        /// grouped by their respective <see cref="NetworkAuthorization"/> value.
        /// </summary>
        private Dictionary<NetworkAuthorization, TAetherObject> _instances;

        /// <summary>
        /// Represents the current <see cref="NetworkAuthorization"/>'s Aether
        /// object representation.
        /// </summary>
        private TAetherObject _localInstance;
        #endregion

        #region Public Properties
        /// <summary>
        /// An lookup table of all internal <see cref="TAetherObject"/> within,
        /// grouped by their respective <see cref="NetworkAuthorization"/> value.
        /// </summary>
        public IReadOnlyDictionary<NetworkAuthorization, TAetherObject> Instances => _instances;

        /// <summary>
        /// Represents the current <see cref="NetworkAuthorization"/>'s Aether
        /// object representation.
        /// </summary>
        public TAetherObject LocalInstance => _localInstance;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            _instances.Clear();
            _localInstance = default;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Construct a new <see cref="TAetherObject"/> instance 
        /// representing <paramref name="authorization"/>.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        protected abstract TAetherObject BuildInstance(GuppyServiceProvider provider, NetworkAuthorization authorization);

        /// <summary>
        /// Construct the internal aether <see cref="TAetherObject"/> instances.
        /// This should be called by any owning implementation class.
        /// </summary>
        /// <param name="provider"></param>
        protected internal void BuildAetherInstances(GuppyServiceProvider provider)
        {
            _instances = new Dictionary<NetworkAuthorization, TAetherObject>(2)
            {
                { NetworkAuthorization.Master, this.BuildInstance(provider, NetworkAuthorization.Master) },
                { NetworkAuthorization.Slave, this.BuildInstance(provider, NetworkAuthorization.Slave) }
            };

            _localInstance = _instances[provider.Settings.Get<NetworkAuthorization>()];
        }

        /// <summary>
        /// Preform an action on every internal <see cref="TAetherObject"/> instance.
        /// </summary>
        /// <param name="action"></param>
        protected void Do(Action<TAetherObject> action)
        {
            foreach (TAetherObject instance in _instances.Values)
                action(instance);
        }
        #endregion
    }
}
