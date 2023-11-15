using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct navAgentWaypoints_component : IComponentData
{
    public FixedList512Bytes<float3> waypoints;
}
