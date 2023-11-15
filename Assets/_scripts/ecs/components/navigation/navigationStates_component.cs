using Unity.Entities;

public partial struct navigationStates_component : IComponentData
{
    public bool moveToZero;
    public bool moveToZeroHasBeenMoved;
    public bool needNewRandomPosition;
    public bool newRandomPositionHasBeenGenerated;
    public bool movingSinceNewRandomPositionHasBenGenerated;
}
