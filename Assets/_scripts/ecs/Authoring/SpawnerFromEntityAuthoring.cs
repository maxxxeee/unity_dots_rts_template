using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

// authoring scripts are used to transfer information set in editor and from prefabs to an entity
// they also add and set needed components

// this authoring sets data for an entity spawner to spawn a certain amount of units per frame
// it also applies an x and z coordinate offset so that spawners still work as intended if any of their coordinates are negative


public class SpawnerFromEntityAuthoring : MonoBehaviour
{
    public GameObject Prefab;

    public float xCoordinateCorrection = 1.0f;
    public float zCoordinateCorrection = 1.0f;

    public int requestedAmountToSpawn;

    public int maxAmountToSpawnPerFrame;

    public bool userInputEnabled;

}

public class SpawnerFromEntityAuthoring_Baker : Baker<SpawnerFromEntityAuthoring>
{
    public override void Bake(SpawnerFromEntityAuthoring authoring)
    {
        if (authoring.transform.position.x < 0.0f)
        {
            authoring.xCoordinateCorrection = -1.0f;
        }

        if (authoring.transform.position.z < 0.0f)
        {
            authoring.zCoordinateCorrection = -1.0f;
        }
        

        var entity = GetEntity(TransformUsageFlags.None);
        
        
        var spawnerData = new Spawner_FromEntity
        {
            Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            xCorrection = authoring.xCoordinateCorrection,
            zCorrection = authoring.zCoordinateCorrection,
            requestedAmount = authoring.requestedAmountToSpawn,
            maxAmountOfUnitsToSpawnPerFrame = authoring.maxAmountToSpawnPerFrame,
            userInputEnabled = authoring.userInputEnabled
            
        };
        AddComponent(entity, spawnerData);

        AddComponent( new LocalTransform{Position = authoring.transform.position, Rotation = authoring.transform.rotation});
        
    }
}
