using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

//this system selects an enemy according to requirements
// requirements are usually: belongs to enemy team, has a health/navAgent component
// selects enemy that is closest to itself

[UpdateAfter(typeof(enemyDetection_System_physics))]
[BurstCompile]
public partial class enemy_selection_system : SystemBase
{

    private EntityQueryDesc detectedUnitsComponentQuerry;
    
    protected override void OnCreate()
    {

        detectedUnitsComponentQuerry = new EntityQueryDesc
        {
            All = new ComponentType[]
            { 
                typeof(detected_units_list_component),
            }
        };
        
        
        RequireForUpdate( 
            GetEntityQuery( 
                detectedUnitsComponentQuerry
            ) );
    }
    
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        ComponentLookup<LocalToWorld> localToWorldFromEntity = GetComponentLookup<LocalToWorld>(true);
        ComponentLookup<AboutToBeDestroyed_Tag> aboutToBeDestroyedTagLookup =
            GetComponentLookup<AboutToBeDestroyed_Tag>();
        ComponentLookup<teamTag> teamTagLookup = GetComponentLookup<teamTag>();
        ComponentLookup<NavAgent_Component> navAgentComponentLookup = GetComponentLookup<NavAgent_Component>();
        
        ComponentLookup<targetOverride_component> targetOverrideComponentLookup =
            GetComponentLookup<targetOverride_component>();


        Entities
            .WithAll<combat_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithBurst()
            .WithReadOnly(localToWorldFromEntity)
            .WithReadOnly(aboutToBeDestroyedTagLookup)
            .WithReadOnly(teamTagLookup)
            .WithReadOnly(navAgentComponentLookup)
            .WithReadOnly(targetOverrideComponentLookup)
            .ForEach((
                Entity localEntity,
                ref currentTargetComponent localCurrentTarget,
                ref detected_units_list_component localDetectedUnitList,
                ref hasTargetTag localHasTarget,
                in teamTag localTeamTag,
                in LocalToWorld localToWorld

            ) =>
            {


                if (targetOverrideComponentLookup.HasComponent(localEntity))
                {
                    if (targetOverrideComponentLookup[localEntity].closeEnoughToAttack)
                    {
                        return;
                    }
                }
                else
                {
                    if (localHasTarget.Value)
                    {
                        return;
                    }
                }
                


                var local_results = localDetectedUnitList.results;
                float shortestDistanceToEnemySoFar = float.MaxValue;
                
                
                //filter out any results not meeting requirements
                
                foreach (var resultInstance in local_results)
                {
                    if (aboutToBeDestroyedTagLookup.HasComponent(resultInstance) ||
                        !localToWorldFromEntity.HasComponent(resultInstance) ||
                        !teamTagLookup.HasComponent(resultInstance) ||
                        !navAgentComponentLookup.HasComponent(resultInstance) ||
                        (teamTagLookup[resultInstance].Value == localTeamTag.Value))
                    {
                        local_results.Remove(resultInstance);
                    }
                    
                }
                
                //check for empty list
                if (local_results.IsEmpty)
                {
                    return;
                }

                //get distance to first valid target
                
                shortestDistanceToEnemySoFar = math.distance(localToWorldFromEntity[local_results[0]].Position, localToWorld.Position);
                localCurrentTarget.currentTarget = local_results[0];
                
                
                
                foreach (var hitInstance in local_results)
                {

                    var hitInstanceLocalToWorld = localToWorldFromEntity[hitInstance];

                    var distanceToHitInstance = math.distance(hitInstanceLocalToWorld.Position, localToWorld.Position);

                    if (shortestDistanceToEnemySoFar > distanceToHitInstance)
                    {
                        localCurrentTarget.currentTarget = hitInstance;
                       
                        
                        
                        shortestDistanceToEnemySoFar = distanceToHitInstance;
                    }

                }

                if (!(localCurrentTarget.currentTarget == Entity.Null))
                {
                    localHasTarget.Value = true;
                }
                

            }).ScheduleParallel();
        
    }
}
