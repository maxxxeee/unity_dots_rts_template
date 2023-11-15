using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

// this systems checks if an enemy unit which was assigend as a units target override is out of reach
// if that is the case it will tell the unit's navAgent to move towards that enemy unit until it is in reach again


[BurstCompile]
public partial class persueTargetOverride_System : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {

        var localToWorldLookup = GetComponentLookup<LocalToWorld>();
        var localTransformLookup = GetComponentLookup<LocalTransform>();
        
        
        var navAgentBufferLookup = GetBufferLookup<NavAgent_Buffer>();
        var navAgentComponentLookup = GetComponentLookup<NavAgent_Component>();
        var navAgentToBeRoutedTagLookup = GetComponentLookup<NavAgent_ToBeRoutedTag>();
        var unitComponentDataLookup = GetComponentLookup<UnitComponentData>();

        var movementCommandOverrideTagLookup = GetComponentLookup<movementCommandOverride_Tag>();
        
        Entities
            .WithAll<targetOverride_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithDisposeOnCompletion(localToWorldLookup)
            .WithDisposeOnCompletion(navAgentBufferLookup)
            .WithDisposeOnCompletion(navAgentComponentLookup)
            .WithDisposeOnCompletion(navAgentToBeRoutedTagLookup)
            .WithDisposeOnCompletion(unitComponentDataLookup)
            .WithDisposeOnCompletion(movementCommandOverrideTagLookup)
            .WithDisposeOnCompletion(localTransformLookup)
            .WithBurst()
            .ForEach((
                ref targetOverride_component localTargetOverrideComponent,
                in Parent localParent,
                in combat_component localCombatComponent
                
            ) =>
            {

                // if this entity has no target Override or the parent has a movement command override skip execution
                
                if (localTargetOverrideComponent.targetOverride == Entity.Null)
                {
                    localTargetOverrideComponent.inPersuit = false;
                    return;
                }

                if (movementCommandOverrideTagLookup.HasComponent(localParent.Value))
                {
                    localTargetOverrideComponent.inPersuit = false;
                    return;
                }
                
                
                //get all required data from data component lookups
                var localParentLocalTransform = localTransformLookup.GetRefRO(localParent.Value);
                
                var localTargetLocalToWorld = localToWorldLookup.GetRefRO(localTargetOverrideComponent.targetOverride);

                var localNavAgentBuffer = navAgentBufferLookup[localParent.Value];
                var localNavAgentComponent = navAgentComponentLookup.GetRefRW(localParent.Value);
                var localNavAgentToBeRoutedTag = navAgentToBeRoutedTagLookup.GetRefRW(localParent.Value);
                var localUnitComponentData = unitComponentDataLookup.GetRefRW(localParent.Value);
                
                // if target override is out of reach instruct nav agent to move towards target override
                if (math.distance(localParentLocalTransform.ValueRO.Position, localTargetLocalToWorld.ValueRO.Position) > localCombatComponent.attackRange && !localTargetOverrideComponent.inPersuit)
                {
                    
                    localNavAgentBuffer.Clear();
                       
                        
                    localNavAgentComponent.ValueRW.fromLocation = localParentLocalTransform.ValueRO.Position;
                    
                    
                    var tempTargetLocationHeading =
                        (localTargetLocalToWorld.ValueRO.Position - localParentLocalTransform.ValueRO.Position);

                    var distanceToTarget = math.length(tempTargetLocationHeading);

                    var directionToTempTarget = tempTargetLocationHeading / distanceToTarget;


                    var tempTargetLocation = directionToTempTarget * (distanceToTarget - (localCombatComponent.attackRange - (localCombatComponent.attackRange * 0.25f))); 
                    
                    
                    localNavAgentComponent.ValueRW.toLocation = tempTargetLocation;
                    localNavAgentComponent.ValueRW.routed = false;

                    localNavAgentToBeRoutedTag.ValueRW.Value = true;
                        
                        
                    localUnitComponentData.ValueRW.reached = false;
                    localUnitComponentData.ValueRW.currentBufferIndex = 0;


                    localTargetOverrideComponent.persuitPosition = tempTargetLocation;
                    localTargetOverrideComponent.inPersuit = true;
                }
                
                // if we are already in persuit but the target is once again out of reach
                if(math.distance(localTargetOverrideComponent.persuitPosition, localTargetLocalToWorld.ValueRO.Position) - 5.0f > localCombatComponent.attackRange && localTargetOverrideComponent.inPersuit)
                {
                    localNavAgentBuffer.Clear();
                    
                    localNavAgentComponent.ValueRW.fromLocation = localParentLocalTransform.ValueRO.Position;

                    var tempTargetLocationHeading =
                        (localTargetLocalToWorld.ValueRO.Position - localParentLocalTransform.ValueRO.Position);

                    var distanceToTarget = math.length(tempTargetLocationHeading);

                    var directionToTempTarget = tempTargetLocationHeading / distanceToTarget;


                    var tempTargetLocation = directionToTempTarget * (distanceToTarget - (localCombatComponent.attackRange - (localCombatComponent.attackRange * 0.25f)));


                    localNavAgentComponent.ValueRW.toLocation = tempTargetLocation;
                    localNavAgentComponent.ValueRW.routed = false;

                    localNavAgentToBeRoutedTag.ValueRW.Value = true;
                        
                        
                    localUnitComponentData.ValueRW.reached = false;
                    localUnitComponentData.ValueRW.currentBufferIndex = 0;


                    localTargetOverrideComponent.persuitPosition = tempTargetLocation;
                    localTargetOverrideComponent.inPersuit = true;

                }

            }).Schedule();
        
        
    }
}
