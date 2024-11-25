using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Services;
using Guppy.Engine.Common.Loaders;
using VoidHuntersRevived.Domain.Entities.Modules;
using VoidHuntersRevived.Domain.Graphics.Modules;
using VoidHuntersRevived.Domain.Modules;
using VoidHuntersRevived.Domain.Physics.Modules;
using VoidHuntersRevived.Domain.Pieces.Modules;
using VoidHuntersRevived.Domain.Ships.Modules;
using VoidHuntersRevived.Domain.Simulations.Modules;
using VoidHuntersRevived.Domain.Teams.Modules;

namespace VoidHuntersRevived.Presentation.Core.Modules
{
    [AutoLoad]
    internal class DomainAssemblyLoader : IAssemblyLoader
    {
        public void ConfigureAssemblies(IAssemblyService assemblies)
        {
            assemblies.Load(typeof(DomainModule).Assembly);
            assemblies.Load(typeof(SimulationModule).Assembly);
            assemblies.Load(typeof(PhysicModule).Assembly);
            assemblies.Load(typeof(EntityModule).Assembly);
            assemblies.Load(typeof(PieceModule).Assembly);
            assemblies.Load(typeof(ShipModule).Assembly);
            assemblies.Load(typeof(TeamModule).Assembly);
            assemblies.Load(typeof(GraphicsLoader).Assembly);
        }
    }
}
