using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Editor.Components;

namespace VoidHuntersRevived.Domain.Editor.Systems
{
    internal sealed class EditorPreviewSystem : EntityUpdateSystem
    {
        public static readonly AspectBuilder EditableAspect = Aspect.All(new[]
        {
            typeof(Editable),
        });

        private readonly IEditor _editor;
        private readonly ISimulationService _simulations;
        private World _world;
        private int _editorHash;

        public EditorPreviewSystem(
            IEditor editor,
            ISimulationService simulations) : base(EditableAspect)
        {
            _editor = editor;
            _simulations = simulations;
            _world = null!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _world = world;
        }

        public void Initialize(ISimulation simulation)
        {
            ParallelKey key = ParallelTypes.ShipPart.Create(0);

            // simulation.CreateEntity(key).MakeShipPart(_editor.ShipPartResource).Attach(new Editable());
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if (_editor.ShipPartResource.GetHashCode() != _editorHash || true)
            {
                foreach(int entityId in this.ActiveEntities)
                {
                    // _world.GetEntity(entityId).MakeShipPart(_editor.ShipPartResource);
                }
                
                _editorHash = _editor.ShipPartResource.GetHashCode();
            }
        }
    }
}
