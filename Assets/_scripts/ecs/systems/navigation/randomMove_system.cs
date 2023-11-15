using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

//this system is used inside a benchmark / mass spawn scene
// it is supposed to create a constant load of the navMesh being used by units to get around the map

// this system will check if a unit has reached its current destination
//  and will trigger the generation of a new random destination for this unit

// if a new position has been generated it will tell the navAgent of a unit to move to that position,
//  provided it has not been done yet

[BurstCompile]
public partial class randomMove_system : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        
        Entities
            .WithAll<NavAgent_Component>()
                 .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<unitConstruction_component>()
            .WithBurst()
            
                 .ForEach((
                    ref NavAgent_Component localNavAgentComponent,
                    ref NavAgent_ToBeRoutedTag localToBeRoutedTag,
                    ref DynamicBuffer<NavAgent_Buffer> localNavAgentBuffer,
                    ref UnitComponentData localUnitComponentData,
                    ref navigationStates_component localNavigationStates,
                    in LocalTransform localTransform,
                    in randomMoveTargetPosition targetPosition
            ) =>
                 {
                     
                     if (localUnitComponentData.reached)
                     {
                         localNavigationStates.movingSinceNewRandomPositionHasBenGenerated = false;
                     }
                     
                     
                     if (localNavigationStates.moveToZero)
                     {
                         return;
                     }
                     
                     if (!localUnitComponentData.reached)
                     {
                         return;
                     }

                     if (localToBeRoutedTag.Value)
                     {
                         return;
                     }
                     

                     
                     
                     if (localUnitComponentData.reached ||localNavAgentBuffer.Length <= 1 || (math.distance(targetPosition.Value, localTransform.Position) < 15.0f))
                     {
                         if (math.distance(targetPosition.Value, float3.zero) < 5.0f)
                         {

                             localNavigationStates.movingSinceNewRandomPositionHasBenGenerated = false;
                              localNavigationStates.needNewRandomPosition = true;
                              localNavigationStates.newRandomPositionHasBeenGenerated = false;

                         }
                         
                         if(math.distance(targetPosition.Value, localTransform.Position) < 5.0f && !localNavigationStates.movingSinceNewRandomPositionHasBenGenerated)
                         {

                             localNavigationStates.movingSinceNewRandomPositionHasBenGenerated = false;
                              localNavigationStates.needNewRandomPosition = true;
                              localNavigationStates.newRandomPositionHasBeenGenerated = false;

                         }
                         
                         else
                         {
                             if (!localNavigationStates.movingSinceNewRandomPositionHasBenGenerated && localNavigationStates.newRandomPositionHasBeenGenerated)
                             {

                                 localNavAgentComponent.fromLocation = localTransform.Position;
                                 localNavAgentComponent.toLocation = targetPosition.Value;
                                 localNavAgentComponent.routed = false;

                                 localToBeRoutedTag.Value = true;
                                 localNavigationStates.movingSinceNewRandomPositionHasBenGenerated = true;
                                 

                                 localUnitComponentData.reached = false;
                                 localUnitComponentData.currentBufferIndex = 0;


                             }
                         }
                         
                     }
                     
                 }).ScheduleParallel();
    }

}

