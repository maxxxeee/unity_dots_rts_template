using Unity.Entities;
using Unity.Mathematics;

public struct projectile_component : IComponentData
{
   public float life_time_left;
   public float damage;

   public float3 velocity;
   
   public bool hasDealtDamage;
   public bool justFired;
   
   
}
