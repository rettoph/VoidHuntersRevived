using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common.Providers;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Graphics.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Graphics.Common.Engines
{
    [Sequence<DrawSequence>(DrawSequence.Draw)]
    public abstract class BaseDrawVertexTypeEngine<TVertex> : StrategyEngine, IStepEngine<GameTime>
        where TVertex : unmanaged, IVertexType
    {
        public string name => nameof(BaseDrawVertexTypeEngine<TVertex>);

        private readonly IVertexTypeManagerProvider<TVertex> _vertexTypeManagerProvider;

        protected abstract Effect effect { get; }

        public BaseDrawVertexTypeEngine(IVertexTypeService vertexTypeService)
        {
            _vertexTypeManagerProvider = vertexTypeService.GetByVertexType<TVertex>();
        }

        public virtual void Step(in GameTime param)
        {
            this.DrawAllVertexTypeManagers();
        }

        protected virtual void DrawAllVertexTypeManagers()
        {
            foreach (IVertexTypeManager<TVertex> vertexTypeManager in _vertexTypeManagerProvider.GetAll())
            {
                this.DrawVertexTypeManager(vertexTypeManager);
            }
        }

        protected virtual void DrawVertexTypeManager(IVertexTypeManager<TVertex> vertexTypeManager)
        {
            foreach (IVertexBuffer<TVertex> vertexBuffer in vertexTypeManager.VertexBuffers)
            {
                this.DrawVertexBuffer(vertexBuffer);
            }
        }

        protected virtual void DrawVertexBuffer(IVertexBuffer<TVertex> vertexBuffer)
        {
            if (vertexBuffer.InstanceCount == 0)
            {
                return;
            }

            vertexBuffer.Flush();
            vertexBuffer.Draw(this.effect);
            vertexBuffer.Clear();
        }
    }
}
