using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// authoring scripts are used to transfer information set in editor and from prefabs to an entity
// they also add and set needed components

// this factory authoring creates a factory with a given build list
// it also adds a team tag to itself according to its gameObject teamTag

public class FactoryAuthoring : MonoBehaviour
{

    public List<GameObject> buildOptions;

    public GameObject buildOption0;
    public GameObject buildOption1;
    public GameObject buildOption2;

    public float3 newUnitPositionOffset;
    public float buildTime;

    public float health = 500;

    public List<int> buildQuque;

    public Entity convertedEntity;
    public int convertedEntityIndex;
    


    private void OnEnable()
    {
        buildQuque.Clear();
    }

    private void Start()
    {
        buildQuque.Clear();
    }
}


public class FactoryAuthoring_Baker : Baker<FactoryAuthoring>
{
 
      public override void Bake(FactoryAuthoring authoring)
       {
         
           authoring.buildQuque.Clear();
           
              var entity = GetEntity(TransformUsageFlags.None);
              
              
              AddComponent<factoryBuildOptions>(entity);

              
              var tempBuildOptions = new factoryBuildOptions();
              

              if (authoring.buildOption0 != null)
              {
                  tempBuildOptions.buildOption0 = GetEntity(authoring.buildOption0, TransformUsageFlags.Dynamic);
              }

              if (authoring.buildOption1 != null)
              {
                  tempBuildOptions.buildOption1 = GetEntity(authoring.buildOption1, TransformUsageFlags.Dynamic);
              }
            
              if (authoring.buildOption2 != null)
              {
                  tempBuildOptions.buildOption2 = GetEntity(authoring.buildOption2, TransformUsageFlags.Dynamic); 
              }
              
             
              

              SetComponent(entity, tempBuildOptions);

              AddComponent<factoryBuildQuque>(entity);
              
              AddComponent<health_component>(entity);

              var tempHealthComponent = new health_component
              {
                  Value = authoring.health,
                  maxValue = authoring.health
              };

              SetComponent(entity, tempHealthComponent);
              
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
              
              AddComponent<factoryProperties_component>(entity);
              
              SetComponent(entity, new factoryProperties_component{ buildTime = authoring.buildTime, gameObjectIndex = authoring.GetInstanceID(), newUnitPositionOffset = authoring.newUnitPositionOffset});

              AddComponent<LocalTransform>(entity);

              var tempFactoryProgressComponent = new factoryBuildProgress_component
              {
                  factoryEntityIndex = -1,
                  Value = 0.0f
              };
              
              AddComponent<factoryBuildProgress_component>(entity);
              SetComponent(entity, tempFactoryProgressComponent);
              
              authoring.convertedEntity = entity;
              authoring.convertedEntityIndex = entity.Index;

       } 
}