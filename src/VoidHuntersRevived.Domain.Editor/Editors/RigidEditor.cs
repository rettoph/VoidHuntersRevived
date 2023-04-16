using Guppy.Common;
using Guppy.MonoGame.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Editor.Constants;
using VoidHuntersRevived.Common.Editor.Messages;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Domain.Editor.Editors
{
    internal class RigidEditor : IShipPartEditor<RigidConfiguration>, ISubscriber<VertexInput>
    {
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;

        private RigidConfiguration _rigidConfiguration;


        public Type ComponentConfigurationType => typeof(RigidConfiguration);

        private IVerticesBuilder _verticesBuilder;

        public RigidEditor(
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            IVerticesBuilder verticesInput)
        {
            _primitiveBatch = primitiveBatch;
            _verticesBuilder = verticesInput;
            _rigidConfiguration = new RigidConfiguration();
        }

        public void Initialize(ShipPartResource shipPart)
        {
        }

        public void Draw(GameTime gameTime)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public void Process(in VertexInput message)
        {
            switch (message.Action)
            {
                case VertexInput.Actions.Add:
                    _verticesBuilder.Add(message.Value);
                    break;
                case VertexInput.Actions.Remove:
                    _verticesBuilder.Remove();
                    break;
            }

        }
    }
}
