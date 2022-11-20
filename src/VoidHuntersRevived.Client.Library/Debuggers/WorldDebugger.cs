using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame.UI;
using Guppy.MonoGame.UI.Constants;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Providers;
using VoidHuntersRevived.Library.Services;
using Num = System.Numerics;

namespace VoidHuntersRevived.Client.Library.Debuggers
{
    [AutoSubscribe]
    [GuppyFilter(typeof(ClientGameGuppy))]
    internal sealed class WorldDebugger : IImGuiDebugger, ISubscriber<Tick>
    {
        private IStepService _steps;
        private ITickService _ticks;
        private bool _open;

        public string Label { get; }

        public bool Open
        {
            get => _open;
            set => _open = value;
        }

        public WorldDebugger(IStepService steps, ITickService ticks)
        {
            _steps = steps;
            _ticks = ticks;
            _open = false;

            this.Label = "World";
        }

        public void Initialize(ImGuiBatch imGuiBatch)
        {
        }

        public void Draw(GameTime gameTime)
        {
            if(ImGui.Begin("World", ref _open))
            {
                ImGui.BeginTable("data", 2);

                ImGui.TableNextColumn();
                ImGui.Text($"ITickProvider Status");

                ImGui.TableNextColumn();
                ImGui.Text(_ticks.Provider.Status.ToString());

                ImGui.TableNextColumn();
                ImGui.Text("ITickProvider CurrentId");

                ImGui.TableNextColumn();
                ImGui.Text(_ticks.Provider.CurrentId.ToString("#,###,##0"));

                ImGui.TableNextColumn();
                ImGui.Text("ITickProvider AvailableId");

                ImGui.TableNextColumn();
                ImGui.Text(_ticks.Provider.AvailableId.ToString("#,###,##0"));

                ImGui.EndTable();
            }
            ImGui.End();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Process(in Tick message)
        {
            // Only track ticks with data
            if (!message.Any())
            {
                return;
            }
        }
    }
}
