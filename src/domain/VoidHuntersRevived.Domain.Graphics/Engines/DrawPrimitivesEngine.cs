using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Game.Graphics.Common;
using Guppy.Game.Graphics.Common.Attributes;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Engines
{
    [GraphicsEnabled]
    public class DrawPrimitivesEngine(
        IPrimitiveService primitiveService,
        ICamera2D camera
    ) : StrategyEngine,
        IOnInitializeEngine,
        IOnDrawEngine
    {
        private readonly IPrimitiveService _primitiveService = primitiveService;
        private readonly ICamera2D _camera = camera;
        private readonly ActionSequenceGroup<PrimitiveSequenceGroupEnum, GameTime> _primitiveActions = new(true);

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.Initialize)]
        public void OnInitialize(IStrategy strategy)
        {
            _primitiveActions.Add(_primitiveService.GetAll());
        }

        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.Draw)]
        public void OnDraw(GameTime gameTime)
        {
            _primitiveActions.Invoke(gameTime);
        }
    }
}
