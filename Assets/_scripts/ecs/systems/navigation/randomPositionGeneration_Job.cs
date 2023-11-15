using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Random = Unity.Mathematics.Random;

// old system of generation random positions for the randomMove System
// currently not in use

[DisableAutoCreation]
[BurstCompile]
public partial class randomPositionGenerator_systemJob : SystemBase
{
    EntityQuery m_Group;
    
    public NativeArray<Random> _randoms = new NativeArray<Random>(JobsUtility.MaxJobThreadCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
    public uint r = (uint) UnityEngine.Random.Range(int.MinValue, int.MaxValue);


    public NativeArray<Entity> entityArrayFromNeedNewPositions;

    protected override void OnCreate()
    {

        m_Group = GetEntityQuery(ComponentType.ReadWrite<needNewPositionTag>(), ComponentType.ReadWrite<randomMoveTargetPosition>());

        for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
        {
            
            if (r == 0)
            {
                r += 1;
            }
            
            _randoms[i] = new Random(r);

            r += 1;
            
        }

    }
    
    
    [BurstCompile]
    public partial struct randomPositionGenerationJob : IJob
    {

        public ComponentLookup<needNewPositionTag> NeedNewPositionComponentDataFromEntity;
        public ComponentLookup<NewRandomPosHasBeenGeneratedTag> NewPositionHasBeenGeneratedComponentDataFromEntity;
        public ComponentLookup<randomMoveTargetPosition> RandomMoveTargetPositionComponentDataFromUnity;
    

        [Unity.Collections.LowLevel.Unsafe.NativeDisableContainerSafetyRestriction]
        public NativeArray<Random> Randoms;
        [Unity.Collections.LowLevel.Unsafe.NativeSetThreadIndex] private int _threadId;

        public NativeArray<Entity> NeedNewPositionEntityArray;

        

        public void Execute()
        {

            for (var index = 0; index < NeedNewPositionEntityArray.Length; index++)
            {
                if (!NeedNewPositionComponentDataFromEntity[NeedNewPositionEntityArray[index]].Value)
                {
                    return;
                }
            
                var rnd      = Randoms[_threadId];

                var temp = rnd.NextFloat3(new float3(500, 0, 500), new float3(-500, 0, -500));
                
                
                var tempRandomTargetPosition = new randomMoveTargetPosition()
                {
                    Value = temp
                };
                
            
                RandomMoveTargetPositionComponentDataFromUnity[NeedNewPositionEntityArray[index]] = tempRandomTargetPosition;

                NewPositionHasBeenGeneratedComponentDataFromEntity[NeedNewPositionEntityArray[index]] =
                    new NewRandomPosHasBeenGeneratedTag() { Value = true };

                NeedNewPositionComponentDataFromEntity[NeedNewPositionEntityArray[index]] = new needNewPositionTag() { Value = false };

                Randoms[_threadId] = rnd;
            }
        
        }
    }

    
    
    protected override void OnUpdate()
    {

        entityArrayFromNeedNewPositions = m_Group.ToEntityArray(Allocator.TempJob);
        
        var job = new randomPositionGenerationJob()
        {
            Randoms = _randoms,
            RandomMoveTargetPositionComponentDataFromUnity = GetComponentLookup<randomMoveTargetPosition>(false),
            NeedNewPositionComponentDataFromEntity = GetComponentLookup<needNewPositionTag>(false),
            NewPositionHasBeenGeneratedComponentDataFromEntity = GetComponentLookup<NewRandomPosHasBeenGeneratedTag>(false),
            NeedNewPositionEntityArray = entityArrayFromNeedNewPositions

        };
        Dependency = job.Schedule(Dependency);
    }
    
    
    protected override void OnDestroy()
    {
        _randoms.Dispose();
        entityArrayFromNeedNewPositions.Dispose();
    }
}
