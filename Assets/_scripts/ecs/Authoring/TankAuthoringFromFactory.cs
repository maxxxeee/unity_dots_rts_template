using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

// authoring scripts are used to transfer information set in editor and from prefabs to an entity
// they also add and set needed components

// this combat authoring is meant to be used with a factory
// it adds a unit construction component and adds navigation and combat blocker components
// it also adds a team tag to itself according to its gameObject teamTag


public class TankAuthoringFromFactory : MonoBehaviour
{
       public float givenSpeed = 5.0f;
       public float givenMinDistance;
       
       public float3 givenOffset;
       
       public float healthPointsAmount;
}

public class TankAuthoringFromFactory_Baker : Baker<TankAuthoringFromFactory>
{
       public override void Bake(TankAuthoringFromFactory authoring)
       {
         
              var entity = GetEntity(TransformUsageFlags.None);
              
              
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

              var tempHealthComponent = new health_component();

              tempHealthComponent.Value = authoring.healthPointsAmount;
              tempHealthComponent.maxValue = authoring.healthPointsAmount;
              

              var tempUnitComponentData = new UnitComponentData()
              {
                   speed =  authoring.givenSpeed,
                   currentBufferIndex = 0,
                   minDistance = authoring.givenMinDistance,
                   offset = authoring.givenOffset,
                   reached = false
                   
              };
              

              var tempToBeRoutedTag = new NavAgent_ToBeRoutedTag()
              {
                     Value = false
              };

              var tempBoidComponent = new Boid_ComponentData
              {
                     acceleration = 1.0f,
                     alignmentBias = 1.0f,
                     cellSize = 2,
                     cohesionBias = 1.0f,
                     perceptionRadius = 15.0f
              };
              

              
              //AddComponent(entity, new randomMoveTargetPosition());
              AddComponent(entity, tempHealthComponent);
              AddComponent(entity, tempUnitComponentData);
              AddBuffer<NavAgent_Buffer>(entity);
              AddComponent(entity, tempToBeRoutedTag);
              AddComponent(entity, new NavAgent_Component());
              AddComponent(entity, new unitConstruction_component{buildTimeLeft = 300.0f});
              AddComponent<Boid_ComponentData>(entity);
              SetComponent(entity, tempBoidComponent);

              
              AddComponent<unitSelectionHighlightShader_component>(entity);
       } 
       
}