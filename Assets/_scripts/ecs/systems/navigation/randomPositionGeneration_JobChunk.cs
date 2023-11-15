using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Jobs.LowLevel.Unsafe;
using Random = Unity.Mathematics.Random;

// system to create random positions for units one chunk at a time
// the range has been limited to +-1500 on x and z achsis
//  whilst y is always kept at 0

[BurstCompile]
public partial class randomPositionGenerator_systemJobChunk : SystemBase
{
    EntityQuery m_Group;
    
    public NativeArray<Random> _randoms = new NativeArray<Random>(JobsUtility.MaxJobThreadCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
    public uint r = (uint) UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    
    protected override void OnCreate()
    {
        
        m_Group = GetEntityQuery(ComponentType.ReadWrite<navigationStates_component>());
        

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
    public partial struct randomPositionGenerationJobChunk : IJobChunk
    {
        
        public ComponentTypeHandle<randomMoveTargetPosition> RandomMoveTargetPositionComponentTypeHandle;
        public ComponentTypeHandle<navigationStates_component> NavigationStatesComponentTypeHandle;

        [Unity.Collections.LowLevel.Unsafe.NativeDisableContainerSafetyRestriction]
        public NativeArray<Random> Randoms;
        [Unity.Collections.LowLevel.Unsafe.NativeSetThreadIndex] private int _threadId;
    
    
   
        
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var chunkRandomTargetPosition = chunk.GetNativeArray(ref RandomMoveTargetPositionComponentTypeHandle);
            var chunkNavigationStates = chunk.GetNativeArray(ref NavigationStatesComponentTypeHandle);
            
            
            for (var i = 0; i < chunk.Count; i++)
            {
                if (!chunkNavigationStates[i].needNewRandomPosition)
                {
                    continue;
                }
            
                var rnd      = Randoms[_threadId];

                var tempRandomTargetPosition = new randomMoveTargetPosition()
                {
                    Value = rnd.NextFloat3(new float3(1500, 0, 1500), new float3(-1500, 0, -1500))
                };
            
                chunkRandomTargetPosition[i] = tempRandomTargetPosition;
                
                chunkNavigationStates[i] = new navigationStates_component()
                {
                    newRandomPositionHasBeenGenerated = true,
                    needNewRandomPosition = false,
                    moveToZero = chunkNavigationStates[i].moveToZero,
                    moveToZeroHasBeenMoved = chunkNavigationStates[i].moveToZeroHasBeenMoved,
                    movingSinceNewRandomPositionHasBenGenerated =
                        chunkNavigationStates[i].movingSinceNewRandomPositionHasBenGenerated
                };
                
                Randoms[_threadId] = rnd;
            }
        
        }
    }

    
    [BurstCompile]
    protected override void OnUpdate()
    {
        var job = new randomPositionGenerationJobChunk()
        {
            Randoms = _randoms,
            RandomMoveTargetPositionComponentTypeHandle = GetComponentTypeHandle<randomMoveTargetPosition>(),
            NavigationStatesComponentTypeHandle = GetComponentTypeHandle<navigationStates_component>()
            
        };
        Dependency = job.ScheduleParallel(m_Group, Dependency);
    }
    
    [BurstCompile]
    protected override void OnDestroy()
    {
        _randoms.Dispose();
    }
}
