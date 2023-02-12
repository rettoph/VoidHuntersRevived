using Guppy.MonoGame.Primitives;
using Guppy.MonoGame.UI;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Domain.Editor
{
    internal class RigidEditor : IShipPartEditor<RigidConfiguration>
    {
        private ImGuiBatch _imGuiBatch;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;

        private RigidConfiguration _rigidConfiguration;
        private DrawConfiguration _drawConfiguration;

        public Type ComponentConfigurationType => typeof(RigidConfiguration);

        public RigidEditor(
            ImGuiBatch imGuiBatch,
            PrimitiveBatch<VertexPositionColor> primitiveBatch)
        {
            _imGuiBatch = imGuiBatch;
            _primitiveBatch = primitiveBatch;
            _rigidConfiguration = new RigidConfiguration();
            _drawConfiguration = new DrawConfiguration();
        }

        public void Initialize(ShipPartResource shipPart)
        {
            // throw new NotImplementedException();
        }

        public void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }
    }
}
