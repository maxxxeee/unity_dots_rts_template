using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.AI;

[BurstCompile]
public partial class NavAgent_System : SystemBase
{
    private NavMeshWorld navMeshWorld;
    private bool navMeshQueryAssigned;
    private NavMeshQuery query;
    //private BeginSimulationEntityCommandBufferSystem es_ECB_Parallel;

    protected override void OnCreate()
    {
        //es_ECB_Parallel = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        navMeshQueryAssigned = false;
        navMeshWorld = NavMeshWorld.GetDefaultWorld();
    }

    protected override void OnUpdate()
    {

        if (!navMeshQueryAssigned)
        {
            query = new NavMeshQuery(navMeshWorld, Allocator.Persistent, NavAgent_GlobalSettings.instance.maxPathNodePoolSize);
            navMeshQueryAssigned = true;
        }

        float3 extents = NavAgent_GlobalSettings.instance.extents;
        int maxIterations = NavAgent_GlobalSettings.instance.maxIterations;
        int maxPathSize = NavAgent_GlobalSettings.instance.maxPathSize;
        NavMeshQuery currentQuery = query;
        //var ecbParallel = es_ECB_Parallel.CreateCommandBuffer().AsParallelWriter();
        
        Entities
            //.WithNativeDisableParallelForRestriction(currentQuery)
            //.WithReadOnly(currentQuery)
            .WithAll<NavAgent_ToBeRoutedTag>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<unitConstruction_component>()
            .WithBurst()
            .ForEach((
                //Entity e,
                //int entityInQueryIndex,
                ref NavAgent_Component localNavAgentComponent,
                ref DynamicBuffer<NavAgent_Buffer> nb,
                ref NavAgent_ToBeRoutedTag localToBeRoutedTag
                ) =>
            {
                //return from entity if navAgent has already been routed
                if (!localToBeRoutedTag.Value)
                {
                    return;
                }
                
                //Debug.Log("currenty entity: " + e.Index);
                
                //TODO: reset querry if target has been reached?
                
                PathQueryStatus status = PathQueryStatus.Failure;
                
                localNavAgentComponent.nml_FromLocation = currentQuery.MapLocation(localNavAgentComponent.fromLocation, extents, 0);
                localNavAgentComponent.nml_ToLocation = currentQuery.MapLocation(localNavAgentComponent.toLocation, extents, 0);
                if (currentQuery.IsValid(localNavAgentComponent.nml_FromLocation) && currentQuery.IsValid(localNavAgentComponent.nml_ToLocation))
                {
                    status = currentQuery.BeginFindPath(localNavAgentComponent.nml_FromLocation, localNavAgentComponent.nml_ToLocation, -1);
                }
                if (status == PathQueryStatus.InProgress)
                {
                    status = currentQuery.UpdateFindPath(maxIterations, out int iterationPerformed);
                }
                
                
                
                if (status == PathQueryStatus.Success)
                {
                    status = currentQuery.EndFindPath(out int polygonSize);
                    NativeArray<NavMeshLocation> res = new NativeArray<NavMeshLocation>(polygonSize, Allocator.Temp);
                    NativeArray<StraightPathFlags> straightPathFlag = new NativeArray<StraightPathFlags>(maxPathSize, Allocator.Temp);
                    NativeArray<float> vertexSide = new NativeArray<float>(maxPathSize, Allocator.Temp);
                    NativeArray<PolygonId> polys = new NativeArray<PolygonId>(polygonSize, Allocator.Temp);
                    int straightPathCount = 0;
                    currentQuery.GetPathResult(polys);

                    //Debug.Log("poly size: " + polys.Length);
                    
                    if (polys.Length <= 1)
                    {
                        nb.Add(new NavAgent_Buffer { wayPoints = localNavAgentComponent.toLocation });
                        
                        localNavAgentComponent.routed = true;
                        
                        localToBeRoutedTag.Value = false;
                        
                        return;
                    }
                   
                    
                    
                    
                    status =
                        PathUtils.FindStraightPath(
                        currentQuery,
                        localNavAgentComponent.fromLocation,
                        localNavAgentComponent.toLocation,
                        polys,
                        polygonSize,
                        ref res,
                        ref straightPathFlag,
                        ref vertexSide,
                        ref straightPathCount,
                        maxPathSize
                        );
                    
                    //Debug.Log("Path Querry Status: " + status);
                    
                    if (status == PathQueryStatus.Success)
                    {
                        {
                            for (int i = 0; i < straightPathCount; i++)
                            {
                                nb.Add(new NavAgent_Buffer { wayPoints = res[i].position });
                            }
                            localNavAgentComponent.routed = true;
                        
                            localToBeRoutedTag.Value = false;
                            //ecbParallel.RemoveComponent<NavAgent_ToBeRoutedTag>(entityInQueryIndex, e);
                        }
                    }
                    
                   
                    res.Dispose();
                    straightPathFlag.Dispose();
                    polys.Dispose();
                    vertexSide.Dispose();
                    //currentQuery.Dispose();
                }

                //Debug.Log("path querry status: " + status + " for entity: " + e.Index);
            }).Schedule();
            //}).Schedule();

        navMeshWorld.AddDependency(Dependency);
        //es_ECB_Parallel.AddJobHandleForProducer(Dependency);
    }
    
    protected override void OnDestroy()
    {
        query.Dispose();
    }
    
}