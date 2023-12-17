using Guppy;
using Guppy.Attributes;
using Guppy.Commands.Messages;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Simulations;

namespace VoidHuntersRevived.Domain.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal class EntityViewerEngine : BasicEngine, IDebugEngine, IImGuiComponent
    {
        public string? Group => nameof(IEntityService);

        private readonly IGuppy _guppy;
        private readonly ISimulation _simulation;
        private readonly IEntityService _entities;
        private readonly IEntityTypeService _entityTypes;
        private readonly ITeamDescriptorGroupService _teams;
        private readonly IImGui _imgui;
        private bool _entityViewerEnabled;
        private string _filter;

        public EntityViewerEngine(
            IGuppy guppy, 
            ISimulation simulation, 
            IEntityService entities, 
            IEntityTypeService entityTypes,
            ITeamDescriptorGroupService teams,
            IImGui imgui)
        {
            _filter = string.Empty;
            _simulation = simulation;
            _guppy = guppy;
            _entities = entities;
            _entityTypes = entityTypes;
            _teams = teams;
            _imgui = imgui;
        }

        public void RenderDebugInfo(GameTime gameTime)
        {
            var buttonStyle = _entityViewerEnabled ? Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonRed : Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonGreen;

            using (_imgui.Apply(buttonStyle))
            {
                if (_imgui.Button($"{(_entityViewerEnabled ? "Disable" : "Enable")} Entity Viewer"))
                {
                    _entityViewerEnabled = !_entityViewerEnabled;
                }
            }
        }

        public void DrawImGui(GameTime gameTime)
        {
            if(_entityViewerEnabled == false)
            {
                return;
            }

            _imgui.Begin($"Entity Viewer - {_simulation.Type}, {_guppy.Name} {_guppy.Id}", ref _entityViewerEnabled);

            _imgui.InputText("Filter", ref _filter, 255);

            GroupsEnumerable<EntityId, Id<IEntityType>, EntityStatus> groups = _entities.QueryEntities<EntityId, Id<IEntityType>, EntityStatus>();
            foreach (var ((ids, types, statuses, count), groupId) in groups)
            {
                ITeamDescriptorGroup teamDescriptorGroup = _teams.GetByGroupId(groupId);

                if (_imgui.CollapsingHeader($"Group: {groupId.id}, Team: {teamDescriptorGroup.Team.Name}, Descriptor: {teamDescriptorGroup.Descriptor.Name}, Count: {count}"))
                {
                    _imgui.Indent();
                    for (int i = 0; i < count; i++)
                    {
                        if (_imgui.CollapsingHeader($"Vhid: {ids[i].VhId}, Type: {_entityTypes.GetById(types[i]).Key}, IsDespawned: {statuses[i].IsDespawned}"))
                        {
                            _imgui.Indent();
                            this.RenderEntityData(ids[i], teamDescriptorGroup.Descriptor);
                            _imgui.Unindent();
                        }
                    }
                    _imgui.Unindent();
                }
            }

            _imgui.End();
            //throw new NotImplementedException();
        }

        private void RenderEntityData(EntityId entityId, VoidHuntersEntityDescriptor descriptor)
        {
            _imgui.PushID($"#{nameof(RenderEntityData)}#{entityId.VhId}");
            _entities.QueryById<EntityId>(entityId, out GroupIndex groupIndex);
            foreach(Type componentType in descriptor.ComponentManagers.Select(x => x.Type))
            {
                object component = GetComponent(componentType, _entities, ref groupIndex);
                _imgui.ObjectViewer(component, _filter);
            }
            _imgui.PopID();
        }

        private static MethodInfo QueryByGroupIndexMethod = typeof(IEntityService).GetMethod(nameof(IEntityService.QueryByGroupIndex), 1, new[] { typeof(GroupIndex).MakeByRefType() }) ?? throw new Exception();
        private static object GetComponent(Type type, IEntityService entities, ref GroupIndex groupIndex)
        {
            object? component = QueryByGroupIndexMethod.MakeGenericMethod(type).Invoke(entities, new object[] { groupIndex });

            return component ?? new object();
        }
    }
}
