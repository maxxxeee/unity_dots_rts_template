using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

//this system checks if a target of an entity is in reach and adjusts its readyToFire value accordingly


[UpdateBefore(typeof(fireCooldown_System))]
[BurstCompile]
public partial class inRangeCheck_system : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        
        ComponentLookup<LocalToWorld> localToWorldLookup = GetComponentLookup<LocalToWorld>(true);
        
        Entities
            .WithAll<fireingCooldown_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithReadOnly(localToWorldLookup)
            .WithBurst()
            .ForEach((
                Entity localEntity,     
                ref readyToFire_Tag localReadyToFire,
                in currentTargetComponent localCurrentTargetComponent,
                in combat_component localCombatComponent
               
            ) =>
            {
                if (localCurrentTargetComponent.currentTarget == Entity.Null)
                {
                    return;
                }
                
                var localEntityLocalToWorld = localToWorldLookup.GetRefRO(localEntity);
                
                var localTargetLocalToWorld = localToWorldLookup.GetRefRO(localCurrentTargetComponent.currentTarget);


                if (math.distance(localEntityLocalToWorld.ValueRO.Position, localTargetLocalToWorld.ValueRO.Position) < localCombatComponent.attackRange)
                {
                    localReadyToFire.Value = true;
                }
                else
                {
                    localReadyToFire.Value = false;
                }
                

            }).ScheduleParallel();
    }
}
