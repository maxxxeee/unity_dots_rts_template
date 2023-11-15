using UnityEngine;
using Unity.Entities;
using Unity.Transforms;


//this system syncs data of selected units to a list inside of a gameObject to be used for the user interface

public partial class UI_sync_to_gameObject_system : SystemBase
{
    
    private EntityQueryDesc UIUnitHealthBarSyncQuerry;
    
    protected override void OnCreate()
    {

        UIUnitHealthBarSyncQuerry = new EntityQueryDesc
        {
            All = new ComponentType[]
            { 
                typeof(UIUnitHealthBarSync_component),
            }
        };
        
        
        RequireForUpdate( 
            GetEntityQuery( 
                UIUnitHealthBarSyncQuerry
            ) );
    }

    
    
    protected override void OnUpdate()
    {

        
        //
        // ui hover health bar sync
        //
        

        var UIMouseSyncHoverGameObjects = GameObject.FindObjectsByType<UIPositionSyncWithHoverEntity>(FindObjectsSortMode.InstanceID);
        
        
        ComponentLookup<health_component> healthComponentFromEntity = GetComponentLookup<health_component>(true);
        ComponentLookup<thisUnitWasSelectedByUser_tag> thisUnitWasSelectedByUserTagLookup =
            GetComponentLookup<thisUnitWasSelectedByUser_tag>();
        

        Entities
            .WithoutBurst()
            .WithReadOnly(healthComponentFromEntity)
            .WithAll<UIUnitHealthBarSync_component>()
            .ForEach((
                ref UIUnitHealthBarSync_component localUIHealthBarSyncComponent
            ) =>
            {
                foreach (var UIMouseSyncGameObjectInstance in UIMouseSyncHoverGameObjects)
                {

                    if (localUIHealthBarSyncComponent.detected_Entity.Equals(Entity.Null) || thisUnitWasSelectedByUserTagLookup.HasComponent(localUIHealthBarSyncComponent.detected_Entity))
                    {
                        UIMouseSyncGameObjectInstance.isVisible = false;
                    }
                    else
                    {
                        UIMouseSyncGameObjectInstance.isVisible = true;

                        UIMouseSyncGameObjectInstance.targetPosition = localUIHealthBarSyncComponent.syncPosition;
                        UIMouseSyncGameObjectInstance.currentHealthValue = healthComponentFromEntity
                            .GetRefRO(localUIHealthBarSyncComponent.detected_Entity).ValueRO.Value;
                        UIMouseSyncGameObjectInstance.maxHealthValue = healthComponentFromEntity
                            .GetRefRO(localUIHealthBarSyncComponent.detected_Entity).ValueRO.maxValue;
                    }
                  
                }
                
            }).Run();


        //
        // factory build status sync
        //
        
        var factoryBuildStatusCoordinationManagers = GameObject.FindObjectsByType<UIFactoryBuildProgress_Coordinator>(FindObjectsSortMode.InstanceID);

        if (factoryBuildStatusCoordinationManagers.Length > 0)
        {
            factoryBuildStatusCoordinationManagers[0].factoryBuildProgressStructList.Clear();
        
            Entities
                .WithoutBurst()
                .WithAll<factoryBuildProgress_component>()
                .ForEach((
                    Entity localEntity,
                    in factoryBuildProgress_component localFactoryBuildProgressComponent,
                    in factoryProperties_component localFactoryPropertiesComponent,
                    in factoryBuildQuque localFactoryBuildQuque,
                    in LocalToWorld localToWorldComponent
                ) =>
                {
                    foreach (var factoryBuildStatusCoordinationManagerInstance in factoryBuildStatusCoordinationManagers)
                    {
                        var tempFactoryBuildStatusStruct = new factoryBuildProgressUISyncStruct
                        {
                            entityIndex = localEntity.Index,
                            factoryBuildProgress = localFactoryBuildProgressComponent.Value,
                            factoryCurrentlyBuilding = localFactoryPropertiesComponent.currentlyBuilding != Entity.Null,
                            factoryPosition = localToWorldComponent.Position,
                            factoryBuildQuqueSize = localFactoryBuildQuque.buildQuque.Length
                        };
                    
                        factoryBuildStatusCoordinationManagerInstance.factoryBuildProgressStructList.Add(tempFactoryBuildStatusStruct);

                    }

                }).Run();
        }
        
       
        
        
        //
        // unit selection ui sync
        //
        
        //legacy code
        // this code was used to create gameObject based visualizations around selected units
        // this was replaced by a shader based solution
        
        /*
        
        var uiUnitSelectionCoordinator = GameObject.FindObjectsByType<UIUnitSelection_Coordinator>(FindObjectsSortMode.InstanceID);

        if (uiUnitSelectionCoordinator.Length > 0)
        {
            uiUnitSelectionCoordinator[0].selectedUnitsUiSyncStruct.Clear();
        
            Entities
                .WithoutBurst()
                .WithAll<thisUnitWasSelectedByUser_tag>()
                .ForEach((
                    //Entity localEntity,
                    in LocalToWorld localToWorldComponent
                ) =>
                {
               
                    var tempUnitSelectionUIStruct = new unitSelectionUISyncStruct
                    {
                        unitPosition = localToWorldComponent.Position
                    };
                    
                    uiUnitSelectionCoordinator[0].selectedUnitsUiSyncStruct.Add(tempUnitSelectionUIStruct);

                }).Run(); 

        }
        
       */
        
        //
        // unit health bar postion sync
        //
    
        
        
        var healtBarUICoordinator = GameObject.FindObjectsByType<UIUnitHealthbar_Coordinator>(FindObjectsSortMode.InstanceID);

        if (healtBarUICoordinator.Length > 0)
        {
            healtBarUICoordinator[0].healthBarUiSyncStruct.Clear();
        
            Entities
                .WithoutBurst()
                .WithAll<thisUnitWasSelectedByUser_tag>()
                .ForEach((
                    //Entity localEntity,
                    in LocalToWorld localToWorldComponent,
                    in health_component localHealthComponent
                ) =>
                {
               
                    var temphealthUIStruct = new unitHealthBarUISyncStruct
                    {
                        unitPosition = localToWorldComponent.Position,
                        currentHealthValue = localHealthComponent.Value,
                        maxHealthValue = localHealthComponent.maxValue
                    
                    };
                    
                    healtBarUICoordinator[0].healthBarUiSyncStruct.Add(temphealthUIStruct);

                }).Run(); 
        }
        
        
        
        //
        // amount of existing units sync to gameObject
        //
        
        var amountOfExistingUnitsgameObject = GameObject.FindObjectsByType<existingAmountOfUnits_ui_logic>(FindObjectsSortMode.InstanceID);

        if (amountOfExistingUnitsgameObject.Length > 0)
        {

            Entities
                .WithoutBurst()
                .WithAll<existingUnitsCounter_component>()
                .ForEach((
                    //Entity localEntity,
                    in existingUnitsCounter_component localExistingUnitsCounterComponent
                ) =>
                {

                    amountOfExistingUnitsgameObject[0].amountOfExistingUnits =
                        localExistingUnitsCounterComponent.amountOfExistingUnits;
                    
                    amountOfExistingUnitsgameObject[0].amountOfExistingFactories =
                        localExistingUnitsCounterComponent.amountOfExistingFactories;


                }).Run(); 
        }
        
    }
    
}
