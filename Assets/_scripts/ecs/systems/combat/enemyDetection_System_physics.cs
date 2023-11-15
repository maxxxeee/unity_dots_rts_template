using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Burst;

//this system detects units around itself by using a sphere cast
// it adds as many detected units which are not part of its own team to a detectedUnits list which will be analyzed in another system


[BurstCompile]
public partial class enemyDetection_System_physics : SystemBase
{

    private EntityQueryDesc combatComponentQuerry;
    
    protected override void OnCreate()
    {

        combatComponentQuerry = new EntityQueryDesc
        {
            All = new ComponentType[]
            { 
                typeof(combat_component),
            }
        };

        RequireForUpdate( 
            GetEntityQuery( 
                combatComponentQuerry
            ) );
    }
    
    private PhysicsWorld physWorld;

    [BurstCompile]
    protected override void OnUpdate()
    {
        CollisionFilter filter = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = ~0u,
            GroupIndex = 0
        };
        
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;

       ComponentLookup<teamTag> teamTagFromEntity = GetComponentLookup<teamTag>(true);

       ComponentLookup<targetOverride_component> targetOverrideComponentLookup =
           GetComponentLookup<targetOverride_component>();


       Entities
           .WithAll<combat_component>()
           .WithNone<combatDisabled_tag>()
           .WithNone<AboutToBeDestroyed_Tag>()
           .WithReadOnly(teamTagFromEntity)
           .WithReadOnly(collisionWorld)
           .WithReadOnly(targetOverrideComponentLookup)
            .WithBurst()
           .ForEach((
               Entity localEntity,
               ref detected_units_list_component local_detectedUnitList,
               ref enemyScanCooldown_component localEnemyScanCooldownComponent,
                in combat_component local_combatComponent,
                in LocalToWorld localToWorld,
                in teamTag localTeamTag
               
            ) =>
            {
                if (targetOverrideComponentLookup.HasComponent(localEntity) && targetOverrideComponentLookup[localEntity].closeEnoughToAttack)
                {
                    return;
                }

                if (localEnemyScanCooldownComponent.cooldownCounter < 0.0f)
                {
                    localEnemyScanCooldownComponent.cooldownCounter = localEnemyScanCooldownComponent.initCooldownTime;
                }
                
                else
                {
                    localEnemyScanCooldownComponent.cooldownCounter -= SystemAPI.Time.DeltaTime;
                    return;
                }
                
                
                NativeList<DistanceHit> tempResults = new NativeList<DistanceHit>(Allocator.Temp);
                
                
                
                if (localTeamTag.Value == 1)
                {
                    filter.BelongsTo = 1 << 8;
                    filter.CollidesWith = 1 << 9;
                }
                else
                {
                    filter.BelongsTo = 1 << 9;
                   filter.CollidesWith = 1 << 8;
                }
                
                
                
                collisionWorld.OverlapSphere(localToWorld.Position, local_combatComponent.attackRange, ref tempResults, filter, QueryInteraction.IgnoreTriggers);
                
               
                
                    foreach (var resultInstance in tempResults)
                    {
                        if(!localTeamTag.Value.Equals(teamTagFromEntity[resultInstance.Entity].Value))
                     
                           if (!local_detectedUnitList.results.Contains(resultInstance.Entity) && local_detectedUnitList.results.Length < local_detectedUnitList.results.Capacity)
                           {
                               local_detectedUnitList.results.Add(resultInstance.Entity);
                           }
                        
                    }
                    

                tempResults.Dispose();

            }).ScheduleParallel();
        
    }
    
}
