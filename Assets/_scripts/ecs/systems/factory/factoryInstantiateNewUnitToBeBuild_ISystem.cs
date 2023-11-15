using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

//this system instantiates units which are finished being constructed by a factory
// the instantiation was moved to an ISystem as instantiation inside of SystemBase is currently broken

[BurstCompile]
public partial struct factoryInstantiateNewUnitToBeBuild : ISystem
{
   
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This makes the system not update unless at least one entity exists that has the factoryProperties component.
        state.RequireForUpdate<factoryProperties_component>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        var foundFactoryInstances = SystemAPI.QueryBuilder().WithAll<factoryProperties_component>().Build().ToEntityArray(Allocator.Temp);

        foreach (var singleFactoryInstance in foundFactoryInstances)
        {
            if (SystemAPI.GetComponent<factoryProperties_component>(singleFactoryInstance).instantiationNeeded)
            {

                var localFactoryInstancePropertiesComponent = SystemAPI.GetComponent<factoryProperties_component>(singleFactoryInstance);
                
                var prefab = localFactoryInstancePropertiesComponent.currentlyBuilding;
                
                var factoryLocalTransform = SystemAPI.GetComponent<LocalToWorld>(singleFactoryInstance);
                
                var tempUnit = state.EntityManager.Instantiate(prefab);
                
                
                var transform = SystemAPI.GetComponentRW<LocalTransform>(tempUnit);

                SystemAPI.GetComponentRW<unitConstruction_component>(tempUnit).ValueRW.buildTimeLeft = localFactoryInstancePropertiesComponent.buildTime;
                
                
                transform.ValueRW.Position = factoryLocalTransform.Position + localFactoryInstancePropertiesComponent.newUnitPositionOffset;


                localFactoryInstancePropertiesComponent.currentlyBuilding = tempUnit;
                localFactoryInstancePropertiesComponent.instantiationNeeded = false;
                
                SystemAPI.SetComponent(singleFactoryInstance, localFactoryInstancePropertiesComponent);
            }
            
        }

        foundFactoryInstances.Dispose();
    }
}
