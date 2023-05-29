using Guppy.Common;
using Guppy.Common.Attributes;
using Guppy.Common.Collections;
using Guppy.ECS;
using Guppy.ECS.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [Sortable<ISimulationSystem>(int.MinValue)]
    internal sealed class EntitySystem : BasicSystem, IUpdateSystem, ISimulationSystem,
        ISubscriber<ISimulationEvent<CreateEntity>>,
        ISubscriber<ISimulationEventRevision<CreateEntity>>,
        ISubscriber<ISimulationEvent<DestroyEntity>>,
        ISubscriber<ISimulationEventRevision<DestroyEntity>>
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private readonly HashCache<ParallelKey> _destroyedEntities;
        private IEntityService _entities = null!;
        private IParallelEntityService _parallelEntities = null!;

        public EntitySystem(ILogger logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
            _destroyedEntities = new HashCache<ParallelKey>(TimeSpan.FromSeconds(10));
        }

        public void Initialize(IParallelComponentMapperService components, IParallelEntityService entities)
        {
            _parallelEntities = entities;
            _entities = _provider.GetRequiredService<IEntityService>();
        }

        public void Initialize(ISimulation simulation)
        {
            //
        }

        public void Update(GameTime gameTime)
        {
            _destroyedEntities.Prune();
        }

        public void Process(in ISimulationEvent<CreateEntity> message)
        {
            ParallelKey entityKey = message.Body.Key ?? message.Key.Step(1);
            Entity entity;

            if (message.Body.Factory is null)
            {
                entity = _entities.Create(message.Body.Type, message.Simulation.Configure);
            }
            else
            {
                entity = _entities.Create(message.Body.Type, message.Simulation.Configure, message.Body.Factory);
            }

            _parallelEntities.Map(entity.Id, entityKey, message.Simulation);

            message.Respond(entityKey);

            _logger.Debug($"{nameof(EntitySystem)}::{nameof(Process)}<{nameof(CreateEntity)}> - Created Entity {entity.Id}, ({message.Simulation.Type}), with key {entityKey})");
        }

        public void Process(in ISimulationEventRevision<CreateEntity> message)
        {
            if (message.Response is not ParallelKey entityKey)
            {
                return;
            }

            if(_destroyedEntities.Add(entityKey) > 1)
            {
                return;
            }

            int entityId = message.Simulation.GetEntityId(entityKey);
            _entities.Destroy(entityId);

            _logger.Debug($"{nameof(EntitySystem)}::{nameof(Process)}<{nameof(CreateEntity)}> - Reverted Entity creation {entityId}, ({message.Simulation.Type}, {message.Body.Key})");
        }

        public void Process(in ISimulationEvent<DestroyEntity> message)
        {
            _destroyedEntities.Add(message.Body.Key);
            int entityId = message.Simulation.GetEntityId(message.Body.Key);

            _logger.Debug($"{nameof(EntitySystem)}::{nameof(Process)}<{nameof(DestroyEntity)}> - Destroying Entity {entityId}, ({message.Simulation.Type}, {message.Body.Key})");

            if (message.Body.Backup)
            {
                _entities.Destroy(entityId, out EntityBackup backup);
                message.Respond(backup);
            }
            else
            {
                _entities.Destroy(entityId);
            }
        }

        public void Process(in ISimulationEventRevision<DestroyEntity> message)
        {
            int count = _destroyedEntities.Remove(message.Body.Key);
            if (count != 0)
            {
                _logger.Debug($"{nameof(EntitySystem)}::{nameof(Process)}<{nameof(DestroyEntity)}> - Unable to revert entity destruction {message.Simulation.Type}, {message.Body.Key.Value} => {_destroyedEntities.Count(message.Body.Key)}");

                return;
            }

            if (message.Response is not EntityBackup backup)
            {
                return;
            }

            Entity entity = _entities.Restore(backup);
            _parallelEntities.Map(entity.Id, message.Body.Key, message.Simulation);

            _logger.Debug($"{nameof(EntitySystem)}::{nameof(Process)}<{nameof(DestroyEntity)}> - Reverted Entity destruction {entity.Id}, ({message.Simulation.Type}, {message.Body.Key.Value})");
        }
    }
}
