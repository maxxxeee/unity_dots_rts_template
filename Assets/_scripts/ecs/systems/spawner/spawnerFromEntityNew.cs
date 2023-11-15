using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// this system acts as the spawner in the benchmark / mass spawn scenes
// the system will work on any entities holding a Spawner_FromEntity logic
//  it will then spawn as many units as requested in a grid

// additionally it will check if any user input was given by looking for an entity holding that data
// this userInput is done in a seperate scene and is meant to be used for a build of the benchmark scene

// this system also adheres to a limit of how many units can be spawned in a single update / frame
//  if this threshold is reached it will saved the amount of units spawned so far to the spawner component
//      and will continue next frame
//  this is supposed to mitigate the very high memory requirements when spawning all units at once


public partial struct spawnerFromEntityNew : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This makes the system not update unless at least one entity exists that has the Spawner component.
        state.RequireForUpdate<Spawner_FromEntity>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        var foundSpawnerInstances = SystemAPI.QueryBuilder().WithAll<Spawner_FromEntity>().Build().ToEntityArray(Allocator.Temp);

        bool userInputEnabled = false;

        foreach (var singleSpawnerInstance in foundSpawnerInstances)
        {
            var tempUserInputEnalbedValue =
                SystemAPI.GetComponent<Spawner_FromEntity>(singleSpawnerInstance).userInputEnabled;
            
            if (tempUserInputEnalbedValue)
            {
                userInputEnabled = tempUserInputEnalbedValue;
            }
        }
        
        
        var foundAmountInputInstances = SystemAPI.QueryBuilder().WithAll<unitAmountInput_component>().Build().ToEntityArray(Allocator.Temp);

        if (userInputEnabled)
        {
            if (foundAmountInputInstances.Length > 0 && !SystemAPI.GetComponent<unitAmountInput_component>(foundAmountInputInstances[0]).hasBeenSetYet)
            {
                return;
            }
        }
          
        
        
        foreach (var singleSpawnerInstance in foundSpawnerInstances)
        {
            
            if (!SystemAPI.GetComponent<Spawner_FromEntity>(singleSpawnerInstance).hasSpawendUnitsAlready)
            {
                var localSpawnerFromEntity = SystemAPI.GetComponent<Spawner_FromEntity>(singleSpawnerInstance);
                
                var prefab = localSpawnerFromEntity.Prefab;
                
                var spawnerLocalTransform = SystemAPI.GetComponent<LocalTransform>(singleSpawnerInstance);
                
                if (foundAmountInputInstances.Length > 0)
                {
                    var localUnitAmountInputComponent =
                        SystemAPI.GetComponent<unitAmountInput_component>(foundAmountInputInstances[0]);
                    
                    localSpawnerFromEntity.CountX = localUnitAmountInputComponent.inputCountX;
                    localSpawnerFromEntity.CountY = localUnitAmountInputComponent.inputCountY;

                    localSpawnerFromEntity.requestedAmount = localUnitAmountInputComponent.requestedAmount;
                }
                else
                {
                    var axisLength = (int)math.sqrt(localSpawnerFromEntity.requestedAmount);
                    
                    localSpawnerFromEntity.CountX = axisLength;
                    localSpawnerFromEntity.CountY = axisLength;
                }
                
                
                //take values from data component to be able to continue spawning from previous frames
                int x = localSpawnerFromEntity.xSpawnGrid;
                int z = localSpawnerFromEntity.zSpawnGrid;

                int amountOfUnitsSpawnedThisFrame = 0;
                int unitAmountSpawnedSoFar = localSpawnerFromEntity.spawnedUnitsSoFar;
                
                for (var amount = 0; amount < localSpawnerFromEntity.requestedAmount; amount++)
                {
                    var tempNewUnit = state.EntityManager.Instantiate(prefab);

                    var transform = SystemAPI.GetComponentRW<LocalTransform>(tempNewUnit);
                        
                    transform.ValueRW.Position = new float3(x * 3.3f * localSpawnerFromEntity.xCorrection, 0.0f, z * 3.3f * localSpawnerFromEntity.zCorrection) + spawnerLocalTransform.Position;
                    
                    
                    z++;
                    amountOfUnitsSpawnedThisFrame++;
                    unitAmountSpawnedSoFar++;

                    if (z == localSpawnerFromEntity.CountY)
                    {
                        x++;
                        z = 0;
                    }

                    //get out of the loop if the amount of overall spawned units equals the requested amount
                    if (unitAmountSpawnedSoFar == localSpawnerFromEntity.requestedAmount)
                    {
                        localSpawnerFromEntity.spawnedUnitsSoFar = unitAmountSpawnedSoFar;
                        
                        break;
                    }

                    //get out of loop if amount of spawned units this frame has reached its preset limit
                    if (amountOfUnitsSpawnedThisFrame == localSpawnerFromEntity.maxAmountOfUnitsToSpawnPerFrame)
                    {
                        localSpawnerFromEntity.xSpawnGrid = x;
                        localSpawnerFromEntity.zSpawnGrid = z;

                        localSpawnerFromEntity.spawnedUnitsSoFar = unitAmountSpawnedSoFar;
                        
                        break;
                    }
                }
                

               if (localSpawnerFromEntity.spawnedUnitsSoFar == localSpawnerFromEntity.requestedAmount)
               {
                  localSpawnerFromEntity.hasSpawendUnitsAlready = true;
               }
               
               SystemAPI.SetComponent(singleSpawnerInstance, localSpawnerFromEntity);  

               
            }
            
        }

        foundSpawnerInstances.Dispose();
        foundAmountInputInstances.Dispose();
    }
}
