using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Engines
{
    [AutoLoad]
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    public class DrawPrimitivesEngine(
        IPrimitiveService primitiveService,
        Camera2D camera
    ) : StrategyEngine,
        IOnInitializeEngine,
        IOnDrawEngine,
        IQueryingEntitiesEngine
    {
        private readonly IPrimitiveService _primitiveService = primitiveService;
        private readonly Camera2D _camera = camera;
        private readonly ActionSequenceGroup<PrimitiveSequenceGroupEnum, IDrawPrimitiveContext> _primitiveActions = new(true);
        private DrawPrimitiveContext _context = default!;

        public EntitiesDB entitiesDB
        {
            set
            {
                _context = new DrawPrimitiveContext()
                {
                    Camera = _camera,
                    EntitiesDb = value,
                };
            }
        }

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.Initialize)]
        public void OnInitialize(IStrategy strategy)
        {
            _primitiveActions.Add(_primitiveService.GetAll());
        }

        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.Draw)]
        public void OnDraw(GameTime gameTime)
        {
            _context.GameTime = gameTime;

            _primitiveActions.Invoke(_context);
        }
    }
}
