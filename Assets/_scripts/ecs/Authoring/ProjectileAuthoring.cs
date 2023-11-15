using UnityEngine;
using Unity.Entities;

// authoring scripts are used to transfer information set in editor and from prefabs to an entity
// they also add and set needed components

// this projectile authoring sets the projectiles initial data and it's team


public class ProjectileAuthoring : MonoBehaviour
{
    public float projectileLifeTime = 15.0f;

    public float damageToTarget = 25.0f;

    public float intitalVelocity = 20.0f;

}


public class ProjectileBaker : Baker<ProjectileAuthoring>
{
    public override void Bake(ProjectileAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        var tempProjectileComponent = new projectile_component();
        
        tempProjectileComponent.life_time_left = authoring.projectileLifeTime;
        tempProjectileComponent.damage = authoring.damageToTarget;
        tempProjectileComponent.hasDealtDamage = false;
        tempProjectileComponent.justFired = true;
        tempProjectileComponent.velocity = authoring.intitalVelocity;
        
        AddComponent(entity, tempProjectileComponent);

        if (authoring.transform.tag == "Team1")
        {
                     
            var team1Tag = new teamTag();
            team1Tag.Value = 1;
            
            AddComponent(entity, team1Tag);
        }

        else
        {
            var team2Tag = new teamTag();
            team2Tag.Value = 2;
            
            AddComponent(entity, team2Tag);
        }
    }
}