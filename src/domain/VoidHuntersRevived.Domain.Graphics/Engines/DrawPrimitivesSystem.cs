using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common.Services;

namespace VoidHuntersRevived.Domain.Graphics.Systems
{
    public class DrawPrimitivesSystem(
        IPrimitiveService primitiveService
    ) : ISceneSystem,
        IInitializeSystem,
        IDrawSystem
    {
        private readonly IPrimitiveService _primitiveService = primitiveService;
        private readonly ActionSequenceGroup<PrimitiveSequenceGroupEnum, GameTime> _primitiveActions = new(true);

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Initialize)]
        public void Initialize()
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