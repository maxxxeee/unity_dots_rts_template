using Unity.Entities;

public partial struct navigationStatesInteractive_component : IComponentData
{
    public bool movementCommandHasBeenIssued;
    public bool movingSinceMovementCommandIssue;
}
