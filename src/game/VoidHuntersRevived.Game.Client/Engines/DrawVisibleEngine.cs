namespace VoidHuntersRevived.Game.Client.Engines
{
    // [AutoLoad]
    // [StrategyFilter(StrategyTypeEnum.Predictive)]
    // [Sequence<DrawSequence>(DrawSequence.Draw)]
    // internal sealed class DrawVisibleEngine : StrategyEngine, IDrawVisibleEngine
    // {
    //     private readonly IEntityQueryService _entityQueryService;
    //     private readonly ILogger _logger;
    //     private readonly Camera2D _camera;
    // 
    //     public string name { get; } = nameof(DrawVisibleEngine);
    // 
    //     public DrawVisibleEngine(
    //         ILogger logger,
    //         IEntityQueryService entityQueryService,
    //         Camera2D camera)
    //     {
    //         _entityQueryService = entityQueryService;
    //         _logger = logger;
    //         _camera = camera;
    //     }
    // 
    //     public void Step(in IVisibleInstanceVertexService param)
    //     {
    //         foreach (var ((typeEntities, hasManyInstances, _, typeCount), _) in _entityQueryService.QueryEntities<TypeEntity, HasMany<InstanceEntity, TypeEntity>, Visible>())
    //         {
    //             for (int i = 0; i < typeCount; i++)
    //             {
    //                 IKey<IEntityType> entityTypeKey = typeEntities[i].Type.Key;
    //                 var type = typeEntities[i].Type;
    // 
    //                 ref HasMany<InstanceEntity, TypeEntity> hasManyIntances = ref hasManyInstances[i];
    // 
    //                 InstanceVertexProvider<VertexInstanceVisible> vertexBufferManager = param.GetInstanceVertexProviderByKey(entityTypeKey);
    // 
    //                 foreach (var (indices, group) in hasManyIntances.Items)
    //                 {
    //                     var (statuses, nodes, colorSchemes, instanceCount) = _entityQueryService.QueryEntities<EntityStatus, Node, ColorScheme>(group);
    //                     vertexBufferManager.EnsureFit(instanceCount);
    // 
    //                     for (int j = 0; j < indices.count; j++)
    //                     {
    //                         uint index = indices[j];
    //                         if (statuses[index].IsDespawned)
    //                         { // Dont render pieces that have been despawned
    //                             continue;
    //                         }
    // 
    //                         ref Node node = ref nodes[index];
    //                         ref ColorScheme colorScheme = ref colorSchemes[index];
    // 
    //                         ref VertexInstanceVisible instanceVertex = ref vertexBufferManager.GetNextVertexUnsafe();
    // 
    //                         instanceVertex.LocalTransformation = node.XnaTransformation;
    //                         instanceVertex.PrimaryColor = colorScheme.Primary.Value.PackedValue;
    //                         instanceVertex.SecondaryColor = colorScheme.Secondary.Value.PackedValue;
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }
}
