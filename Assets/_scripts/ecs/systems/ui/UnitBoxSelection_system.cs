using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Burst;

// this system executes a drag box selection by using a physics overlap box

// the system accesses the mouse status data singelton component

// with this information it will execute a boxoverlap once the left mouse button has been released 
//  and a given minimum distance has been traveled between press and release

// any entity inside of the boxoverlap with a navAgent component will also receive a "thisUnitWasSelectedByUser_tag"

[UpdateAfter(typeof(UI_sync_from_gameObject_system))]
[BurstCompile]
public partial class UnitBoxSelection_System : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        CollisionFilter filter = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = ~0u,
            GroupIndex = 0
        };
        
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
        
       
       var thisUnitWasSelectedByUserQuerry = new EntityQueryBuilder(Allocator.TempJob)
           .WithAll<thisUnitWasSelectedByUser_tag>()
           .Build(this);

       var thisUnitWasSelectedByUserQuerryArray = thisUnitWasSelectedByUserQuerry.ToEntityArray(Allocator.TempJob);

       var navAgentComponentLookup = GetComponentLookup<NavAgent_Component>();

       Entities
           .WithAll<mouseStatus_component>()
           .WithReadOnly(collisionWorld)
           .WithBurst()
           .WithoutBurst()
           .WithDisposeOnCompletion(thisUnitWasSelectedByUserQuerryArray)
           .WithDisposeOnCompletion(navAgentComponentLookup)
           .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
           .ForEach((
               EntityCommandBuffer commandBuffer,
               ref mouseStatus_component localMouseStatus_component,
               in unitSelection_shiftButtonStatus_component localShiftButtonStatusComponent
           ) =>
            {

                //execute box selection once draging was already true but the mouse button just got released
                if (!localMouseStatus_component.leftMouseButtonDown &&
                    localMouseStatus_component.mouseDragBoxSelectionActive)
                {
                    NativeList<DistanceHit> tempResults = new NativeList<DistanceHit>(Allocator.Temp);

                    NativeList<Entity> detectedEntites = new NativeList<Entity>(Allocator.Temp);

                    var boxMiddlePoint = localMouseStatus_component.mousePostionAtStartOfLeftMouseButtonDownTerrain  + (localMouseStatus_component.mousePositionAtStopOfLeftMouseButtonDownTerrain 
                                                                                                                        - localMouseStatus_component.mousePostionAtStartOfLeftMouseButtonDownTerrain ) / 2;

                    var boxHalfExtends = math.distance(
                        localMouseStatus_component.mousePostionAtStartOfLeftMouseButtonDownTerrain,
                         boxMiddlePoint);
                    

                    collisionWorld.OverlapBox(boxMiddlePoint, quaternion.identity, boxHalfExtends, ref tempResults,
                        filter,
                        QueryInteraction.IgnoreTriggers);

                    int amountOfUnitsSelected = 0;

                    foreach (var resultInstance in tempResults)
                    {
                        if (navAgentComponentLookup.HasComponent(resultInstance.Entity))
                        {
                            commandBuffer.AddComponent<thisUnitWasSelectedByUser_tag>(resultInstance.Entity);
                            amountOfUnitsSelected = +1;
                            detectedEntites.Add(resultInstance.Entity);
                        }
                    }

                    //deselect old units
                    if (amountOfUnitsSelected > 0 && !localShiftButtonStatusComponent.Value)
                    {
                        foreach (var oldUnitSelectedInstance in thisUnitWasSelectedByUserQuerryArray)
                        {
                            //special case if the old unit equals the "newly" selected one
                            bool doNotRemoveTagOfThisEntity = false;
                            foreach (var detectedEntityInstance in detectedEntites)
                            {
                                if (oldUnitSelectedInstance == detectedEntityInstance)
                                {
                                    doNotRemoveTagOfThisEntity = true;
                                }    
                                
                            }

                            if (!doNotRemoveTagOfThisEntity)
                            {
                                commandBuffer.RemoveComponent<thisUnitWasSelectedByUser_tag>(oldUnitSelectedInstance);
                            }
                            
                        }
                    }

                    detectedEntites.Dispose();
                    tempResults.Dispose();

                    localMouseStatus_component.mouseDragBoxSelectionActive = false;
                }
                

                if (localMouseStatus_component.leftMouseButtonDown)
                {
                    if (math.distance(localMouseStatus_component.mouseTerrainPosition, localMouseStatus_component.mousePostionAtStartOfLeftMouseButtonDownTerrain) > 30.0f)
                    {
                        localMouseStatus_component.mouseDragBoxSelectionActive = true;
                    }

                }
                
            }).Schedule();

    }
}
