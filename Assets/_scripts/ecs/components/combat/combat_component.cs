using Unity.Entities;
using Unity.Mathematics;


[System.Serializable]
public struct combat_component : IComponentData
{
    public quaternion originalRotation;
    public float3 originalPosition;
    public bool originalOrientationHasBeenSetYet;

    public float attackRange;
}