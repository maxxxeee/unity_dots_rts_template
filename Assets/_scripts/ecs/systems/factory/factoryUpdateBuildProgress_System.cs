using Unity.Entities;
using Unity.Burst;


// this system updates the build progress of a factory building a unit
//      and writing that data into a factoryBuildProgress_component

// this data will be used by the gameObject ui

[BurstCompile]
public partial class factoryUpdateBuildProgress_System : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {

        ComponentLookup<unitConstruction_component> unitConstructionComponentLookup =
            GetComponentLookup<unitConstruction_component>();

        Entities
            .WithBurst()
            .WithAll<factoryBuildProgress_component>()
            .WithReadOnly(unitConstructionComponentLookup)
            .ForEach((
                Entity localEntity,
                ref factoryBuildProgress_component localFactoryBuildProgressComponent,
                in factoryProperties_component localFactoryPropertiesComponent
            ) =>
            {
                if (localFactoryPropertiesComponent.currentlyBuilding != Entity.Null && unitConstructionComponentLookup.HasComponent(localFactoryPropertiesComponent.currentlyBuilding))
                {
                    localFactoryBuildProgressComponent.Value = 1 - 
                        (unitConstructionComponentLookup[localFactoryPropertiesComponent.currentlyBuilding].buildTimeLeft /
                        localFactoryPropertiesComponent.buildTime);

                    localFactoryBuildProgressComponent.factoryEntityIndex = localEntity.Index;
                }
            }).ScheduleParallel();
    }
}
