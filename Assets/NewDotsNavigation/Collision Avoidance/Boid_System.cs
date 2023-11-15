using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public partial class Boid_System : SystemBase
{
    public NativeParallelMultiHashMap<int, Boid_ComponentData> cellVsEntityPositions;

    public static int GetUniqueKeyForPosition(float3 position, int cellSize)
    {
        return (int)((15 * math.floor(position.x / cellSize)) + (17 * math.floor(position.y / cellSize)) + (19 * math.floor(position.z / cellSize)));
    }

    protected override void OnCreate()
    {
        cellVsEntityPositions = new NativeParallelMultiHashMap<int, Boid_ComponentData>(0, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        EntityQuery eq = GetEntityQuery(typeof(Boid_ComponentData));
        cellVsEntityPositions.Clear();
        if (eq.CalculateEntityCount() > cellVsEntityPositions.Capacity)
        {
            cellVsEntityPositions.Capacity = eq.CalculateEntityCount();
        }

        NativeParallelMultiHashMap<int, Boid_ComponentData>.ParallelWriter cellVsEntityPositionsParallel = cellVsEntityPositions.AsParallelWriter();
        Entities.ForEach((ref Boid_ComponentData bc, ref LocalTransform localTransform) =>
        {
            bc.currentPosition = localTransform.Position;
            cellVsEntityPositionsParallel.Add(GetUniqueKeyForPosition(localTransform.Position, bc.cellSize), bc);
        }).ScheduleParallel();

        float deltaTime = World.Time.DeltaTime;
        NativeParallelMultiHashMap<int, Boid_ComponentData> cellVsEntityPositionsForJob = cellVsEntityPositions;
        Entities.WithBurst().WithReadOnly(cellVsEntityPositionsForJob).ForEach((ref Boid_ComponentData bc, ref LocalTransform localTransform) =>
        {
            int key = GetUniqueKeyForPosition(localTransform.Position, bc.cellSize);
            NativeParallelMultiHashMapIterator<int> nmhKeyIterator;
            Boid_ComponentData neighbour;
            int total = 0;
            float3 separation = float3.zero;
            float3 alignment = float3.zero;
            float3 coheshion = float3.zero;
            bc.velocity = bc.target - localTransform.Position;
            if (cellVsEntityPositionsForJob.TryGetFirstValue(key, out neighbour, out nmhKeyIterator))
            {
                do
                {
                    if (!localTransform.Position.Equals(neighbour.currentPosition) && math.distance(localTransform.Position, neighbour.currentPosition) < bc.perceptionRadius)
                    {
                        float3 distanceFromTo = localTransform.Position - neighbour.currentPosition;
                        separation += (distanceFromTo / math.distance(localTransform.Position, neighbour.currentPosition));
                        coheshion += neighbour.currentPosition;
                        alignment += neighbour.velocity;
                        total++;
                    }
                } while (cellVsEntityPositionsForJob.TryGetNextValue(out neighbour, ref nmhKeyIterator));
                if (total > 0)
                {
                    coheshion = coheshion / total;
                    coheshion = coheshion - (localTransform.Position + bc.velocity);
                    coheshion = math.normalize(coheshion) * bc.cohesionBias;

                    separation = separation / total;
                    separation = separation - bc.velocity;
                    separation = math.normalize(separation) * bc.separationBias;

                    alignment = alignment / total;
                    alignment = alignment - bc.velocity;
                    alignment = math.normalize(alignment) * bc.alignmentBias;

                }

                bc.acceleration += (coheshion + alignment + separation);
                //rot.Value = math.slerp(rot.Value, quaternion.LookRotation(math.normalize(bc.velocity), math.up()), deltaTime * 10);
                bc.velocity = bc.velocity + bc.acceleration;
                bc.velocity = math.normalize(bc.velocity) * bc.speed;
                //trans.Value = math.lerp(trans.Value, (trans.Value + bc.velocity), deltaTime * bc.step);
                bc.acceleration = math.normalize(bc.target - localTransform.Position) * bc.targetBias;
            }
        }).ScheduleParallel();
    }

    protected override void OnDestroy()
    {
        cellVsEntityPositions.Dispose();
    }
}
