using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Loaders;
using VoidHuntersRevived.Domain.Entities.Loaders;
using VoidHuntersRevived.Domain.Loaders;
using VoidHuntersRevived.Domain.Physics.Loaders;
using VoidHuntersRevived.Domain.Pieces.Loaders;
using VoidHuntersRevived.Domain.Ships.Loaders;
using VoidHuntersRevived.Domain.Simulations.Loaders;
using VoidHuntersRevived.Domain.Teams.Loaders;

namespace VoidHuntersRevived.Presentation.Core.Loaders
{
    [AutoLoad]
    internal class DomainAssemblyLoader : IAssemblyLoader
    {
        public void ConfigureAssemblies(IAssemblyProvider assemblies)
        {
            assemblies.Load(typeof(DomainLoader).Assembly);
            assemblies.Load(typeof(SimulationLoader).Assembly);
            assemblies.Load(typeof(PhysicsLoader).Assembly);
            assemblies.Load(typeof(EntityLoader).Assembly);
            assemblies.Load(typeof(PieceLoader).Assembly);
            assemblies.Load(typeof(ShipLoader).Assembly);
            assemblies.Load(typeof(TeamLoader).Assembly);
        }
    }
}
