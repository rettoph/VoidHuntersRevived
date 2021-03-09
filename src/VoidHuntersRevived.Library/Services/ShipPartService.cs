using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Lists;
using Guppy.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Services
{
    /// <summary>
    /// Primary class used to manage all internal ShipPart Configuration data.
    /// This can auto import entire folders of *.vhsp files, and will
    /// allow for the exportation of ShipParts & their related data. Note,
    /// network drivers will also exist to pass along data between server/client
    /// instance upon connection. This allows for "mods" to be installed on the
    /// client level and used later within a server instance.
    /// 
    /// The side effect of this is that instances of this service should be scoped,
    /// so that old ship parts can be disposed of once the ServerGameScene is released.
    /// </summary>
    public sealed class ShipPartService : Service
    {
        #region Private Fields
        /// <summary>
        /// A table of all registered contexts, organized by
        /// their id.
        /// </summary>
        private Dictionary<UInt32, ShipPartContext> _contexts;

        private EntityList _entities;

        /// <summary>
        /// A list of all types implementing <see cref="ShipPartContext"/>
        /// </summary>
        private Dictionary<UInt32, Type> _contextTypes;
        #endregion

        #region Public Properties
        public ShipPartContext this[String name] => _contexts[name.xxHash()];
        public ShipPartContext this[UInt32 id] => _contexts[id];

        public IReadOnlyDictionary<UInt32, ShipPartContext> Contexts => _contexts;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _entities);

            _contexts = new Dictionary<uint, ShipPartContext>();
            _contextTypes = AssemblyHelper.Types
                .GetTypesAssignableFrom<ShipPartContext>()
                .ToDictionary(
                    keySelector: t => t.FullName.xxHash(),
                    elementSelector: t => t);
        }
        #endregion

        #region API Methods
        public ShipPart Create(ShipPartContext context, Action<ShipPart, ServiceProvider, ServiceConfiguration> setup = null)
            => context.Create(_entities, setup);

        public ShipPart Create(String name, Action<ShipPart, ServiceProvider, ServiceConfiguration> setup = null)
            => this.Create(_contexts[name.xxHash()], setup);

        public T Create<T>(String name, Action<T, ServiceProvider, ServiceConfiguration> setup = null)
            where T : ShipPart
                => this.Create(_contexts[name.xxHash()], (sp, p, c) => setup?.Invoke(sp as T, p, c)) as T;

        /// <summary>
        /// Register a new ship part context instance.
        /// </summary>
        /// <param name="context"></param>
        public void TryRegister(ShipPartContext context)
            => _contexts[context.Id] = context;

        /// <summary>
        /// Parse a stream and register the serialized context instance.
        /// </summary>
        /// <param name="stream"></param>
        public ShipPartContext TryRegister(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {

                // Extract the context type & create an empty instance.
                var context = Activator.CreateInstance(_contextTypes[reader.ReadUInt32()], reader.ReadString()) as ShipPartContext;
                context.TryRead(reader);
                this.TryRegister(context);

                return context;
            }
        }

        /// <summary>
        /// Import all files within the target location.
        /// </summary>
        /// <param name="target"></param>
        public void ImportAll(String path, String pattern = "*.vhsp")
        {
            foreach(String file in Directory.GetFiles(path, pattern))
                using(FileStream stream = File.OpenRead(file))
                    this.TryRegister(stream);
        }
        #endregion
    }
}
