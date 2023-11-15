using Unity.Entities;
using Unity.Burst;
using Unity.Collections;


//this system syncs the amount of instantiated units and factories to a data component
// this data component will be synced to a ui gameObject by a different system

[BurstCompile]
public partial class syncAmountOfExistingUnits_system : SystemBase
{

    [BurstCompile]
    protected override void OnUpdate()
    {
        var navAgentQuery = new EntityQueryBuilder(Allocator.TempJob)
            .WithAll<NavAgent_Component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .Build(this);

        var navAgentQueryEntityArray = navAgentQuery.ToEntityArray(Allocator.TempJob);
      
        var factoryQuery = new EntityQueryBuilder(Allocator.TempJob)
            .WithAll<factoryProperties_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .Build(this);

        var factoryQueryEntityArray = factoryQuery.ToEntityArray(Allocator.TempJob);
        
        
        Entities
            .WithAll<existingUnitsCounter_component>()
            .WithBurst()
            .WithDisposeOnCompletion(navAgentQueryEntityArray)
            .WithDisposeOnCompletion(factoryQueryEntityArray)
            .ForEach((
                ref existingUnitsCounter_component localExistingUnitsCounterComponent 
            ) =>
            {
                localExistingUnitsCounterComponent.amountOfExistingUnits = navAgentQueryEntityArray.Length;
                localExistingUnitsCounterComponent.amountOfExistingFactories = factoryQueryEntityArray.Length;

            }).ScheduleParallel();

        
        
        //navAgentQueryEntityArray.Dispose();
        //factoryQueryEntityArray.Dispose();
        
    }
}