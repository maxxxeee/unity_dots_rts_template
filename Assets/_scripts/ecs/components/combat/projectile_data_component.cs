using UnityEngine;
using Unity.Entities;

public struct projectile_data_component : IComponentData
{
    public float projectileSpeed;
    
    public Entity Projectile;
}
