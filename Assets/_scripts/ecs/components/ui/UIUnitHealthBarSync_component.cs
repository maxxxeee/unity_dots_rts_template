using Unity.Entities;
using Unity.Mathematics;

public struct UIUnitHealthBarSync_component : IComponentData
{
    public Entity detected_Entity;
    public float3 syncPosition;
}
