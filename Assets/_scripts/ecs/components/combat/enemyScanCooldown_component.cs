using Unity.Entities;

public struct enemyScanCooldown_component : IComponentData
{
   public float initCooldownTime;
   public float cooldownCounter;
}
