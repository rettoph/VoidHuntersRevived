using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Extensions.Svelto;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Common.Entities.Mockers
{
    public class EntityPredictiveStrategyMocker : PredictiveStrategyMocker
    {
        public EnginesRoot EnginesRoot { get; set; }
        public EntitiesDB EntitiesDB => this.EnginesRoot.GetEntitiesDB();
    }
}
