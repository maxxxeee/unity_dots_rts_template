using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

//this system detects if the current target of a unit is currently not reachable anymore
// by either
// being beyond the units combat range
// or by being destroyed / non existent

[BurstCompile]
public partial class target_outOfReach_System : SystemBase
{
    
    //rotates unit's turret back to default rotation once the target has left its reach
    [BurstCompile]
    protected override void OnUpdate()
    {
        
        ComponentLookup<LocalToWorld> localToWorldFromEntity = GetComponentLookup<LocalToWorld>(true);

        ComponentLookup<AboutToBeDestroyed_Tag> aboutToBeDestroyedTagLookup =
            GetComponentLookup<AboutToBeDestroyed_Tag>(true);

        ComponentLookup <targetOverride_component> targetOverrideComponentLookup = GetComponentLookup<targetOverride_component>(true);

        Entities
            .WithBurst()
            .WithAll<combat_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<unitConstruction_component>()
            .WithReadOnly(localToWorldFromEntity)
            .WithReadOnly(aboutToBeDestroyedTagLookup)
            .WithReadOnly(targetOverrideComponentLookup)
            .ForEach((
               Entity localEntity,
               ref currentTargetComponent currentTargetComponent,
                ref hasTargetTag localHasTarget,
                ref detected_units_list_component localDetectedUnitsListComponent,
               in combat_component localCombatComponent,
                in LocalToWorld localToWorld
               
            ) =>
            {

                //do not check if current target is out of reach if the local entity has a target override component
                if (targetOverrideComponentLookup.HasComponent(localEntity) &&  currentTargetComponent.currentTarget == targetOverrideComponentLookup[localEntity].targetOverride)
                {
                    return;
                }

                //check if current target is still a valid target
                if (aboutToBeDestroyedTagLookup.HasComponent(currentTargetComponent.currentTarget) ||  currentTargetComponent.currentTarget.Equals(Entity.Null) || !localHasTarget.Value || !localToWorldFromEntity.HasComponent(currentTargetComponent.currentTarget))
                {
                    currentTargetComponent.currentTarget = Entity.Null;
                    localHasTarget.Value = false;
                    localDetectedUnitsListComponent.results.Remove(currentTargetComponent.currentTarget);

                    return;
                }

                
                if (!currentTargetComponent.currentTarget.Equals(Entity.Null) || localHasTarget.Value)
                {
                    //if target is too far away fromm current position
                    if( math.distance(localToWorld.Position , localToWorldFromEntity[currentTargetComponent.currentTarget].Position) > localCombatComponent.attackRange + 5.0f)
                    {
                         localDetectedUnitsListComponent.results.Remove(currentTargetComponent.currentTarget);
                      
                         currentTargetComponent.currentTarget = Entity.Null;

                         localHasTarget.Value = false;
                    }
                }
                
            }).ScheduleParallel();
    }
}
