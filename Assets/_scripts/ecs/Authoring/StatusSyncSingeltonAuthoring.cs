using UnityEngine;
using Unity.Entities;

// authoring scripts are used to transfer information set in editor and from prefabs to an entity
// they also add and set needed components

// this authoring represents a singelton which stores data needed for systems
// the sync from gameObject system syncs it's data to this singelton

// this adds components storing different button statuses

public class StatusSyncSingeltonAuthoring : MonoBehaviour
{
}


public class StatusSyncSingeltonAuthoring_Baker : Baker<StatusSyncSingeltonAuthoring>
{
    public override void Bake(StatusSyncSingeltonAuthoring authoring)
    {
        
        var entity = GetEntity(TransformUsageFlags.None);

        var tempMouseStatusComponent = new mouseStatus_component();

        tempMouseStatusComponent.leftMouseButtonDown = false;
        tempMouseStatusComponent.rightMouseButtonDown = false;
        
        AddComponent(entity, tempMouseStatusComponent);

        AddComponent(entity, new unitSelection_shiftButtonStatus_component());
        
        AddComponent(entity, new existingUnitsCounter_component());
        
    }

}