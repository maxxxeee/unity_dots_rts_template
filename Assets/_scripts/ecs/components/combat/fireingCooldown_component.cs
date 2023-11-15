using Unity.Entities;

public struct fireingCooldown_component : IComponentData
{
   public float remaningCoolDown;
   public float cooldownAmount;
}
