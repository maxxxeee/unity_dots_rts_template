using Unity.Entities;
using Unity.Burst;
using Unity.Collections;


// this system removes the selected unit component from all units currently possessing that component
//  under the correct circumstances

// if no unit is currently selected 
//  or drag box selection is active
//   or the shift button is being pressed
// the system will skip execution

// if none of the above is true and the left mouse button was pressed
//  whilst the mouse is not hovering over any other unit
//      all currently selected units will get deselected

[UpdateBefore(typeof(unitMouseSelection_system))]
[BurstCompile]
public partial class unitMouseDeselection_system : SystemBase
{
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        var thisUnitWasSelectedByUserQuerry = new EntityQueryBuilder(Allocator.TempJob)
            .WithAll<thisUnitWasSelectedByUser_tag>()
            .Build(this);

        var thisUnitWasSelectedByUserQuerryArray = thisUnitWasSelectedByUserQuerry.ToEntityArray(Allocator.TempJob);
        

        Entities
            .WithAll<UIUnitHealthBarSync_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithBurst()
            .WithDisposeOnCompletion(thisUnitWasSelectedByUserQuerryArray)
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                EntityCommandBuffer commandBuffer,
                in UIUnitHealthBarSync_component localUIUnitHealthBarSyncComponent,
                in mouseStatus_component localMouseStatusComponent,
                in unitSelection_shiftButtonStatus_component localShiftButtonStatus
            ) =>
            {

                if (localMouseStatusComponent.mouseDragBoxSelectionActive || thisUnitWasSelectedByUserQuerryArray.Length < 1 || localShiftButtonStatus.Value)
                {
                    return;
                }
                
                if (localMouseStatusComponent.leftMouseButtonDown && localUIUnitHealthBarSyncComponent.detected_Entity == Entity.Null)
                {
                    foreach (var unitSelecetedByUserInstance in thisUnitWasSelectedByUserQuerryArray)
                    {
                        commandBuffer.RemoveComponent<thisUnitWasSelectedByUser_tag>(unitSelecetedByUserInstance);
                    }
                }
                
            }).Schedule();
    }
}
