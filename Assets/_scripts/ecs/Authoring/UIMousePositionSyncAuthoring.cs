using UnityEngine;
using Unity.Entities;

// authoring scripts are used to transfer information set in editor and from prefabs to an entity
// they also add and set needed components

// this authoring represents a singelton which stores data needed for systems
// the sync from gameObject system syncs it's data to this singelton

// this adds components storing information about the mouse coursers location on the screen and in world
// it also adds data storage for the ui hover health bar component

public class UIMousePositionSyncAuthoring : MonoBehaviour
{
}


public class UIMousePositionSyncAuthoring_Baker : Baker<UIMousePositionSyncAuthoring>
{
    public override void Bake(UIMousePositionSyncAuthoring authoring)
       {
           var entity = GetEntity(TransformUsageFlags.None);
              
              AddComponent<UIMouse_Position_Sync_component>(entity);
              AddComponent<UIUnitHealthBarSync_component>(entity);
       } 
}