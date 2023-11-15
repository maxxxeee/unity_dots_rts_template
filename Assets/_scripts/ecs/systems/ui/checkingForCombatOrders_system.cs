using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;

//this system checks if the right conditions occured to issue combat commands to selected units
// combat commands get issued if unit(s) have already been selected 
//   and the mouse is currently hovering above a unit from a different team
//   whilst the right mouse button was clicked

// if all of the above is true then the selected units will get a target override component
//  containing the hovered over enemy unit

// if a movement command was issued before the combat command, the movement override will get removed by this system

[UpdateBefore(typeof(checkingForMovementOrders_system))]
[BurstCompile]
public partial class checkingForCombatOrders_system : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {

        var teamTagComponentLookup = GetComponentLookup<teamTag>();
        
        var mouseStatusQuery = new EntityQueryBuilder(Allocator.TempJob)
            .WithAll<mouseStatus_component>()
            .Build(this);

        var mouseStatusQueryEntityArray = mouseStatusQuery.ToEntityArray(Allocator.TempJob);

        var mouseStatusComponentLookup = GetComponentLookup<mouseStatus_component>();
        
        
        var UIStatusQuery = new EntityQueryBuilder(Allocator.TempJob)
            .WithAll<UIUnitHealthBarSync_component>()
            .Build(this);
        
        var UIStatusQueryEntityArray = UIStatusQuery.ToEntityArray(Allocator.TempJob);

        var UIStatusComponentLookup = GetComponentLookup<UIUnitHealthBarSync_component>();

        var combatComponentLookup = GetComponentLookup<combat_component>();

        var movementCommandOverrideLookup = GetComponentLookup<movementCommandOverride_Tag>();
        
        Entities
            .WithAll<thisUnitWasSelectedByUser_tag>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithDisposeOnCompletion(teamTagComponentLookup)
            .WithDisposeOnCompletion(UIStatusQueryEntityArray)
            .WithDisposeOnCompletion(UIStatusComponentLookup)
            .WithDisposeOnCompletion(mouseStatusQueryEntityArray)
            .WithDisposeOnCompletion(mouseStatusComponentLookup)
            .WithDisposeOnCompletion(combatComponentLookup)
            .WithDisposeOnCompletion(movementCommandOverrideLookup)
            .WithBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                Entity localEntity,
                EntityCommandBuffer commandBuffer,
                DynamicBuffer<Child> localChildrenBuffer
            ) =>
            {
                var mouseStatusSingeltonComponent = mouseStatusComponentLookup[mouseStatusQueryEntityArray[0]];
                var UIStatusSingeltonComponent = UIStatusComponentLookup[UIStatusQueryEntityArray[0]];
                
                
                //if right mouse button is pressed
                if (mouseStatusSingeltonComponent.rightMouseButtonDown)
                {
                    //if currently hoverd over entity is not null
                    if (UIStatusSingeltonComponent.detected_Entity != Entity.Null)
                    {
                        //if team tags of the currently selected and hoverd over units are not the same
                        if (teamTagComponentLookup.GetRefRO(UIStatusSingeltonComponent.detected_Entity).ValueRO.Value
                            !=
                            teamTagComponentLookup.GetRefRO(localEntity).ValueRO.Value)
                        {
                            foreach (var childInstance in localChildrenBuffer)
                            {
                                if (combatComponentLookup.HasComponent(childInstance.Value))
                                {
                                    commandBuffer.AddComponent(childInstance.Value, new targetOverride_component
                                    {
                                        targetOverride = UIStatusSingeltonComponent.detected_Entity
                                    });
                                    
                                    commandBuffer.SetComponent(childInstance.Value, new currentTargetComponent
                                    {
                                        currentTarget = UIStatusSingeltonComponent.detected_Entity
                                    });
                                    
                                    commandBuffer.SetComponent(childInstance.Value, new hasTargetTag
                                    {
                                        Value = true
                                    });

                                    if (movementCommandOverrideLookup.HasComponent(localEntity))
                                    {
                                        commandBuffer.RemoveComponent<movementCommandOverride_Tag>(localEntity);
                                    }
                                    
                                    
                                    
                                }
                            }
                        }

                    }

                }
                
            }).Schedule();
    }
}
