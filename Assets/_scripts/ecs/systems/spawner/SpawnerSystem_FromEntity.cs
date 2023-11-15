using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

//old spawner system based on systemBase
//  Broke with entites 1.0 release
//  replaced by ISystem based spawner

[BurstCompile]
[DisableAutoCreation]
public partial class SpawnerSystem_FromEntity : SystemBase
{
    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<Spawner_FromEntity>();
    }
    
    [BurstCompile]
    struct SetSpawnedTranslation : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalTransform> TranslationFromEntity;

        public NativeArray<Entity> Entities;
        public float4x4 LocalToWorld;
        public int Stride;
        
        public float xCorrection;
        public float zCorrection;

        public void Execute(int i)
        {
            var entity = Entities[i];
            var y = i / Stride;
            var x = i - (y * Stride);

            TranslationFromEntity[entity] = new LocalTransform()
            {
                Position = math.transform(LocalToWorld, new float3(x * 1.3F * xCorrection, 0, y * 1.3F * zCorrection))
            };
        }
    }
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .WithBurst()
            .ForEach((
                Entity entity,
                in Spawner_FromEntity spawnerFromEntity,
                in LocalToWorld spawnerLocalToWorld
                ) =>
        {
            Dependency.Complete();

            
            
            var spawnedCount = spawnerFromEntity.CountX * spawnerFromEntity.CountY;
            var spawnedEntities =
                new NativeArray<Entity>(spawnedCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            
            EntityManager.Instantiate(spawnerFromEntity.Prefab, spawnedEntities);
            EntityManager.DestroyEntity(entity);
            
            var translationFromEntity = GetComponentLookup<LocalTransform>();
            var setSpawnedTranslationJob = new SetSpawnedTranslation
            {
                TranslationFromEntity = translationFromEntity,
                Entities = spawnedEntities,
                LocalToWorld = spawnerLocalToWorld.Value,
                Stride = spawnerFromEntity.CountX,
                zCorrection = spawnerFromEntity.zCorrection,
                xCorrection = spawnerFromEntity.xCorrection
                
            };

            Dependency = setSpawnedTranslationJob.Schedule(spawnedCount, 64, Dependency);
        }).Run();
    }
    
    
    
}
