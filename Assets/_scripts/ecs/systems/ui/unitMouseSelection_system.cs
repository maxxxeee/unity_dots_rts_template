using Unity.Entities;
using Unity.Burst;
using Unity.Collections;


// this system gives units the selection component under the correct circumstances

// this system skips execution if
//      mouse drag box selection is active

// if the shift button was not pressed at the time of the left mouse button being pressed
// every already selected unit will lose that selection

// the newly selected unit will receive the "thisUnitWasSelectedByUser_tag"

[BurstCompile]
public partial class unitMouseSelection_system : SystemBase
{
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        var mouseStatusQuery = new EntityQueryBuilder(Allocator.TempJob)
            .WithAll<mouseStatus_component>()
            .Build(this);

        var mouseStatusQueryEntityArray = mouseStatusQuery.ToEntityArray(Allocator.TempJob);

        var mouseStatusComponentLookup = GetComponentLookup<mouseStatus_component>();
        
        
        var thisUnitWasSelectedByUserQuerry = new EntityQueryBuilder(Allocator.TempJob)
            .WithAll<thisUnitWasSelectedByUser_tag>()
            .Build(this);

        var thisUnitWasSelectedByUserQuerryArray = thisUnitWasSelectedByUserQuerry.ToEntityArray(Allocator.TempJob);

        var shiftButtonStatusComponentLookup = GetComponentLookup<unitSelection_shiftButtonStatus_component>();
        
        Entities
            .WithAll<UIMouseHoveringOverThisEntity_tag>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithBurst()
            .WithDisposeOnCompletion(mouseStatusQueryEntityArray)
            .WithDisposeOnCompletion(mouseStatusComponentLookup)
            .WithDisposeOnCompletion(thisUnitWasSelectedByUserQuerryArray)
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                Entity localEntity,
                EntityCommandBuffer commandBuffer
            ) =>
            {
                var mouseStatusSingeltonComponent = mouseStatusComponentLookup[mouseStatusQueryEntityArray[0]];

                var shiftButtonStatusComponent = shiftButtonStatusComponentLookup[mouseStatusQueryEntityArray[0]];

                if (mouseStatusSingeltonComponent.mouseDragBoxSelectionActive)
                {
                    return;
                }
                
                if (mouseStatusSingeltonComponent.leftMouseButtonDown)
                {
                   
                    //only remove previously selected units if shift is not pressed 
                    if (!shiftButtonStatusComponent.Value)
                    {
                        //remove tag from previous units
                        foreach (var selectedByUserTagInstance in thisUnitWasSelectedByUserQuerryArray)
                        {
                            commandBuffer.RemoveComponent<thisUnitWasSelectedByUser_tag>(selectedByUserTagInstance);
                        }
                    }
                    
                    //add tag to new unit
                    commandBuffer.AddComponent<thisUnitWasSelectedByUser_tag>(localEntity);
                }

            }).Schedule();
    }
}
