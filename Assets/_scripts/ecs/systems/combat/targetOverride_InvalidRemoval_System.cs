using Unity.Entities;
using Unity.Burst;

//this system specifically looks out for invalid targets inside of a targetOverride
// invalid targets happen if a target is still to be instantiated or it was recently destroyed

//if an invalid target override is detected it will remove the targetOverride component and set the current target to Entity.null

[BurstCompile]
public partial class targetOverride_InvalidRemoval_System : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {

        var navAgentComponentLookup = GetComponentLookup<NavAgent_Component>();
        
        ComponentLookup<AboutToBeDestroyed_Tag> aboutToBeDestroyedTagLookup =
            GetComponentLookup<AboutToBeDestroyed_Tag>(true);
        
        Entities
            .WithBurst()
            .WithAll<combat_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<unitConstruction_component>()
            .WithReadOnly(navAgentComponentLookup)
            .WithReadOnly(aboutToBeDestroyedTagLookup)
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                Entity localEntity,
                EntityCommandBuffer commandBuffer,
                ref currentTargetComponent currentTargetComponent,
                ref hasTargetTag localHasTarget,
                ref targetOverride_component localTargetOverrideComponent
            ) =>
            {
                //remove invalid target
                if ( aboutToBeDestroyedTagLookup.HasComponent(localTargetOverrideComponent.targetOverride) || localTargetOverrideComponent.targetOverride.Index < 0 ||
                     localTargetOverrideComponent.targetOverride == Entity.Null || !navAgentComponentLookup.HasComponent(localTargetOverrideComponent.targetOverride))
                {
                    commandBuffer.RemoveComponent<targetOverride_component>(localEntity);
                    
                    
                    currentTargetComponent.currentTarget = Entity.Null;
                    localHasTarget.Value = false;
                }
                
            }).ScheduleParallel();
    }
}
