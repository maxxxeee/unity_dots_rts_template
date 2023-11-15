using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

//this system checks if a target override is in reach to be attacked

// this system was also supposed to contain logic to persue a target override out of reach
//  but this logic was moved to persueTargetOverride_System 


[UpdateAfter(typeof(fireCooldown_System))]
[BurstCompile]
public partial class targetOverride_System : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        ComponentLookup<LocalToWorld> localToWorldFromEntity = GetComponentLookup<LocalToWorld>(true);

        Entities
            .WithAll<targetOverride_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithBurst()
            .WithReadOnly(localToWorldFromEntity)
            .ForEach((
                ref targetOverride_component localTargetOverrideComponent,
                ref readyToFire_Tag localReadyToFireTag,
                in combat_component localCombatComponent, 
                in Parent localParent
            ) =>
            {
                var targetOverridePosition =
                    localToWorldFromEntity[localTargetOverrideComponent.targetOverride].Position;

                var parentWorldPosition = localToWorldFromEntity[localParent.Value].Position;

                var distanceToOverrideTarget = math.distance(parentWorldPosition, targetOverridePosition);
                
                
                if (distanceToOverrideTarget > localCombatComponent.attackRange)
                {
                    
                    localTargetOverrideComponent.closeEnoughToAttack = false;
                    localReadyToFireTag.Value = false;
                    
                }
                else
                {
                    localTargetOverrideComponent.closeEnoughToAttack = true;
                    localReadyToFireTag.Value = true;
                }

            }).Schedule();

    }
}
