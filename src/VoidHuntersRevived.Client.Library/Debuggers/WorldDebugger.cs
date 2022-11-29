using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.MonoGame.UI;
using Guppy.MonoGame.UI.Constants;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Resources.Providers;
using Guppy.Resources.Serialization.Json;
using ImGuiNET;
using ImPlotNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Providers;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Client.Library.Debuggers
{
    [AutoSubscribe]
    [GuppyFilter(typeof(ClientGameGuppy))]
    internal sealed class WorldDebugger : IImGuiDebugger, ISubscriber<Tick>, ISubscriber<Step>
    {
        private const int StepBufferSize = 256;

        private double _stepMaxValue;
        private double _stepMinValue;
        private double[] _intervalBuffer;
        private Buffer<double> _currentStepIntervalBuffer;
        private Buffer<double> _targetStepIntervalBuffer;
        private StepRemoteProvider _steps;
        private ITickService _ticks;
        private IJsonSerializer _json;
        private bool _open;

        public string Label { get; }

        public bool Open
        {
            get => _open;
            set => _open = value;
        }

        public WorldDebugger(StepRemoteProvider steps, ITickService ticks, IJsonSerializer json, ISettingProvider settings)
        {
            _steps = steps;
            _ticks = ticks;
            _json = json;
            _open = false;
            _currentStepIntervalBuffer = new Buffer<double>(StepBufferSize);
            _targetStepIntervalBuffer = new Buffer<double>(StepBufferSize);
            _intervalBuffer = new double[StepBufferSize];

            Array.Fill(_currentStepIntervalBuffer.Items, settings.Get<TimeSpan>(SettingConstants.StepInterval).Value.TotalMilliseconds);
            Array.Fill(_targetStepIntervalBuffer.Items, settings.Get<TimeSpan>(SettingConstants.StepInterval).Value.TotalMilliseconds);
            Array.Fill(_intervalBuffer, settings.Get<TimeSpan>(SettingConstants.StepInterval).Value.TotalMilliseconds);

            this.Label = "World";
        }

        public void Initialize(ImGuiBatch imGuiBatch)
        {
        }

        public void Draw(GameTime gameTime)
        {
            if(ImGui.Begin("World", ref _open, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse))
            {
                if(ImGui.Button("Save Historical Data"))
                {
                    this.SaveReplayData();
                }

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

                ImPlot.SetNextAxesToFit();
                if (ImPlot.BeginPlot("Step Data", Num.Vector2.Zero))
                {
                    ImPlot.PlotLine("Setting Step", ref _intervalBuffer[0], StepBufferSize);
                    ImPlot.PlotLine("Target Step", ref _targetStepIntervalBuffer.Items[0], StepBufferSize);
                    ImPlot.PlotLine("Current Step", ref _currentStepIntervalBuffer.Items[0], StepBufferSize);

                    ImPlot.EndPlot();
                }
            }
            ImGui.End();
        }

        private void SaveReplayData()
        {
            string jsonString = _json.Serialize(_ticks.History);

            File.WriteAllText("replay.vhr", jsonString);
        }

        public void Update(GameTime gameTime)
        {
            //
        }

        public void Process(in Tick message)
        {
            // Only track ticks with data
            if (!message.Data.Any())
            {
                return;
            }
        }

        public void Process(in Step message)
        {
            _currentStepIntervalBuffer.Add(_steps.CurrentInterval.TotalMilliseconds);
            _targetStepIntervalBuffer.Add(_steps.TargetInterval.TotalMilliseconds);

            _stepMaxValue = Math.Max(_stepMaxValue, _steps.TargetInterval.TotalMilliseconds + 10);
            _stepMinValue = Math.Min(_stepMinValue, _steps.TargetInterval.TotalMilliseconds - 10);
        }
    }
}
