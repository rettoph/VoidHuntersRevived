using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame;
using Guppy.MonoGame.Constants;
using Guppy.MonoGame.Messages;
using Guppy.MonoGame.Providers;
using Guppy.MonoGame.UI;
using Guppy.MonoGame.Utilities.Cameras;
using Guppy.Network;
using Guppy.Network.Enums;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Diagnostics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Domain.Client.Debuggers
{
    [GuppyFilter<LocalGameGuppy>()]
    internal sealed class AetherDebugger : SimpleDrawableGameComponent,
        ISubscriber<Toggle<AetherDebugger>>
    {
        private class DebugData
        {
            public DebugView View;
            public ISimulation Simulation;
            public bool Enabled;
            public Num.Vector3 ColorVector3;

            public Color Color => new Color(ColorVector3.X, ColorVector3.Y, ColorVector3.Z);

            public DebugData(
                DebugView view, 
                ISimulation simulation, 
                bool enabled)
            {
                this.View = view;
                this.Simulation = simulation;
                this.Enabled = enabled;
                this.ColorVector3 = new Num.Vector3(0.9f, 0.7f, 0.7f);
            }

            public void UpdateColor()
            {
                this.View.DefaultShapeColor = this.Color;
                this.View.InactiveShapeColor = this.Color;
                this.View.KinematicShapeColor = this.Color;
                this.View.SleepingShapeColor = this.Color;
                this.View.StaticShapeColor = this.Color;
            }
        }

        private readonly IGlobalSimulationService _simulations;
        private readonly GraphicsDevice _graphics;
        private readonly ContentManager _content;
        private readonly Camera2D _camera;
        private readonly NetScope _netScope;
        private DebugData[] _data;
        private readonly DebugViewFlags[] _debugFlags;
        private readonly Menu _menu;
        
        public AetherDebugger(
            Camera2D camera,
            IGlobalSimulationService simulations,
            IMenuProvider menus,
            GraphicsDevice graphics,
            ContentManager content,
            NetScope netScope)
        {
            _camera = camera;
            _simulations = simulations;
            _graphics = graphics;
            _content = content;
            _netScope = netScope;
            _data = new DebugData[_simulations.Instances.Count()];
            _menu = menus.Get(MenuConstants.Debug);
            _debugFlags = Enum.GetValues<DebugViewFlags>()
                .Except(DebugViewFlags.None.Yield())
                .ToArray();

            this.IsEnabled = false;
            this.Visible = false;
        }

        public override void Initialize()
        {
            _menu.Add(new MenuItem()
            {
                Label = "Aether",
                OnClick = Toggle<AetherDebugger>.Instance
            });

            for (var i = 0; i < _simulations.Instances.Count; i++)
            {
                _data[i] = new DebugData(
                    view: new DebugView(_simulations.Instances[i].Aether),
                    simulation: _simulations.Instances[i],
                    enabled: false);

                _data[i].View.LoadContent(_graphics, _content);
                _data[i].UpdateColor();
            }
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            if(ImGui.Begin("Aether Debug View", ImGuiWindowFlags.AlwaysAutoResize))
            {
                foreach(DebugData data in _data)
                {
                    var header = $"{data.Simulation.NetScope().Peer!.Type} - {data.Simulation.Type}";
                    if(ImGui.CollapsingHeader(header))
                    {
                        ImGui.PushID($"{header}_toggle");
                        if(ImGui.Button($"Toggle"))
                        {
                            data.Enabled = !data.Enabled;
                        }

                        if(data.Enabled)
                        {
                            foreach(var flag in _debugFlags)
                            {
                                var state = data.View.Flags.HasFlag(flag);
                                ImGui.PushID($"{header}_flag_{flag}");
                                if (ImGui.Button($"Toggle {flag} ({state})"))
                                {
                                    if(state)
                                    {
                                        data.View.RemoveFlags(flag);
                                        continue;
                                    }

                                    data.View.AppendFlags(flag);
                                }
                            }

                            ImGui.PushID($"{header}_color_1");
                            if (ImGui.ColorPicker3("Shape Color", ref data.ColorVector3, ImGuiColorEditFlags.NoTooltip))
                            {
                                data.UpdateColor();
                            }
                        }
                    }

                    if (data.Enabled)
                    {
                        data.View.RenderDebugData(_camera.Projection, _camera.View);
                    }
                }
            }

            ImGui.End();
        }

        public void Process(in Toggle<AetherDebugger> message)
        {
            this.Visible = !this.Visible;
        }
    }
}
