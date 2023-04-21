using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Utilities;
using Guppy.GUI;
using Guppy.GUI.Elements;
using Guppy.GUI.Loaders;
using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Editor.Messages;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;

namespace VoidHuntersRevived.Domain.Editor.Systems
{
    [GuppyFilter<IEditorGuppy>]
    internal sealed class RigidEditorSystem : EntityDrawSystem, ISubscriber<VertexInput>
    {
        private readonly IEditor _editor;
        private readonly IVerticesBuilder _builder;
        private bool _enabled;
        private TextButton _button;
        private RigidConfiguration _configuration;

        public BlockList StageBlockList => BlockList.CreateWhitelist(Stages.Editor);

        public RigidEditorSystem(IEditor editor, IVerticesBuilder builder) : base(Aspect.All())
        {
            _editor = editor;
            _builder = builder;
            _enabled = false;
            _configuration = new RigidConfiguration();
            _button = new TextButton()
            {
                Text = "Add Rigid Component",
            };

            _button.OnReleased += this.HandleActivateClicked;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            var container = new Container<Element>();
            container.Add(_button);

            _editor.ControlPanel.Add(container);
        }

        public override void Draw(GameTime gameTime)
        {
            _builder.Draw();
        }

        private void HandleActivateClicked(Button<Label> sender, IMessage? args)
        {
            if(!_enabled)
            {
                _editor.ShipPartResource.Add(_configuration);
                _button.Text = "Create Rigid Fixture";
                _enabled = true;
                return;
            }

            if(_builder.Building)
            {
                IEnumerable<Vector2> vertices = _builder.Build();
                _configuration.Shapes = _configuration.Shapes.Concat(new PolygonShape(new Vertices(vertices), 1f).Yield()).ToArray();

                return;
            }

            _builder.Start(true);
            _button.Text = "Finish Rigid Fixture";
        }

        public void Process(in VertexInput message)
        {
            if(!_builder.Building)
            {
                return;
            }

            switch (message.Action)
            {
                case VertexInput.Actions.Add:
                    _builder.Add(message.Value);
                    break;
                case VertexInput.Actions.Remove:
                    _builder.Remove();
                    break;
            }
        }
    }
}
