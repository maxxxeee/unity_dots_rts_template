using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;


// authoring scripts are used to transfer information set in editor and from prefabs to an entity
// they also add and set needed components

// this tank authoring is meant to be used with an entity spawner as it used for benchmark purposes
// it adds a random move component to simulate constant load on the navMesh system
// it also adds a team tag to itself according to its gameObject teamTag

public struct teamTag : IComponentData
{
       public int Value;
}


public struct randomMoveTargetPosition : IComponentData
{
       public float3 Value;
}

public struct needNewPositionTag : IComponentData
{
       public bool Value;
}


public class TankAuthoring : MonoBehaviour
{
       public float givenSpeed = 5.0f;
       public float givenMinDistance;
       
       public float3 givenOffset;
       
       public bool moveToZeroOnSpawn;
       public float healthPointsAmount;
}

public class TankAuthoring_Baker : Baker<TankAuthoring>
{
       public override void Bake(TankAuthoring authoring)
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
              

              var tempNewNavigationStatesComponent = new navigationStates_component()
              {
                     moveToZero = authoring.moveToZeroOnSpawn,
                     moveToZeroHasBeenMoved = false,
                     movingSinceNewRandomPositionHasBenGenerated = false,
                     needNewRandomPosition = false,
                     newRandomPositionHasBeenGenerated = false
              };
              

              
              AddComponent(entity, new randomMoveTargetPosition());
              AddComponent(entity, tempHealthComponent);
              AddComponent(entity, tempUnitComponentData);
              AddBuffer<NavAgent_Buffer>(entity);
              AddComponent(entity, tempToBeRoutedTag);
              AddComponent(entity, tempNewNavigationStatesComponent);
              AddComponent(entity, new NavAgent_Component());

              AddComponent<unitSelectionHighlightShader_component>(entity);

       } 
       
}