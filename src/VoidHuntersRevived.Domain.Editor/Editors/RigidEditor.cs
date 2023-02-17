using Guppy.Common;
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
using VoidHuntersRevived.Common.Editor.Constants;
using VoidHuntersRevived.Common.Editor.Messages;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Domain.Editor.Editors
{
    internal class RigidEditor : IShipPartEditor<RigidConfiguration>, ISubscriber<VertexInput>
    {
        private ImGuiBatch _imGuiBatch;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatch;

        private RigidConfiguration _rigidConfiguration;


        public Type ComponentConfigurationType => typeof(RigidConfiguration);

        private IVerticesBuilder _verticesBuilder;

        public RigidEditor(
            ImGuiBatch imGuiBatch,
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            IVerticesBuilder verticesInput)
        {
            _imGuiBatch = imGuiBatch;
            _primitiveBatch = primitiveBatch;
            _verticesBuilder = verticesInput;
            _rigidConfiguration = new RigidConfiguration();
        }

        public void Initialize(ShipPartResource shipPart)
        {
        }

        public void Draw(GameTime gameTime)
        {
            uint dockId = ImGui.GetID(DockSpaces.Editor);
            ImGui.SetNextWindowDockID(dockId, ImGuiCond.Always);

            if (ImGui.Begin(nameof(RigidEditor), ImGuiWindowFlags.AlwaysAutoResize))
            {
                if(!_verticesBuilder.Building && ImGui.Button("Add"))
                {
                    _verticesBuilder.Start(closed: true);
                }

                if(_verticesBuilder.Building)
                {
                    ImGui.Text($"Snap ({_verticesBuilder.Snap})");
                    if(ImGui.Button("Toggle"))
                    {
                        _verticesBuilder.Snap = !_verticesBuilder.Snap;
                    }

                    ImGui.Text("Degrees");
                    ImGui.InputFloat("Degrees", ref _verticesBuilder.Degrees);

                    ImGui.Text("Side Length");
                    ImGui.InputFloat("Length", ref _verticesBuilder.Length);
                }
            }

            ImGui.End();

            _verticesBuilder.Draw();
        }

        public void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public void Process(in VertexInput message)
        {
            if(ImGui.GetIO().WantCaptureMouse)
            {
                return;
            }

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
