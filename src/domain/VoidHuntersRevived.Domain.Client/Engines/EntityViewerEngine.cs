using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Common.Enums;
using Guppy.Common.Services;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui;
using Guppy.Game.ImGui.Services;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using Svelto.ECS.Internal;
using System.Reflection;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

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
        private readonly IImGuiObjectExplorerService _objectExplorer;
        private readonly IObjectTextFilterService _objectFilter;
        private readonly IImGui _imgui;
        private bool _entityViewerEnabled;
        private string _filter;

        private Dictionary<uint, TextFilterResult> _filterResults;
        private Vector4 _redForeground = Color.Red.ToVector4();
        private Vector4 _greenForeground = Color.LightGreen.ToVector4();
        private Vector4 _redBackground = Color.DarkRed.ToVector4();
        private Vector4 _greenBackground = Color.DarkGreen.ToVector4();

        public EntityViewerEngine(
            IGuppy guppy,
            ISimulation simulation,
            IEntityService entities,
            IEntityTypeService entityTypes,
            ITeamDescriptorGroupService teams,
            IImGuiObjectExplorerService objectExplorer,
            IObjectTextFilterService objectFilter,
            IImGui imgui)
        {
            _simulation = simulation;
            _guppy = guppy;
            _entities = entities;
            _entityTypes = entityTypes;
            _teams = teams;
            _objectExplorer = objectExplorer;
            _imgui = imgui;
            _objectFilter = objectFilter;
            _filter = string.Empty;
            _filterResults = new Dictionary<uint, TextFilterResult>();
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
            if (_entityViewerEnabled == false)
            {
                return;
            }

            _imgui.Begin($"Entity Viewer - {_simulation.Type}, {_guppy.Name} {_guppy.Id}", ref _entityViewerEnabled);

            _imgui.InputText("Filter", ref _filter, 255);

            GroupsEnumerable<EntityId, Id<IEntityType>, EntityStatus> groups = _entities.QueryEntities<EntityId, Id<IEntityType>, EntityStatus>();
            foreach (var ((ids, types, statuses, nativeIds, count), groupId) in groups)
            {
                ITeamDescriptorGroup teamDescriptorGroup = _teams.GetByGroupId(groupId);

                this.RenderTeamDescriptorGroup(groupId, teamDescriptorGroup, ids, types, statuses, nativeIds, count);
            }

            _imgui.End();
            //throw new NotImplementedException();
        }

        private void RenderTeamDescriptorGroup(ExclusiveGroupStruct groupId, ITeamDescriptorGroup teamDescriptorGroup, Svelto.DataStructures.NB<EntityId> ids, Svelto.DataStructures.NB<Id<IEntityType>> types, Svelto.DataStructures.NB<EntityStatus> statuses, NativeEntityIDs nativeIds, int count)
        {
            using (_imgui.ApplyID($"{nameof(EntityViewerEngine)}_{nameof(ExclusiveGroupStruct)}_{groupId.id}"))
            {
                uint id = _imgui.GetID(nameof(TextFilterResult));
                ref TextFilterResult result = ref this.GetFilterResult(id);
                string label = $"Group: {groupId.id}, Team: {teamDescriptorGroup.Team.Name}, Descriptor: {teamDescriptorGroup.Descriptor.Name}, Count: {count}";
                Vector4? color = result switch
                {
                    TextFilterResult.NotMatched => _redBackground,
                    TextFilterResult.Matched => _greenBackground,
                    _ => null
                };

                result = this.BasicFilter(label);
                if (_imgui.CollapsingHeader(label, color))
                {
                    _imgui.Indent();
                    for (int i = 0; i < count; i++)
                    {
                        result = result.Max(this.RenderEntityData(ids[i], teamDescriptorGroup.Descriptor, _entityTypes.GetById(types[i]), statuses[i], nativeIds[i]));
                    }
                    _imgui.Unindent();
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        result = result.Max(this.FilterEntityData(ids[i], teamDescriptorGroup.Descriptor, _entityTypes.GetById(types[i])));
                    }
                }
            }

        }

        private TextFilterResult RenderEntityData(EntityId entityId, VoidHuntersEntityDescriptor descriptor, IEntityType type, EntityStatus status, uint nativeId)
        {
            using (_imgui.ApplyID($"{nameof(EntityId)}_{entityId.VhId.Value}"))
            {
                uint id = _imgui.GetID(nameof(TextFilterResult));
                ref TextFilterResult result = ref this.GetFilterResult(id);
                string label = $"Vhid: {entityId.VhId}, Type: {type.Key}, NativeId: {nativeId}";
                Vector4? color = result switch
                {
                    TextFilterResult.NotMatched => _redBackground,
                    TextFilterResult.Matched => _greenBackground,
                    _ => null
                };

                result = this.BasicFilter(label);

                if (_imgui.CollapsingHeader(label, color))
                {
                    _entities.QueryById<EntityId>(entityId, out GroupIndex groupIndex);

                    _imgui.Indent();
                    using (_imgui.ApplyID(nameof(EntityId)))
                    {
                        result = result.Max(_objectExplorer.DrawObjectExplorer(entityId, _filter));
                    }

                    foreach (Type componentType in descriptor.ComponentManagers.Select(x => x.Type))
                    {
                        using (_imgui.ApplyID(componentType.AssemblyQualifiedName ?? string.Empty))
                        {
                            object component = GetComponent(componentType, _entities, ref groupIndex);
                            result = result.Max(_objectExplorer.DrawObjectExplorer(component, _filter));
                        }
                    }
                    _imgui.Unindent();
                }
                else
                {
                    result = result.Max(this.FilterEntityData(entityId, descriptor, type));
                }

                return result;
            }
        }

        private static MethodInfo QueryByGroupIndexMethod = typeof(IEntityService).GetMethod(nameof(IEntityService.QueryByGroupIndex), 1, new[] { typeof(GroupIndex).MakeByRefType() }) ?? throw new Exception();
        private static object GetComponent(Type type, IEntityService entities, ref GroupIndex groupIndex)
        {
            object? component = QueryByGroupIndexMethod.MakeGenericMethod(type).Invoke(entities, new object[] { groupIndex });

            return component ?? new object();
        }

        private ref TextFilterResult GetFilterResult(uint id)
        {
            ref TextFilterResult result = ref CollectionsMarshal.GetValueRefOrAddDefault(_filterResults, id, out _);

            return ref result;
        }

        private TextFilterResult BasicFilter(string input)
        {
            if (_filter.IsNullOrEmpty())
            {
                return TextFilterResult.None;
            }

            if (input.Contains(_filter))
            {
                return TextFilterResult.Matched;
            }

            return TextFilterResult.NotMatched;
        }

        private TextFilterResult FilterEntityData(EntityId entityId, VoidHuntersEntityDescriptor descriptor, IEntityType type)
        {
            _entities.QueryById<EntityId>(entityId, out GroupIndex groupIndex);
            TextFilterResult result = this.BasicFilter($"{entityId.VhId}{descriptor.Name}{type.Key}");

            foreach (Type componentType in descriptor.ComponentManagers.Select(x => x.Type))
            {
                using (_imgui.ApplyID(componentType.AssemblyQualifiedName ?? string.Empty))
                {
                    object component = GetComponent(componentType, _entities, ref groupIndex);
                    result = result.Max(_objectFilter.Filter(component, _filter));
                }
            }

            return result;
        }
    }
}
