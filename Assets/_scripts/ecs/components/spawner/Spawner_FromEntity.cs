using Unity.Entities;

public struct Spawner_FromEntity : IComponentData
{
    public int CountX;
    public int CountY;
    public int requestedAmount;

    public bool userInputEnabled;
    
    public int maxAmountOfUnitsToSpawnPerFrame;
    public int spawnedUnitsSoFar;
    public int xSpawnGrid;
    public int zSpawnGrid;

    public Entity Prefab;
    
    public float xCorrection;
    public float zCorrection;
    
    public bool hasSpawendUnitsAlready;
}
