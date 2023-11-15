using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

//this system checks if the prerequisits to fire a projectile have been met
// if that is the case, it writes the needed information into a data component

// before entities 1.0 this system instantiated the projectile
//  entities 1.0 broke proper instantiation inside of SystemBase Systems; now this systems passes information along to an ISystem
//  to take over instantioation


[BurstCompile]
public partial class fireProjectile_System :  SystemBase
{


    [BurstCompile]
    protected override void OnUpdate()
    {
        
        ComponentLookup<projectile_component> projectileComponentFromEntity = GetComponentLookup<projectile_component>(true);

        Entities
            .WithAll<combat_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithReadOnly(projectileComponentFromEntity)
            .WithBurst()
            .ForEach((
                ref projectileInstantiationISystem_component localProjectileInstantiationISystemComponent,
                ref fireingCooldown_component localCooldownComponent,
                in LocalToWorld localToWorld,
                in hasTargetTag localHasTarget,
                in projectile_data_component localProjectileDataComponent,
                in teamTag localTeamTag,
                in readyToFire_Tag localReadyToFire
            ) =>
            {
                //do not fire if local entity currently has not target
                if (!localHasTarget.Value)
                {
                    return;
                }
                
                // save needed details to data component and set cooldown
                if (localReadyToFire.Value && localCooldownComponent.remaningCoolDown < 0.01f)
                {

                    float3 spawnOffset = new float3(0, 0.25f, 0.8f);
                        

                        localProjectileInstantiationISystemComponent.instantiationPosition =
                            localToWorld.Position + localToWorld.Forward + spawnOffset;

                        localProjectileInstantiationISystemComponent.instantiationRotation = localToWorld.Rotation;
                        

                    localProjectileInstantiationISystemComponent.damage =
                        projectileComponentFromEntity[localProjectileDataComponent.Projectile].damage;

                    localProjectileInstantiationISystemComponent.lifeTimeLeft =
                        projectileComponentFromEntity[localProjectileDataComponent.Projectile].life_time_left;

                    localProjectileInstantiationISystemComponent.Velocity =
                        localToWorld.Forward * localProjectileDataComponent.projectileSpeed;


                    localProjectileInstantiationISystemComponent.instationTeamTag = localTeamTag.Value;
                    

                    localProjectileInstantiationISystemComponent.needToInstantiate = true;
                    
                    localCooldownComponent.remaningCoolDown = localCooldownComponent.cooldownAmount;
                    
                }

            }).ScheduleParallel();
         
    }
}
