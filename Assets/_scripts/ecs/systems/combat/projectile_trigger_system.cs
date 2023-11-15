using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

//  this system acts on unity physics trigger events which occure when a projectile colides
// it only actually reacts if the collision was triggered by colliding with an enemy unit
// in that case the projectile substracts it's given damage number from the enemy's unit health
// after which the projectile gets destroyed


[BurstCompile]
struct projectileTriggerJob : ITriggerEventsJob
{
    public ComponentLookup<projectile_component> ProjectileComponentData;
   [ReadOnly] public ComponentLookup<teamTag> TeamTagData;
    public ComponentLookup<health_component> healthComponentData;
    
    [BurstCompile]
    public void Execute ( TriggerEvent evt )
    {
        Entity entity1 = evt.EntityA;
        Entity entity2 = evt.EntityB;

        
        //ignore if both entites have the projectile component
        if (ProjectileComponentData.HasComponent(entity1) && ProjectileComponentData.HasComponent(entity2))
        {
            return;
        }
        
        
        if (healthComponentData.HasComponent(entity1))
        {

            if (ProjectileComponentData.HasComponent(entity2))
            {

                health_component entity1healthData = healthComponentData[entity1];

                projectile_component entity2ProjectileData = ProjectileComponentData[entity2];
                
                if (!ProjectileComponentData[entity2].hasDealtDamage)
                {
                    if (TeamTagData[entity2].Value != TeamTagData[entity1].Value)
                    {
                        entity1healthData.Value -= entity2ProjectileData.damage;

                        healthComponentData[entity1] = entity1healthData;
                        
                        entity2ProjectileData.hasDealtDamage = true;

                        ProjectileComponentData[entity2] = entity2ProjectileData;
                        
                    }
                }
                
            }
           
        }
          
         
        if( healthComponentData.HasComponent(entity2))
        {

            if (ProjectileComponentData.HasComponent(entity1))
            {
                health_component entity2healthData = healthComponentData[entity2];

                projectile_component entity1ProjectileData = ProjectileComponentData[entity1];
                
                if (!ProjectileComponentData[entity1].hasDealtDamage)
                {
                    if (TeamTagData[entity1].Value != TeamTagData[entity2].Value)
                    {

                        entity2healthData.Value -= entity1ProjectileData.damage;

                        healthComponentData[entity2] = entity2healthData;
                        
                        entity1ProjectileData.hasDealtDamage = true;

                        ProjectileComponentData[entity1] = entity1ProjectileData;
                        
                    }
                }

            }
        }
       
    }
}



[BurstCompile]
public partial class projectile_trigger_system : SystemBase
{

   
    Simulation _simulationSingleton;
    
    [BurstCompile]
    protected override void OnCreate ()
    {
        RequireForUpdate( 
            GetEntityQuery( 
                new EntityQueryDesc{
                    All = new ComponentType[]
                    {
                        typeof(projectile_component)
                    }
        } ) );
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var job = new  projectileTriggerJob
        {
            ProjectileComponentData = GetComponentLookup<projectile_component>( isReadOnly:false ) ,
            healthComponentData = GetComponentLookup<health_component>( isReadOnly:false ) ,
            TeamTagData = GetComponentLookup<teamTag>( isReadOnly:true )
        };
        Dependency = job.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), Dependency);
    }
    
}

