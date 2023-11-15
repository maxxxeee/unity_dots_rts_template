using Unity.Entities;

public struct unitAmountInput_component : IComponentData
{
    public int inputCountX;
    public int inputCountY;

    public int requestedAmount;

    public bool hasBeenSetYet;
}
