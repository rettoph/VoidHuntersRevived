using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.EntityComponent.DependencyInjection;
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
        /// <para>Represents all platform instances.</para>
        /// 
        /// <para>
        /// When <see cref="NetworkAuthorization"/> is <see cref="NetworkAuthorization.Master"/>, this will contain
        /// only the <see cref="NetworkAuthorization.Master"/> instance. This is because platform actions taken on the
        /// server should only effect the server instance. The slave instance is used as a reference to know what has
        /// been broadcasted.
        /// </para>
        /// 
        /// <para>
        /// When <see cref="NetworkAuthorization"/> is <see cref="NetworkAuthorization.Slave"/>, this will contain
        /// the <see cref="NetworkAuthorization.Master"/> and <see cref="NetworkAuthorization.Master"/> instance.
        /// This is because platform actions taken on the client should alter the server instances in an effort to
        /// "predict" incoming server data.
        /// </para>
        /// </summary>
        private TAetherObject[] _platformInstances;

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
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Construct a new <see cref="TAetherObject"/> instance 
        /// representing <paramref name="authorization"/>.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        protected abstract TAetherObject BuildInstance(ServiceProvider provider, NetworkAuthorization authorization);

        /// <summary>
        /// Construct the internal aether <see cref="TAetherObject"/> instances.
        /// This should be called by any owning implementation class.
        /// </summary>
        /// <param name="provider"></param>
        protected internal void BuildAetherInstances(ServiceProvider provider)
        {
            _instances = new Dictionary<NetworkAuthorization, TAetherObject>(2)
            {
                { NetworkAuthorization.Master, this.BuildInstance(provider, NetworkAuthorization.Master) },
                { NetworkAuthorization.Slave, this.BuildInstance(provider, NetworkAuthorization.Slave) }
            };

            switch (provider.Settings.Get<NetworkAuthorization>())
            {
                case NetworkAuthorization.Master:
                    _localInstance = _instances[NetworkAuthorization.Master];
                    _platformInstances = new TAetherObject[]
                    {
                        _instances[NetworkAuthorization.Master]
                    };
                    break;
                case NetworkAuthorization.Slave:
                    _localInstance = _instances[NetworkAuthorization.Slave];
                    _platformInstances = new TAetherObject[]
                    {
                        _instances[NetworkAuthorization.Master],
                        _instances[NetworkAuthorization.Slave]
                    };
                    break;
            }
        }

        /// <summary>
        /// Perform an action on every internal <see cref="TAetherObject"/> instance.
        /// </summary>
        /// <param name="action"></param>
        public void Do(Action<TAetherObject> action)
        {
            foreach (TAetherObject instance in _instances.Values)
                action(instance);
        }

        /// <summary>
        /// Perform an action on every internal <see cref="TAetherObject"/> instance.
        /// </summary>
        /// <param name="action"></param>
        public void Do(Action<NetworkAuthorization, TAetherObject> action)
        {
            foreach (var kvp in _instances)
                action(kvp.Key, kvp.Value);
        }
        #endregion
    }
}
