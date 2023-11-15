using Unity.Entities;
using Unity.Mathematics;

public partial struct targetOverride_component : IComponentData
{
   public Entity targetOverride;
   
   public bool closeEnoughToAttack;
   public bool inPersuit;

   public float3 persuitPosition;
}
