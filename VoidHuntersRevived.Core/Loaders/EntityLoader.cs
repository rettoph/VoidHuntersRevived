using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using System.Linq;
using VoidHuntersRevived.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Core.Loaders
{
    public class EntityLoader : ILoader
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private List<RegisterdEntityInfo> _registeredEntityInfoList;
        private Dictionary<String, EntityInfo> _entityInfoTable;
        public IReadOnlyCollection<EntityInfo> EntityInfoCollection;

        #region Structs
        private struct RegisterdEntityInfo
        {
            public readonly String Handle;
            public readonly Type Type;
            public readonly String NameHandle;
            public readonly String DescriptionHandle;
            public readonly Int32 Priority;
            public readonly Object Data;

            public RegisterdEntityInfo(String handle, Type type, String nameHandle, String descriptionHandle, Int32 priority = 0, Object data = null)
            {
                this.Handle = handle;
                this.Type = type;
                this.NameHandle = nameHandle;
                this.DescriptionHandle = descriptionHandle;
                this.Priority = priority;
                this.Data = data;
            }
        }
        #endregion

        public EntityLoader(IServiceProvider provider, ILogger logger)
        {
            _provider = provider;
            _logger = logger;
            _registeredEntityInfoList = new List<RegisterdEntityInfo>();
        }

        public void Register<TEntity>(
            String handle,
            String nameHandle,
            String descriptionHandle,
            Object data = null,
            Int32 priority = 0)
            where TEntity : class, IEntity
        {
            _logger.LogDebug($"Registering new IEntity<{typeof(TEntity).Name}>('{handle}') => nameHandle: '{nameHandle}', descriptionHandle: '{descriptionHandle}'");

            _registeredEntityInfoList.Add(
                new RegisterdEntityInfo(
                    handle: handle,
                    type: typeof(TEntity),
                    nameHandle: nameHandle,
                    descriptionHandle: descriptionHandle,
                    priority: priority,
                    data: data));
        }

        /// <summary>
        /// Create an entity and automatically add it to a scene
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="scene"></param>
        /// <returns></returns>
        public TEntity Create<TEntity>(String handle, IScene scene)
            where TEntity : class, IEntity
        {
            var type = typeof(TEntity);

            _logger.LogDebug($"Attempting to create new IEntity<{type.Name}>('{handle}')");
            if(!_entityInfoTable.ContainsKey(handle) || !type.IsAssignableFrom(_entityInfoTable[handle].Type))
            {
                _logger.LogError($"Unknown entity handle or type. Please ensure IEntity<{type.Name}>('{handle}') has been registered.");
                return default(TEntity);
            }

            return this.Create(_entityInfoTable[handle], scene) as TEntity;
        }
        /// <summary>
        /// Create a new entity with a given entity info object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="scene"></param>
        /// <returns></returns>
        public IEntity Create(EntityInfo info, IScene scene)
        {
            // Create & return a new instance of an entity
            IEntity entity = (IEntity)ActivatorUtilities.CreateInstance(_provider, info.Type, info);

            // begin entity initialization
            entity.TryBoot();
            entity.TryPreInitialize();

            // Add the entity to the input scene
            scene.Entities.Add(entity);

            // Complete initialization
            entity.TryInitialize();
            entity.TryPostInitialize();

            return entity;
        }

        public void Initialize()
        {
            _logger.LogDebug("Initializing top priority entities...");

            var stringLoader = _provider.GetLoader<StringLoader>();

            _entityInfoTable = _registeredEntityInfoList
                .GroupBy(rei => rei.Handle)
                .ToDictionary(
                    keySelector: g => g.Key,
                    elementSelector: g =>
                    {
                        var rei = g
                            .ToList()
                            .OrderByDescending(i => i.Priority)
                            .First();

                        return new EntityInfo(
                            rei.Handle,
                            rei.Type,
                            stringLoader.Get(rei.NameHandle),
                            stringLoader.Get(rei.DescriptionHandle),
                            data: rei.Data);
                    });

            // Create a new colllection containing all the registered and initialized entity info objects
            this.EntityInfoCollection = _entityInfoTable
                .Select(g => g.Value)
                .ToArray();
        }
    }
}
