using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//old spawner system based on systemBase
//  Broke with entites 1.0 release
//  replaced by ISystem based spawner

[DisableAutoCreation]
[BurstCompile]

public partial class spawnFromEntityForEachLoop : SystemBase
{
    [BurstCompile]
    protected override void OnCreate()
    {
        // This makes the system not update unless at least one entity exists that has the Spawner component.
        RequireForUpdate<Spawner_FromEntity>();
    }

    
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .WithoutBurst()
            .ForEach((
                Entity entity,
                ref Spawner_FromEntity spawnerFromEntity,
                in LocalToWorld spawnerLocalToWorld
            ) =>
            {
                if (!spawnerFromEntity.hasSpawendUnitsAlready)
                {
                    Debug.Log("Units should get spawned now!");
                    
                    var spawnedCount = spawnerFromEntity.CountX * spawnerFromEntity.CountY;
                    //var spawnedEntities = new NativeArray<Entity>(spawnedCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            
                   // EntityManager.Instantiate(spawnerFromEntity.Prefab, spawnedEntities);
                    
                    var instances = EntityManager.Instantiate(spawnerFromEntity.Prefab, spawnedCount, Allocator.Temp);

                    int entityIndex = 0;
                    var translationFromEntity = GetComponentLookup<LocalTransform>();

                    for (var x = 0; x < SystemAPI.GetSingleton<Spawner_FromEntity>().CountX; x++)
                    {
                        for (var y = 0; y < SystemAPI.GetSingleton<Spawner_FromEntity>().CountY; y++)
                        {

                            var instanceTranslation = translationFromEntity.GetRefRW(instances[entityIndex]);

                            instanceTranslation.ValueRW = new LocalTransform()
                            {
                                Position = math.transform(spawnerLocalToWorld.Value,
                                    new float3(x * 1.3F * spawnerFromEntity.xCorrection, 0,
                                        y * 1.3F * spawnerFromEntity.zCorrection))
                            };

                            entityIndex += 1;

                        }
                    }

                    spawnerFromEntity.hasSpawendUnitsAlready = true;
                    
                    //EntityManager.DestroyEntity(entity);
                }

            }).Run();
    }


}
