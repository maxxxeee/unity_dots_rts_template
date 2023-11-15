using Unity.Entities;
using Unity.Mathematics;

public partial struct mouseStatus_component : IComponentData
{
    //generic information
    public bool leftMouseButtonDown;
    public bool rightMouseButtonDown;
    public float3 mouseTerrainPosition;

    //mouse drag selection information

    public bool mouseDragBoxSelectionActive;

    public float3 mousePostionAtStartOfLeftMouseButtonDownTerrain;
    public float3 mousePositionAtStopOfLeftMouseButtonDownTerrain;
}
