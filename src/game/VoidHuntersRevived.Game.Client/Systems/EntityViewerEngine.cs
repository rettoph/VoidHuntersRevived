namespace VoidHuntersRevived.Game.Client.Systems
{
    //[AutoLoad]
    //[Sequence<DrawSequence>(DrawSequence.PreDraw)]
    //internal class EntityViewerEngine : StrategyEngine, IDebugEngine, IImGuiComponent
    //{
    //    public string? Group => nameof(IEntityService);
    //
    //    private readonly IScene _scene;
    //    private readonly IStrategy _strategy;
    //    private readonly IEntityQueryService _entityQueryService;
    //    private readonly IEntityTemplateProviderService _entityTemplateService;
    //    private readonly IImGuiObjectExplorerService _objectExplorer;
    //    private readonly IObjectTextFilterService _objectFilter;
    //    private readonly IImGui _imgui;
    //    private bool _entityViewerEnabled;
    //    private string _filter;
    //
    //    private Dictionary<uint, TextFilterResult> _filterResults;
    //    private Vector4 _redForeground = Color.Red.ToVector4();
    //    private Vector4 _greenForeground = Color.LightGreen.ToVector4();
    //    private Vector4 _redBackground = Color.DarkRed.ToVector4();
    //    private Vector4 _greenBackground = Color.DarkGreen.ToVector4();
    //
    //    public EntityViewerEngine(
    //        IScene scene,
    //        IStrategy strategy,
    //        IEntityQueryService entityQueryService,
    //        IEntityTemplateProviderService entityTemplateService,
    //        IImGuiObjectExplorerService objectExplorer,
    //        IObjectTextFilterService objectFilter,
    //        IImGui imgui)
    //    {
    //        _strategy = strategy;
    //        _scene = scene;
    //        _entityQueryService = entityQueryService;
    //        _entityTemplateService = entityTemplateService;
    //        _objectExplorer = objectExplorer;
    //        _imgui = imgui;
    //        _objectFilter = objectFilter;
    //        _filter = string.Empty;
    //        _filterResults = new Dictionary<uint, TextFilterResult>();
    //    }
    //
    //    public void RenderDebugInfo(GameTime gameTime)
    //    {
    //        var buttonStyle = _entityViewerEnabled ? Guppy.Game.MonoGame.Common.Assets.ImGuiStyles.ButtonRed : Guppy.Game.MonoGame.Common.Assets.ImGuiStyles.ButtonGreen;
    //
    //        using (_imgui.Apply(buttonStyle))
    //        {
    //            if (_imgui.Button($"{(_entityViewerEnabled ? "Disable" : "Enable")} Entity Viewer"))
    //            {
    //                _entityViewerEnabled = !_entityViewerEnabled;
    //            }
    //        }
    //    }
    //
    //    public void DrawImGui(GameTime gameTime)
    //    {
    //        if (_entityViewerEnabled == false)
    //        {
    //            return;
    //        }
    //
    //        _imgui.Begin($"Entity Viewer - {_strategy.Type}, {_scene.Name} {_scene.Id}", ref _entityViewerEnabled);
    //
    //        _imgui.InputText("Filter", ref _filter, 255);
    //
    //        // foreach (VoidHuntersEntityDescriptor descriptor in _entityDescriptorService.GetAll())
    //        // {
    //        //     var (instanceEntities, ids, statuses, nativeIds, count) = _entityQueryService.QueryEntities<InstanceEntity, EntityId, EntityStatus>(descriptor.InstanceGroup);
    //        //     this.RenderTeamDescriptorGroup(descriptor, instanceEntities, ids, statuses, nativeIds, count);
    //        // }
    //
    //        _imgui.End();
    //        //throw new NotImplementedException();
    //    }
    //
    //    private void RenderTeamDescriptorGroup(VoidHuntersEntityDescriptor descriptor, Svelto.DataStructures.NB<InstanceEntity> instanceEntities, Svelto.DataStructures.NB<EntityId> ids, Svelto.DataStructures.NB<EntityStatus> statuses, NativeEntityIDs nativeIds, int count)
    //    {
    //        using (_imgui.ApplyID($"{nameof(EntityViewerEngine)}_{nameof(ExclusiveGroupStruct)}_{descriptor.InstanceGroup.id}"))
    //        {
    //            uint id = _imgui.GetID(nameof(TextFilterResult));
    //            ref TextFilterResult result = ref this.GetFilterResult(id);
    //            string label = $"Group: {descriptor.InstanceGroup.id}, Descriptor: {descriptor.Name}, Count: {count}";
    //            Vector4? color = result switch
    //            {
    //                TextFilterResult.NotMatched => _redBackground,
    //                TextFilterResult.Matched => _greenBackground,
    //                _ => null
    //            };
    //
    //            result = this.BasicFilter(label);
    //            if (_imgui.CollapsingHeader(label, color))
    //            {
    //                _imgui.Indent();
    //                for (int i = 0; i < count; i++)
    //                {
    //                    result = result.Max(this.RenderEntityData(ids[i], descriptor, instanceEntities[i].Type, statuses[i], nativeIds[i]));
    //                }
    //                _imgui.Unindent();
    //            }
    //            else
    //            {
    //                for (int i = 0; i < count; i++)
    //                {
    //                    result = result.Max(this.FilterEntityData(ids[i], descriptor, instanceEntities[i].Type));
    //                }
    //            }
    //        }
    //
    //    }
    //
    //    private TextFilterResult RenderEntityData(EntityId entityId, VoidHuntersEntityDescriptor descriptor, EntityTemplate type, EntityStatus status, uint nativeId)
    //    {
    //        using (_imgui.ApplyID($"{nameof(EntityId)}_{entityId.VhId.Value}"))
    //        {
    //            uint id = _imgui.GetID(nameof(TextFilterResult));
    //            ref TextFilterResult result = ref this.GetFilterResult(id);
    //            string label = $"Vhid: {entityId.VhId}, Type: {type.Key}, NativeId: {nativeId}";
    //            Vector4? color = result switch
    //            {
    //                TextFilterResult.NotMatched => _redBackground,
    //                TextFilterResult.Matched => _greenBackground,
    //                _ => null
    //            };
    //
    //            result = this.BasicFilter(label);
    //
    //            if (_imgui.CollapsingHeader(label, color))
    //            {
    //                _entityQueryService.QueryById<EntityId>(entityId, out GroupIndex groupIndex);
    //
    //                _imgui.Indent();
    //                using (_imgui.ApplyID(nameof(EntityId)))
    //                {
    //                    result = result.Max(_objectExplorer.DrawObjectExplorer(entityId, _filter));
    //                }
    //
    //                foreach (Type componentType in descriptor.Instance.componentsToBuild.Select(x => x.GetEntityComponentType()).Where(x => x.IsAssignableTo<IEntityComponent>()))
    //                {
    //                    using (_imgui.ApplyID(componentType.AssemblyQualifiedName ?? string.Empty))
    //                    {
    //                        object component = GetComponent(componentType, _entityQueryService, ref groupIndex);
    //                        result = result.Max(_objectExplorer.DrawObjectExplorer(component, _filter));
    //                    }
    //                }
    //                _imgui.Unindent();
    //            }
    //            else
    //            {
    //                result = result.Max(this.FilterEntityData(entityId, descriptor, type));
    //            }
    //
    //            return result;
    //        }
    //    }
    //
    //    private static MethodInfo QueryByGroupIndexMethod = typeof(IEntityQueryService).GetMethod(nameof(IEntityQueryService.QueryByGroupIndex), 1, new[] { typeof(GroupIndex).MakeByRefType() }) ?? throw new Exception();
    //    private static object GetComponent(Type type, IEntityQueryService entityQueryService, ref GroupIndex groupIndex)
    //    {
    //        object? component = QueryByGroupIndexMethod.MakeGenericMethod(type).Invoke(entityQueryService, new object[] { groupIndex });
    //
    //        return component ?? new object();
    //    }
    //
    //    private ref TextFilterResult GetFilterResult(uint id)
    //    {
    //        ref TextFilterResult result = ref CollectionsMarshal.GetValueRefOrAddDefault(_filterResults, id, out _);
    //
    //        return ref result;
    //    }
    //
    //    private TextFilterResult BasicFilter(string input)
    //    {
    //        if (_filter.IsNullOrEmpty())
    //        {
    //            return TextFilterResult.None;
    //        }
    //
    //        if (input.Contains(_filter))
    //        {
    //            return TextFilterResult.Matched;
    //        }
    //
    //        return TextFilterResult.NotMatched;
    //    }
    //
    //    private TextFilterResult FilterEntityData(EntityId entityId, VoidHuntersEntityDescriptor descriptor, EntityTemplate type)
    //    {
    //        _entityQueryService.QueryById<EntityId>(entityId, out GroupIndex groupIndex);
    //        TextFilterResult result = this.BasicFilter($"{entityId.VhId}{descriptor.Name}{type.Key}");
    //
    //        foreach (Type componentType in descriptor.Instance.componentsToBuild.Select(x => x.GetEntityComponentType()).Where(x => x.IsAssignableTo<IEntityComponent>()))
    //        {
    //            using (_imgui.ApplyID(componentType.AssemblyQualifiedName ?? string.Empty))
    //            {
    //                object component = GetComponent(componentType, _entityQueryService, ref groupIndex);
    //                result = result.Max(_objectFilter.Filter(component, _filter));
    //            }
    //        }
    //
    //        return result;
    //    }
    //}
}