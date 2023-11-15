using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;

//this system replaces the projectile instantiation which used to happen inside of the fireProjectile System

[BurstCompile]
public partial struct projectileInstantiation_ISystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This makes the system not update unless at least one entity exists that has the Spawner component.
        state.RequireForUpdate<projectileInstantiationISystem_component>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
      
        var projectileInstantiationComponents = SystemAPI.QueryBuilder().WithAll<projectileInstantiationISystem_component>().Build().ToEntityArray(Allocator.Temp);

        
        
        foreach (var projectileInstantionComponent in projectileInstantiationComponents)
        {
        
            // instantiate and set needed components for every projectile marked with "needToInstantiate"
            if (SystemAPI.GetComponent<projectileInstantiationISystem_component>(projectileInstantionComponent).needToInstantiate)
            {

                var localProjectileInstantiationISystemComponent = SystemAPI.GetComponent<projectileInstantiationISystem_component>(projectileInstantionComponent);
                
                var projectileDataComponent = SystemAPI.GetComponent<projectile_data_component>(projectileInstantionComponent);
                
                
                var prefab = projectileDataComponent.Projectile;
                
                
                var tempProjectile = state.EntityManager.Instantiate(prefab);
                
                
                var transform = SystemAPI.GetComponentRW<LocalTransform>(tempProjectile);


                var projectileComponentFromNewEntity = SystemAPI.GetComponentRW<projectile_component>(tempProjectile);
                
                projectileComponentFromNewEntity.ValueRW.damage = localProjectileInstantiationISystemComponent.damage;
                
                
                projectileComponentFromNewEntity.ValueRW.life_time_left = localProjectileInstantiationISystemComponent.lifeTimeLeft;
                
                projectileComponentFromNewEntity.ValueRW.velocity = localProjectileInstantiationISystemComponent.Velocity;


                SystemAPI.GetComponentRW<teamTag>(tempProjectile).ValueRW.Value =
                    localProjectileInstantiationISystemComponent.instationTeamTag;
                
                transform.ValueRW.Position = localProjectileInstantiationISystemComponent.instantiationPosition;

                transform.ValueRW.Rotation = localProjectileInstantiationISystemComponent.instantiationRotation;


                localProjectileInstantiationISystemComponent.needToInstantiate = false;
                
                SystemAPI.SetComponent(projectileInstantionComponent, localProjectileInstantiationISystemComponent);
            }
            
        }

        projectileInstantiationComponents.Dispose();
    }
}
