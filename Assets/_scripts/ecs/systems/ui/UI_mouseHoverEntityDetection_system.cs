using Unity.Entities;
using Unity.Physics;
using RaycastHit = Unity.Physics.RaycastHit;
using Unity.Burst;

//this system detects if the mouse is currently hovering over any unit
// it uses a physics raycast from the players camera 
// if a unit gets hit by the raycast it will receive a "UIMouseHoveringOverThisEntity_tag"
// if the mouse moves away from this unit it will loose the being hovered over tag

[BurstCompile]
public partial class mouseHoverEntityDetection_system : SystemBase
{
    private EntityQueryDesc UIUnitHealthBarSyncQuerry;
    
    protected override void OnCreate()
    {

        UIUnitHealthBarSyncQuerry = new EntityQueryDesc
        {
            All = new ComponentType[]
            { 
                typeof(UIUnitHealthBarSync_component),
            }
        };
        
        RequireForUpdate( 
            GetEntityQuery( 
                UIUnitHealthBarSyncQuerry
            ) );
    }
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
        
        Entities
            .WithAll<UIMouse_Position_Sync_component>()
            .WithReadOnly(collisionWorld)
            .WithBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                EntityCommandBuffer commandBuffer,
                ref UIUnitHealthBarSync_component localUIHealthbar_sync_component,
                in UIMouse_Position_Sync_component localUIMouse_position_sync_component
                
            ) =>
            {
                CollisionFilter localFilter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = 0
                };
                
                
                RaycastHit localRayCastHit;
                RaycastInput localRayCastInput = new RaycastInput
                {
                    End = localUIMouse_position_sync_component.currentMouseWorldPos + localUIMouse_position_sync_component.ScreenMouseRayDirection * 2000.0f,
                    Filter = localFilter,
                    Start = localUIMouse_position_sync_component.mainCameraPosition
                };

                
                collisionWorld.CastRay(localRayCastInput, out localRayCastHit);
                

                if (localRayCastHit.Entity.Index == 0 || localRayCastHit.Entity == Entity.Null)
                {
                    if (localUIHealthbar_sync_component.detected_Entity != Entity.Null)
                    {
                        commandBuffer.RemoveComponent<UIMouseHoveringOverThisEntity_tag>(localUIHealthbar_sync_component.detected_Entity);
                        localUIHealthbar_sync_component.detected_Entity = Entity.Null;
                    }
                    return;
                }
              
                Entity entityFromRayCastResult = localRayCastHit.Entity;
                    
                //remove tag if the detected entity is not equal to the currently tagged entity
                if (!localUIHealthbar_sync_component.detected_Entity.Equals(entityFromRayCastResult) &&!localUIHealthbar_sync_component.detected_Entity.Equals(Entity.Null))
                {
                    commandBuffer.RemoveComponent<UIMouseHoveringOverThisEntity_tag>(localUIHealthbar_sync_component.detected_Entity);
                    localUIHealthbar_sync_component.detected_Entity = Entity.Null;
                }
                    
                if (SystemAPI.HasComponent<health_component>(entityFromRayCastResult) && !SystemAPI.HasComponent<UIMouseHoveringOverThisEntity_tag>(entityFromRayCastResult))
                {
                   commandBuffer.AddComponent<UIMouseHoveringOverThisEntity_tag>(entityFromRayCastResult);
                   localUIHealthbar_sync_component.detected_Entity = entityFromRayCastResult;
                }
            }).Schedule();
    }
}
