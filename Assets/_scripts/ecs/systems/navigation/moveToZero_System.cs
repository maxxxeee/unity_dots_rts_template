using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

//this system is used inside a benchmark / mass spawn scene
// it will move all units towards the middle of the scene to create a meeting point between opposing teams
// this system will only be active until a unit has reached the zero coordinates once

[BurstCompile]
public partial class moveToZero_System : SystemBase
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
                ref navigationStates_component localNavigationStates,
                ref UnitComponentData localUnitComponentData,
                ref NavAgent_ToBeRoutedTag localToBeRouted,
                ref DynamicBuffer<NavAgent_Buffer> localNavAgentBuffer,
                in LocalToWorld localTranslation

            ) =>
            {
                if (!localNavigationStates.moveToZero)
                {
                    return;
                }
                
               
                if (!localNavigationStates.moveToZeroHasBeenMoved)
                {
                    localNavAgentComponent.fromLocation = localTranslation.Position;
                    localNavAgentComponent.toLocation = float3.zero;
                    localNavAgentComponent.routed = false;

                    localToBeRouted.Value = true;

                   
                    localNavigationStates.moveToZeroHasBeenMoved = true;
                }
                else
                {
                    if (math.distance(localTranslation.Position, float3.zero) < 75.0f)
                   
                    {
                        localUnitComponentData.reached = true;
                        localNavAgentBuffer.Clear();
                        localNavigationStates.moveToZero = false;
                        localNavigationStates.needNewRandomPosition = true;
                        localNavigationStates.movingSinceNewRandomPositionHasBenGenerated = false;
    
                    }
                }
                
                if (localNavigationStates.moveToZero && localNavAgentBuffer.Length <= 1 && localNavigationStates.moveToZeroHasBeenMoved && localNavAgentComponent.routed)
                {
                    localNavigationStates.moveToZero = false;
                }
            }).ScheduleParallel();
    }

}

