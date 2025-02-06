using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Graphics.Systems
{
    public class DrawPrimitivesSystem(
        IPrimitiveService primitiveService
    ) : StrategySystem,
        IOnInitializeSystem,
        IDrawSystem
    {
        private readonly IPrimitiveService _primitiveService = primitiveService;
        private readonly ActionSequenceGroup<PrimitiveSequenceGroupEnum, GameTime> _primitiveActions = new(true);

        [SequenceGroup<OnInitializeSequenceGroupEnum>(OnInitializeSequenceGroupEnum.Initialize)]
        public void OnInitialize(IStrategy strategy)
        {
            this._primitiveActions.Add(this._primitiveService.GetAll());
        }

        [SequenceGroup<DrawSequenceGroupEnum>(DrawSequenceGroupEnum.Draw)]
        public void Draw(GameTime gameTime)
        {
            this._primitiveActions.Invoke(gameTime);
        }
    }
}