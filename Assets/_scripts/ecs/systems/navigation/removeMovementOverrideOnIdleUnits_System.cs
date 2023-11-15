using Unity.Entities;
using Unity.Burst;

// this system checks if a unit was given a movement override by the player
//  if the unit then also has reached its given override destination, the system will remove the movement override tag 

[BurstCompile]
public partial class removeMovementOverrideOnIdleUnits_System : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities
            .WithAll<movementCommandOverride_Tag>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                Entity localEntity,
                EntityCommandBuffer commandBuffer,
                in DynamicBuffer<NavAgent_Buffer> localNavAgentBuffer 
            ) =>
            {
                if (localNavAgentBuffer.IsEmpty)
                {
                    commandBuffer.RemoveComponent<movementCommandOverride_Tag>(localEntity);
                }
            }).Schedule();
    }
}
