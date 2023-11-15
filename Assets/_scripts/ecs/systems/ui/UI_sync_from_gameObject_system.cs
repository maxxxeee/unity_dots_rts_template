using UnityEngine;
using Unity.Entities;

//this systems syncs needed data from gameObjects and saves them inside of data components


public partial class UI_sync_from_gameObject_system : SystemBase
{
    private EntityQueryDesc UIMousePoistionSyncQuerry;

    private GameObject localTerrainGameObject;
    private TerrainCollider localTerrainCollider;
    
    protected override void OnCreate()
    {

        UIMousePoistionSyncQuerry = new EntityQueryDesc
        {
            All = new ComponentType[]
            { 
                typeof(UIMouse_Position_Sync_component),
            }
        };
        
        RequireForUpdate( 
            GetEntityQuery( 
                UIMousePoistionSyncQuerry
            ) );
    }

    
    
    protected override void OnUpdate()
    {

        var mainCamera = Camera.main;
        
        
        Entities
            .WithoutBurst()
            .WithAll<UIMouse_Position_Sync_component>()
            .ForEach((
                ref UIMouse_Position_Sync_component localUIMousePositionSyncComponent,
                ref mouseStatus_component localMouseStatusComponent
            ) =>
            {
                localTerrainGameObject = GameObject.FindGameObjectWithTag("Terrain");
                localTerrainCollider = localTerrainGameObject.GetComponent<TerrainCollider>();
                
                Vector3 currentMouseWorldPosition = Vector3.zero;

                //camera game object sync
                if(mainCamera != null)
                {

                    var mousePos = Input.mousePosition;

                    mousePos.z = mainCamera.nearClipPlane;

                    localUIMousePositionSyncComponent.ScreenMouseRayDirection = mainCamera.ScreenPointToRay(mousePos).direction;

                    localUIMousePositionSyncComponent.mainCameraPosition = mainCamera.transform.position;
                    
                    
                    
                    currentMouseWorldPosition =  mainCamera.ScreenToWorldPoint(mousePos);
                    
                   
                   localUIMousePositionSyncComponent.currentMouseWorldPos = currentMouseWorldPosition;
                }

                //      mouse status sync
                
                //mouse location on terrain
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if ( localTerrainCollider.Raycast (ray, out hit, Mathf.Infinity))
                {
                    localMouseStatusComponent.mouseTerrainPosition = hit.point;
                }
                
                
                //save mouse position on left click change to detect mouse drag
                if (!localMouseStatusComponent.leftMouseButtonDown && Input.GetMouseButton(0))
                {
                    localMouseStatusComponent.mousePostionAtStartOfLeftMouseButtonDownTerrain = localMouseStatusComponent.mouseTerrainPosition;
                }
                
                if (localMouseStatusComponent.leftMouseButtonDown && !Input.GetMouseButton(0))
                {
                    localMouseStatusComponent.mousePositionAtStopOfLeftMouseButtonDownTerrain = localMouseStatusComponent.mouseTerrainPosition;
                }
                
                
                //mouse buttons
                localMouseStatusComponent.leftMouseButtonDown = Input.GetMouseButton(0);
                localMouseStatusComponent.rightMouseButtonDown = Input.GetMouseButton(1);


            }).Run();
        
        
        //
        // shift button status sync
        //
        
        Entities
            .WithAll<unitSelection_shiftButtonStatus_component>()
            .WithBurst()
            .ForEach((
                ref unitSelection_shiftButtonStatus_component localShiftStatusComponent
            ) =>
            {

                localShiftStatusComponent.Value = Input.GetKey(KeyCode.LeftShift);

            }).Run();
    }
}
