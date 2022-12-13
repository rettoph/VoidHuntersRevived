using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.MonoGame.UI;
using Guppy.MonoGame.UI.Debuggers;
using Guppy.Resources.Providers;
using Guppy.Resources.Serialization.Json;
using ImGuiNET;
using ImPlotNET;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Providers;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Client.Library.Debuggers
{
    [AutoSubscribe]
    [GuppyFilter(typeof(ClientGameGuppy))]
    internal sealed class WorldDebugger : SimpleDebugger, IImGuiDebugger, ISubscriber<Tick>, ISubscriber<Step>
    {
        private const int StepBufferSize = 256;

        private double _stepDifference;
        private double _stepDifferenceAverage;
        private double _stepDifferenceSum;

        private readonly double[] _intervalBuffer;
        private readonly Buffer<double> _currentStepIntervalBuffer;
        private readonly Buffer<double> _targetStepIntervalBuffer;
        private readonly Buffer<double> _stepDifferenceBuffer;
        private readonly Buffer<double> _stepDifferenceAverageBuffer;
        private readonly StepRemoteProvider _steps;
        private readonly GameState _state;
        private readonly ITickService _ticks;
        private readonly IJsonSerializer _json;
        private readonly Lazy<IBus> _bus;

        public string ButtonLabel { get; }

        public WorldDebugger(GameState state, StepRemoteProvider steps, Lazy<IBus> bus, ITickService ticks, IJsonSerializer json, ISettingProvider settings)
        {
            _state = state;
            _steps = steps;
            _ticks = ticks;
            _json = json;
            _bus = bus;
            _currentStepIntervalBuffer = new Buffer<double>(StepBufferSize);
            _targetStepIntervalBuffer = new Buffer<double>(StepBufferSize);
            _stepDifferenceBuffer = new Buffer<double>(StepBufferSize);
            _stepDifferenceAverageBuffer = new Buffer<double>(StepBufferSize);
            _intervalBuffer = new double[StepBufferSize];

            Array.Fill(_currentStepIntervalBuffer.Items, settings.Get<TimeSpan>(SettingConstants.StepInterval).Value.TotalMilliseconds);
            Array.Fill(_targetStepIntervalBuffer.Items, settings.Get<TimeSpan>(SettingConstants.StepInterval).Value.TotalMilliseconds);
            Array.Fill(_intervalBuffer, settings.Get<TimeSpan>(SettingConstants.StepInterval).Value.TotalMilliseconds);

            this.ButtonLabel = "World";
            this.IsEnabled = false;
            this.Visible = false;
        }

        public void Initialize(ImGuiBatch imGuiBatch)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            if(ImGui.Begin("World", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse))
            {
                if(ImGui.Button("Save Historical Data"))
                {
                    this.SaveReplayData();
                }

                if (ImGui.Button("Toggle Simulated Lag"))
                {
                    this._bus.Value.Publish(new ToggleSimulatedLag());
                }

                ImGui.BeginTable("data", 2);

                ImGui.TableNextColumn();
                ImGui.Text("Last Tick");

                ImGui.TableNextColumn();
                ImGui.Text(_state.LastTickId.ToString("#,###,##0"));

                ImGui.TableNextColumn();
                ImGui.Text("Available Tick");

                ImGui.TableNextColumn();
                ImGui.Text(_ticks.AvailableId.ToString("#,###,##0"));

                ImGui.TableNextColumn();
                ImGui.Text("Last Step");

                ImGui.TableNextColumn();
                ImGui.Text(_state.LastStep.ToString("#,###,##0"));

                ImGui.TableNextColumn();
                ImGui.Text("Target Step");

                ImGui.TableNextColumn();
                ImGui.Text(_steps.TargetStep.ToString("#,###,##0"));

                ImGui.TableNextColumn();
                ImGui.Text("Step Difference");

                ImGui.TableNextColumn();
                ImGui.Text(_stepDifference.ToString(" 0;-#; 0"));

                ImGui.TableNextColumn();
                ImGui.Text("Step Difference Average");

                ImGui.TableNextColumn();
                ImGui.Text(_stepDifferenceAverage.ToString(" 0;-#; 0"));


                ImGui.EndTable();

                ImPlot.SetNextAxesToFit();
                if (ImPlot.BeginPlot("Step Difference (Target - Current)", Num.Vector2.Zero))
                {
                    ImPlot.PlotLine("Difference", ref _stepDifferenceBuffer.Items[0], StepBufferSize);
                    ImPlot.PlotLine("Average", ref _stepDifferenceAverageBuffer.Items[0], StepBufferSize);

                    ImPlot.EndPlot();
                }

                ImPlot.SetNextAxesToFit();
                if (ImPlot.BeginPlot("Step Interval (Milliseconds)", Num.Vector2.Zero))
                {
                    ImPlot.PlotLine("Target", ref _targetStepIntervalBuffer.Items[0], StepBufferSize);
                    ImPlot.PlotLine("Current", ref _currentStepIntervalBuffer.Items[0], StepBufferSize);

                    ImPlot.EndPlot();
                }
            }
            ImGui.End();
        }

        private void SaveReplayData()
        {
            string jsonString = _json.Serialize(_state.History);

            File.WriteAllText("replay.vhr", jsonString);
        }

        public override void Update(GameTime gameTime)
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
            // Update interval buffers
            _currentStepIntervalBuffer.Add(_steps.CurrentInterval.TotalMilliseconds);
            _targetStepIntervalBuffer.Add(_steps.TargetInterval.TotalMilliseconds);

            // Update buffers
            _stepDifference = _steps.TargetStep - _state.LastStep;

            _stepDifferenceBuffer.Add(_stepDifference, out var oldStepDifference);

            _stepDifferenceSum += _stepDifference - oldStepDifference;
            _stepDifferenceAverage = _stepDifferenceSum / _stepDifferenceAverageBuffer.Length;

            _stepDifferenceAverageBuffer.Add(_stepDifferenceAverage);
        }

        public void Toggle()
        {
            this.Visible = !this.Visible;
        }
    }
}
