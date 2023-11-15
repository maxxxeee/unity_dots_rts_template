using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public partial struct UIMouse_Position_Sync_component : IComponentData
{
   public float3 currentMouseWorldPos;
   public float3 ScreenMouseRayDirection;
   public float3 mainCameraPosition;
}
