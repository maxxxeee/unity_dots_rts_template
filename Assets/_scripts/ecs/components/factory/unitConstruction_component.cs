using Unity.Entities;
using Unity.Mathematics;

public struct unitConstruction_component : IComponentData
{
   public float buildTimeLeft;
   
   public float4 originalColor;

   public bool hasOriginialColorBeenSetYet;
}
