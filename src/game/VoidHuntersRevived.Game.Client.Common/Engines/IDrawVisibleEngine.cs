using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Game.Client.Common.Graphics.Vertices;
using VoidHuntersRevived.Game.Client.Common.Services;

namespace VoidHuntersRevived.Game.Client.Common.Engines
{
    public interface IDrawVisibleEngine : IStepEngine<IVertexBufferManagerService<VertexInstanceVisible, Id<IEntityType>>>
    {
    }
}
