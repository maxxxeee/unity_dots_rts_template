using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

//this system counts down the time needed to construct the unit currently being build by a factory
// once the remaning time reaches zero the system will remove the unitConstruction_component
//  and any remaning combat / navigation blocking components

[BurstCompile]
public partial class factoryBuildUnit_system : SystemBase
{
    
    private EntityQueryDesc unitConstructionQuerry;
    
    [BurstCompile]
    protected override void OnCreate()
    {

        unitConstructionQuerry = new EntityQueryDesc
        {
            All = new ComponentType[]
            { 
                typeof(unitConstruction_component),
            }
        };
        
        RequireForUpdate( 
            GetEntityQuery( 
                unitConstructionQuerry
            ) );
    }
    
    [BurstCompile]
    protected override void OnUpdate()
    {

        var getChildrenBuffer = GetBufferLookup<Child>();

        Entities
            .WithBurst()
            .WithAll<unitConstruction_component>()
            .WithReadOnly(getChildrenBuffer)
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                Entity localEntity,
                EntityCommandBuffer commandBuffer,
                ref unitConstruction_component localUnitConstructionComponent
            ) =>
            {
                if (localUnitConstructionComponent.buildTimeLeft < 0.0f)
                {
                    commandBuffer.RemoveComponent<unitConstruction_component>(localEntity);

                    foreach (var childInstance in getChildrenBuffer[localEntity])
                    {
                        if (SystemAPI.HasComponent<combatDisabled_tag>(childInstance.Value))
                        {
                            commandBuffer.RemoveComponent<combatDisabled_tag>(childInstance.Value);
                        }
                    }
                    
                }
                else
                {
                    localUnitConstructionComponent.buildTimeLeft -= SystemAPI.Time.DeltaTime;
                }
                
            }).Schedule();
    }
}
