using UnityEngine;
using Unity.Entities;
using Unity.Physics;

// authoring scripts are used to transfer information set in editor and from prefabs to an entity
// they also add and set needed components

// this combat authoring is meant to be used with a factory
// it adds a unit construction component and adds navigation and combat blocker components
// it also adds a team tag to itself according to its gameObject teamTag

public class CombatAuthoringFromFactory : MonoBehaviour
{
    public GameObject Projectile;
    
    public float fireCooldown_TimeAmount;
    public float damageDealt;
    public float fireringRange;
    public float projectileSpeed;

    public float enemyCheckCooldownAmount;
}


public class CombatAuthoringFromFactory_Baker : Baker<CombatAuthoringFromFactory>
{
    public override void Bake(CombatAuthoringFromFactory authoring)
    {
        
        var entity = GetEntity(TransformUsageFlags.None);
        
        CollisionFilter filter = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = ~0u,
            GroupIndex = 0
        };

        if (authoring.transform.tag == "Team1")
        {
            filter.BelongsTo = 8u;
            filter.CollidesWith = 9u;
        }
        else
        {
            filter.BelongsTo = 9u;
            filter.CollidesWith = 8u;
        }
        
        

        var tempCombatComponent = new combat_component();

        tempCombatComponent.attackRange = authoring.fireringRange;
        

        var tempProjectileDataComponent = new projectile_data_component()
        {
            projectileSpeed = authoring.projectileSpeed,
            Projectile =  GetEntity(authoring.Projectile, TransformUsageFlags.Dynamic)
            
        };
        

        var tempFireCooldownComponent = new fireingCooldown_component()
        {
            cooldownAmount = authoring.fireCooldown_TimeAmount,
            remaningCoolDown = 3.0f
        };
        
        
        var tempReadyToFireTag = new readyToFire_Tag()
        {
            Value = false
        };
        
        
        AddComponent(entity, tempCombatComponent);
        AddComponent(entity, tempProjectileDataComponent);
        AddComponent(entity, tempFireCooldownComponent);
        AddComponent(entity, tempReadyToFireTag);
        
        AddComponent(entity, new currentTargetComponent());
        AddComponent(entity, new hasTargetTag());
        AddComponent(entity, new detected_units_list_component());
       
        
        
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
        
        AddComponent(entity, new combatDisabled_tag());
        
        AddComponent<projectileInstantiationISystem_component>(entity);
        
        AddComponent<unitSelectionHighlightShader_component>(entity);
     
        AddComponent<enemyScanCooldown_component>(entity);

        var tempEnemyScanCooldownComponent = new enemyScanCooldown_component()
        {
            initCooldownTime = authoring.enemyCheckCooldownAmount,
            cooldownCounter = authoring.enemyCheckCooldownAmount
        };
            
        SetComponent(entity, tempEnemyScanCooldownComponent);

        
    }
}

