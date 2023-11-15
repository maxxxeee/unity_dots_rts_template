using Unity.Entities;
using Unity.Burst;

//this system disposes of entities wearing the AboutToBeDestroyed_Tag

[BurstCompile]
public partial class DestroyEntitesWithTag_System : SystemBase
{

    private EntityQueryDesc aboutToBeDestroyedQuerry;

    [BurstCompile]
    protected override void OnCreate()
    {
        aboutToBeDestroyedQuerry = new EntityQueryDesc
        {
            All = new ComponentType[]
            { 
                typeof(AboutToBeDestroyed_Tag),
            }
        };
        
        RequireForUpdate( 
            GetEntityQuery( 
                aboutToBeDestroyedQuerry
                ) );
    }
    
    [BurstCompile]
    protected override void OnUpdate()
    {

        Entities
            .WithAll<AboutToBeDestroyed_Tag>()
            .WithBurst()
            .WithNone<unitConstruction_component>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                Entity localEntity,
                EntityCommandBuffer commandBuffer
            ) =>
            {
                commandBuffer.DestroyEntity(localEntity);

            }).ScheduleParallel();

    }
}
