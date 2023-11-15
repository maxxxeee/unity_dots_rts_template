using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Physics;

[BurstCompile]
public partial class projectile_system : SystemBase
{
    
    //this system monitors projectiles, applies constant velocity and destroys the projectile if it deals damage
    //  or it's lifetime expires

    [BurstCompile]
    protected override void OnUpdate()
    {
        var deltaTime = World.Time.DeltaTime; 
        
        Entities
            .WithAll<projectile_component>()
            .WithBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ForEach((
                Entity localEntity,
                EntityCommandBuffer commandBuffer,
                ref projectile_component localProjectile_Component

            ) =>
            {
                //apply a constant velocity so that the projectile moves with constant speed
                if (localProjectile_Component.life_time_left > localProjectile_Component.life_time_left - 0.01f)
                {
                    PhysicsVelocity velocity = new PhysicsVelocity
                    {
                        Linear = localProjectile_Component.velocity,
                        Angular = float3.zero
                    };
                    

                    commandBuffer.SetComponent(localEntity, velocity);
                    
                    localProjectile_Component.justFired = false;
                }
                
                //destroy given projectile if it has dealt damage
                if (localProjectile_Component.hasDealtDamage)
                {
                    commandBuffer.DestroyEntity(localEntity);
                }
                else
                {
                   
                    //count down projectile life time
                    if (localProjectile_Component.life_time_left > 0)
                    {
                        localProjectile_Component.life_time_left -= deltaTime;
                    }
                    //destroy projectile if lifetime has expired
                    else
                    {
                        commandBuffer.DestroyEntity(localEntity);
                    }
                }
                
            }).ScheduleParallel();
        
       
    }
    
    
    
}
