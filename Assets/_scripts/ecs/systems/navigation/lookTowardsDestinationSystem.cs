using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

// this system rotates an entity contaning a navAgent component towards it's current destination
// the navMesh system itself will not rotate an Agent towards its destination. it will only sync its position with the entity

[BurstCompile]
public partial class lookTowardsDestinationSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities
           .WithBurst()
           .WithAll<NavAgent_Component>()
           .WithNone<AboutToBeDestroyed_Tag>()
           .WithNone<unitConstruction_component>()
           .ForEach((
               ref LocalTransform localTransform,
               in NavAgent_Component localNavAgentComponent,
               in UnitComponentData localUnitComponentData
           ) =>
           {
               if (localNavAgentComponent.routed)
               {
                   float3 directionToCurrentWaypoint = (localUnitComponentData.waypointDirection);
                       directionToCurrentWaypoint = math.normalize(directionToCurrentWaypoint);

                       var tempVector = directionToCurrentWaypoint * new float3(1, 0, 1);

                       if (math.distance(tempVector, float3.zero) > 0)
                       {
                           localTransform.Rotation = quaternion.LookRotation(tempVector, new float3(0.0f, 1.0f, 0.0f));
                       }
                       else
                       {
                           localTransform.Rotation = quaternion.LookRotation(new float3(0.001f, 0.001f, 0.001f), new float3(0.0f, 1.0f, 0.0f));
                       }
               }
           }).ScheduleParallel();
    }
}
