using Guppy.Attributes;
using Guppy.Common;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines.Debug
{
    [AutoLoad]
    internal class AetherWorldDebugEngine : BasicEngine, ISimpleDebugEngine
    {
        private const string Bodies = nameof(Bodies);
        private const string Contacts = nameof(Contacts);

        private readonly AetherWorld _world;

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public AetherWorldDebugEngine(AetherWorld world, IOptional<GraphicsDevice> graphics, IResourceProvider resources)
        {
            _world = world;
            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(typeof(AetherWorld).Name, Bodies, this.GetBodiesValue),
                new ISimpleDebugEngine.SimpleDebugLine(typeof(AetherWorld).Name, Contacts, this.GetContactsValue),
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
