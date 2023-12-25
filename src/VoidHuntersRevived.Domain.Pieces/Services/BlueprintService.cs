using Guppy.Resources.Providers;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal sealed partial class BlueprintService : IBlueprintService
    {
        private readonly Dictionary<Id<IBlueprint>, Blueprint> _blueprints;
        private readonly IPieceService _pieces;
        private readonly ILogger _logger;

        public BlueprintService(IPieceService pieces, IResourceProvider resources, ILogger logger, IEnumerable<BlueprintDto> blueprints)
        {
            _pieces = pieces;
            _logger = logger;
            _blueprints = new Dictionary<Id<IBlueprint>, Blueprint>();

            foreach (BlueprintDto dto in resources.GetAll<BlueprintDto>()
                .Select(x => x.Item2)
                .Concat(blueprints))
            {
                this.TryGetByDto(dto, out _);
            }
        }

        public IBlueprint GetById(Id<IBlueprint> id)
        {
            return _blueprints[id];
        }

        public IEnumerable<IBlueprint> GetAll()
        {
            return _blueprints.Values;
        }

        public bool TryGetByDto(BlueprintDto blueprintDto, [MaybeNullWhen(false)] out IBlueprint blueprint)
        {
            ref Blueprint? blueprintInstance = ref CollectionsMarshal.GetValueRefOrAddDefault(_blueprints, blueprintDto.Id, out bool exists);

            if (exists == false)
            {
                try
                {
                    _logger.Verbose("{ClassName}::{MethodName} - Preparing to create {BluePrint} instance for {BlueprintId} ({BlueprintName})", nameof(BlueprintService), nameof(TryGetByDto), nameof(IBlueprint), blueprintDto.Id, blueprintDto.Name);
                    blueprintInstance = new Blueprint(blueprintDto, _pieces);
                }
                catch (Exception e)
                {
                    _blueprints.Remove(blueprintDto.Id);
                    _logger.Error(e, "{ClassName}::{MethodName} - There was an exception creating blueprint instance", nameof(BlueprintService), nameof(TryGetByDto));
                    blueprint = default!;
                    return false;
                }
            }

            blueprint = blueprintInstance!;
            return true;
        }
    }
}
