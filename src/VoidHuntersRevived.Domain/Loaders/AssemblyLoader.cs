using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Loaders;
using VoidHuntersRevived.Domain.Entities.Loaders;
using VoidHuntersRevived.Domain.Physics.Loaders;
using VoidHuntersRevived.Domain.Pieces.Loaders;
using VoidHuntersRevived.Domain.Ships.Loaders;
using VoidHuntersRevived.Domain.Simulations.Loaders;

namespace VoidHuntersRevived.Domain.Loaders
{
    [AutoLoad]
    internal class AssemblyLoader : IAssemblyLoader
    {
        public void ConfigureAssemblies(IAssemblyProvider assemblies)
        {
            assemblies.Load(typeof(SimulationLoader).Assembly);
            assemblies.Load(typeof(PhysicsLoader).Assembly);
            assemblies.Load(typeof(EntityLoader).Assembly);
            assemblies.Load(typeof(PieceLoader).Assembly);
            assemblies.Load(typeof(PieceLoader).Assembly);
            assemblies.Load(typeof(ShipLoader).Assembly);
        }
    }
}
