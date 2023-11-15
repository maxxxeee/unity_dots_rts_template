using Unity.Entities;

public struct factoryBuildProgress_component : IComponentData
{
   public float Value;
   public int factoryEntityIndex;
}
