using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial class UnitMovement : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = World.Time.DeltaTime;
        Entities
            .WithAll<NavAgent_Component>()
            .WithAll<UnitComponentData>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<unitConstruction_component>()
            .WithBurst()
            .ForEach((
                ref LocalTransform localTransform,
                ref UnitComponentData localUnitComponentData,
                ref DynamicBuffer<NavAgent_Buffer> localNavAgentBuffer,
                in NavAgent_Component localNavAgentComponent,
                in NavAgent_ToBeRoutedTag localToBeRoutedTag
            ) =>
            {
                if (localToBeRoutedTag.Value)
                {
                    return;
                }
                
                if (localNavAgentBuffer.Length < 1)
                {
                    return;
                }

                
                if (localUnitComponentData.reached)
                {
                    return;
                }
                
                
                //if agent has been routed and has waypoints left
                if (localNavAgentComponent.routed && localNavAgentBuffer.Length > 0)
                {
                    //calc waypoint direction by subtracting own position from waypoint position 
                    localUnitComponentData.waypointDirection = math.normalize((localNavAgentBuffer[localUnitComponentData.currentBufferIndex].wayPoints) - localTransform.Position);
                    
                    // move agent towards current waypoint by adding waypoint direction plus offset multiplied by speed and delta time
                    localTransform.Position += (math.normalize(localUnitComponentData.waypointDirection + localUnitComponentData.offset)) * localUnitComponentData.speed * deltaTime;
                    
                    // if the agent has reached its target location + has the minimum distance to the waypoint + current waypoint from buffer is not the last one
                    //TODO: find out where the reached boolean is getting changed
                    
                    if (!localUnitComponentData.reached 
                        && math.distance(localTransform.Position, localNavAgentBuffer[localUnitComponentData.currentBufferIndex].wayPoints) <= localUnitComponentData.minDistance 
                        && localUnitComponentData.currentBufferIndex < localNavAgentBuffer.Length) // -1
                    {
                        //TODO: delete reached waypoints from buffer?
                        
                        // assign the next waypoint as the current one
                        localUnitComponentData.currentBufferIndex = localUnitComponentData.currentBufferIndex + 1;
                        
                        //own: remove old waypoint from buffer

                        //localNavAgentBuffer.RemoveAt(localUnitComponentData.currentBufferIndex - 1);
                        
                        // set status to reached if the currently reached waypoint is the last one
                        
                        //if (localUnitComponentData.currentBufferIndex == localNavAgentBuffer.Length - 1)
                        if (localUnitComponentData.currentBufferIndex == localNavAgentBuffer.Length)
                        //if (localNavAgentBuffer.Length <= 1)
                        {
                            localUnitComponentData.reached = true;
                            
                            //remove all buffered locations but the latest one
                            
                            //for (int i = 0; i < localNavAgentBuffer.Length - 1; i++)
                            //{
                            //    localNavAgentBuffer.RemoveAt(i);
                            //}
                            
                            //own: clear out buffer to set unit to idle
                            localNavAgentBuffer.Clear();
                        }
                        
                    }
                    
                    
                    
                    //TODO: does this trigger the backtravel?
                    
                    /*
                    
                    // if the unit has not yet reached the minDistance to the current waypoint and the current waypoint index from buffer is not 0 or smaller 
                    else if (localUnitComponentData.reached && math.distance(trans.Value, localNavAgentBuffer[localUnitComponentData.currentBufferIndex].wayPoints) <= localUnitComponentData.minDistance && localUnitComponentData.currentBufferIndex > 0)
                    {
                        
                        // set the current waypoint to the one before the current one
                        localUnitComponentData.currentBufferIndex = localUnitComponentData.currentBufferIndex - 1;
                        
                        // if the index of the current waypoint is 0, set reached to false
                        if (localUnitComponentData.currentBufferIndex == 0)
                        {
                            localUnitComponentData.reached = false;
                        }
                    }
                    */
                }
                
            //}).ScheduleParallel();
            }).ScheduleParallel();
    }
}
