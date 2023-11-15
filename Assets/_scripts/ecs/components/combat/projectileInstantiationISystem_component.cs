using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;

public struct projectileInstantiationISystem_component : IComponentData
{
   public bool needToInstantiate;

   public float3 instantiationPosition;
   public quaternion instantiationRotation;
   public int instationTeamTag;
   public float3 Velocity;
   public float lifeTimeLeft;
   public float damage;
}
