using Unity.Entities;
using Unity.Burst;

//this system counts down a fire cooldown which gets set after a projectile has been fired
// once the cooldown reaches zero the readyToFire Value gets set to true
// also takes target override into account

[BurstCompile]
public partial class fireCooldown_System : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        var deltaTime = World.Time.DeltaTime;

        ComponentLookup<targetOverride_component> targetOverrideComponentLookup =
            GetComponentLookup<targetOverride_component>(true);
        
        Entities
            .WithAll<fireingCooldown_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithReadOnly(targetOverrideComponentLookup)
            .WithBurst()
            .ForEach((
                    Entity localEntity,
                    ref fireingCooldown_component local_fireingCooldown_component,
                    ref readyToFire_Tag localReadyToFire
            ) =>
                {
                    bool localEntityHasTargetOverride = targetOverrideComponentLookup.HasComponent(localEntity);
                
                if (local_fireingCooldown_component.remaningCoolDown <= 0.0f)
                {
                    if (!localEntityHasTargetOverride)
                    {
                        localReadyToFire.Value = true;
                    }
                   
                }
                else
                {
                    local_fireingCooldown_component.remaningCoolDown -= deltaTime;

                    if (!localEntityHasTargetOverride)
                    {
                        localReadyToFire.Value = false;
                    }
                }

                
                }).ScheduleParallel();
    }
}
