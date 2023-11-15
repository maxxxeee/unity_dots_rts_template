using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


//[DisableAutoCreation]
//[UpdateBefore(typeof(UnitMovement))]
//[UpdateBefore(typeof(NavAgent_System))]
[BurstCompile]
public partial class NavAgentPosition_Sync : SystemBase
{
    protected override void OnUpdate()
    {

        Entities
            .WithAll<NavAgent_Component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<unitConstruction_component>()
            .WithBurst()
            .ForEach((
                    //Entity e,
                    ref NavAgent_Component localNavAgentComponent,
                    in LocalTransform localTransform
                ) =>
                {
                    //if (math.distance(float3.zero, localNavAgentComponent.fromLocation) < 3.0f)
                    //{
                        var tempLocation = localTransform.Position;

                        //tempLocation.y = 0.6f;
                    
                        localNavAgentComponent.fromLocation = tempLocation;
                    //}
                    
                }
            ).ScheduleParallel();
    }
}
