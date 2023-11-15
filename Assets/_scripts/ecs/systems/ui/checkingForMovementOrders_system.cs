using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;

// this system will issue movement commands to any currently selected units
// if the mouse is currently hovering over terrain and the right mouse button was pressed
//  then a movementOverride_tag will be added and the navAgent instructed to move to the new location

[BurstCompile]
public partial class checkingForMovementOrders_system : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
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
        
        
        Entities
            .WithAll<thisUnitWasSelectedByUser_tag>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithDisposeOnCompletion(UIStatusQueryEntityArray)
            .WithDisposeOnCompletion(mouseStatusQueryEntityArray)
            .WithDisposeOnCompletion(mouseStatusComponentLookup)
            .WithDisposeOnCompletion(UIStatusComponentLookup)
            .WithBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                Entity localEntity,
                EntityCommandBuffer commandBuffer,
                ref DynamicBuffer<NavAgent_Buffer> localNavAgentBuffer,
                ref NavAgent_Component localNavAgentComponent,
                ref NavAgent_ToBeRoutedTag localNavAgentToBeRoutedTag,
                ref UnitComponentData localUnitComponentData,
                in LocalToWorld localToWorld
            ) =>
            {
                var mouseStatusSingeltonComponent = mouseStatusComponentLookup[mouseStatusQueryEntityArray[0]];
                var UIStatusSingeltonComponent = UIStatusComponentLookup[UIStatusQueryEntityArray[0]];
                
                
                //if right mouse button is pressed
                if (mouseStatusSingeltonComponent.rightMouseButtonDown)
                {
                    //if currently hovered over entity is not null
                    if (UIStatusSingeltonComponent.detected_Entity == Entity.Null)
                    {

                        commandBuffer.AddComponent<movementCommandOverride_Tag>(localEntity);
                        
                        localNavAgentBuffer.Clear();
                       
                        
                        localNavAgentComponent.fromLocation = localToWorld.Position;

                        var tempMouseWorldPosition = mouseStatusSingeltonComponent.mouseTerrainPosition;

                        tempMouseWorldPosition.y = 0.0f;
                        
                        localNavAgentComponent.toLocation = tempMouseWorldPosition;
                        localNavAgentComponent.routed = false;

                        localNavAgentToBeRoutedTag.Value = true;
                        
                        
                        localUnitComponentData.reached = false;
                        localUnitComponentData.currentBufferIndex = 0;
                        
                    }
                }
                
            }).Schedule();
    }
}
