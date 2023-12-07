using Guppy.Attributes;
using Guppy.Common;
using Guppy.Game;
using Guppy.Game.ImGui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines.Debug
{
    [AutoLoad]
    internal class AetherWorldDebugEngine : BasicEngine, ISimpleDebugEngine
    {
        private const string Bodies = nameof(Bodies);
        private const string Contacts = nameof(Contacts);

        private readonly AetherWorld _world;

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public AetherWorldDebugEngine(AetherWorld world)
        {
            _world = world;
            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(AetherWorld), Bodies, this.GetBodiesValue),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(AetherWorld), Contacts, this.GetContactsValue),
            };
        }

        private string GetBodiesValue()
        {
            return _world.BodyList.Count.ToString("#,###,##0");
        }

        private string GetContactsValue()
        {
            return _world.ContactCount.ToString("#,###,##0");
        }
    }
}
