using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct factoryProperties_component : IComponentData
{
    public float buildTime;
    public Entity currentlyBuilding;
    public int gameObjectIndex;
    
    public float3 newUnitPositionOffset;

    public bool instantiationNeeded;
}
