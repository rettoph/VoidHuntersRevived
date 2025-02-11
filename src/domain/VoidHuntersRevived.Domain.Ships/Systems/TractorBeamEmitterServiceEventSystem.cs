using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Exceptions;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Ships.Systems
{
    public class TractorBeamEmitterServiceEventSystem(
        INodeSocketService nodeSocketService,
        ITreeService treeService,
        ITeamService teamService,
        ILogger logger
    ) : ISceneSystem,
        IEventSystem<TractorBeamEmitter_Select>,
        IEventSystem<TractorBeamEmitter_Deselect>
    {
        private readonly INodeSocketService _nodeSocketService = nodeSocketService;
        private readonly ITreeService _treeService = treeService;
        private readonly ITeamService _teamService = teamService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<EventSequenceGroupEnum>(EventSequenceGroupEnum.Process)]
        public void Process(in VhId eventId, TractorBeamEmitter_Select data)
        {
            try
            {
                EntityLocalId cloneLocalId = this._treeService.Spawn(
                    sourceId: eventId,
                    globalId: eventId.ToGlobalEntityId(1),
                    team: this._teamService.GetDefaultTeam(),
                    treeTemplateKey: Resources.EntityTemplates.Ship.ChainEntityTemplate,
                    nodes: data.TargetData,
                    initializer: (IEntityService entities, in InitializingEntity entity) =>
                    {
                        if (!entities.Query.TryGetLocalId(data.TractorBeamEmitterGlobalId, out EntityLocalId tractorBeamEmitterLocalId))
                        {
                            throw new ArgumentException($"Unable to locate {nameof(TractorBeamEmitter)} {data.TractorBeamEmitterGlobalId.Value}");
                        }

                        entity.Initializer.Init<Body>(new Body(entity.LocalId, data.Transform));
                        entity.Initializer.Init<Tractorable>(new Tractorable(tractorBeamEmitterLocalId));
                    });
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "Exception thrown");
#if DEBUG
                throw;
#endif
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }

        [SequenceGroup<EventSequenceGroupEnum>(EventSequenceGroupEnum.Process)]
        public void Process(in VhId eventId, TractorBeamEmitter_Deselect data)
        {
            try
            {
                if (data.AttachToSocketVhId.HasValue && this._nodeSocketService.TryGetNodeSocket(data.AttachToSocketVhId.Value, out NodeSocket attachToSocket))
                { // Spawn a new piece attached to the input node
                    this._nodeSocketService.Spawn(eventId, attachToSocket, data.TargetData);
                }
                else
                { // Spawn a new free floating chain
                    EntityLocalId cloneLocalId = this._treeService.Spawn(
                        sourceId: eventId,
                        globalId: eventId.ToGlobalEntityId(2),
                        team: this._teamService.GetDefaultTeam(),
                        treeTemplateKey: Resources.EntityTemplates.Ship.ChainEntityTemplate,
                        nodes: data.TargetData,
                        initializer: (IEntityService entities, in InitializingEntity entity) =>
                        {
                            entity.Initializer.Init<Body>(new Body(entity.LocalId, data.Transform));
                            entity.Initializer.Init<Tractorable>(new Tractorable(default));
                        });
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "Exception thrown");
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }
    }
}
