using Unity.Entities;
using Unity.Burst;

//this system checks if a factory currently has units on its build quque and if it is currenlty building anything
// if the factory is currently idle but has units to build in it's build quque 
//    this system will set a new unit to be build by the factory with the time requirement 


[BurstCompile]
public partial class factoryBuildFromQuque_system : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        var unitConstructionComponentLookup = GetComponentLookup<unitConstruction_component>();
        
        Entities
            .WithBurst()
            .WithAll<factoryProperties_component>()
            .WithAll<factoryBuildOptions>()
            .WithAll<factoryBuildQuque>()
            .WithReadOnly(unitConstructionComponentLookup)
            .ForEach((
                ref factoryProperties_component localFactoryPropertiesComponent,
                ref factoryBuildQuque localFactoryBuildQuque,
                in factoryBuildOptions localFactoryBuildOptions
            ) =>
            {
                //remove entity which has no unit construction component anymore
                if (!SystemAPI.HasComponent<unitConstruction_component>(localFactoryPropertiesComponent
                        .currentlyBuilding))
                {
                    localFactoryPropertiesComponent.currentlyBuilding = Entity.Null;
                }
                
                
                if (!localFactoryPropertiesComponent.currentlyBuilding.Equals(Entity.Null))
                {
                    return;
                }
                
                
                if (localFactoryBuildQuque.buildQuque.IsEmpty)
                {
                    return;
                }
                

                if (!localFactoryPropertiesComponent.currentlyBuilding.Equals(Entity.Null))
                {
                     if(unitConstructionComponentLookup.GetRefRO(localFactoryPropertiesComponent.currentlyBuilding).ValueRO.buildTimeLeft <= 0.0f)
                     {
                         localFactoryPropertiesComponent.currentlyBuilding = Entity.Null;
                     }
                }
                

                //set new entity to be build
                
                    //workaround ; currently it seems that prefabs getting converted to entities inside of an entity fixed list do not convert properly
                    Entity unitToInstantiate = localFactoryBuildOptions.buildOption0;
                    
                    //Debug.Log("will consider which build option to pick next");
                    
                    if ((int)localFactoryBuildQuque.buildQuque[0] == (int)0)
                    {
                        //Debug.Log("Set build option 0 as unit to be build!");
                        unitToInstantiate = localFactoryBuildOptions.buildOption0;
                    }
                    if (localFactoryBuildQuque.buildQuque[0] == 1)
                    {
                        unitToInstantiate = localFactoryBuildOptions.buildOption1;
                    }
                    if (localFactoryBuildQuque.buildQuque[0] == 2)
                    {
                        unitToInstantiate = localFactoryBuildOptions.buildOption2;
                    }


                    localFactoryPropertiesComponent.currentlyBuilding = unitToInstantiate;

                    localFactoryPropertiesComponent.instantiationNeeded = true;

                    localFactoryBuildQuque.buildQuque.RemoveAt(0);
                    
            }).Schedule();
        
    }
}
