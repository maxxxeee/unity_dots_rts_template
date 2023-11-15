using Unity.Entities;

public struct moveToZero_Tag : IComponentData
{
    public bool Value;
    public bool hasBeenMoved;
}
