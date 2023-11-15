using UnityEngine;
using Unity.Entities;

// this system syncs the build list of a factory with its counterpart user interface gameGameobject

public partial class factoryGameObjectBuildQuque : SystemBase
{
    protected override void OnUpdate()
    {

        var allFactoryAuthoringGameObjects = GameObject.FindObjectsByType<FactoryAuthoring>(FindObjectsSortMode.InstanceID);

       CompleteDependency();
       
       Entities
           .WithoutBurst()
           .WithAll<factoryBuildQuque>()
           .ForEach((
               ref factoryBuildQuque localFactoryBuildQuque,
               in factoryProperties_component localFactoryPropertiesComponent
           ) =>
           {

               foreach (var factoryAuthoringInstance in allFactoryAuthoringGameObjects)
               {
                   if (factoryAuthoringInstance.GetInstanceID() == localFactoryPropertiesComponent.gameObjectIndex)
                   {
                     
                       foreach (var buildOptionsIndex in factoryAuthoringInstance.buildQuque)
                       {
                           localFactoryBuildQuque.buildQuque.Add(buildOptionsIndex);
                           factoryAuthoringInstance.buildQuque.Remove(buildOptionsIndex);
                       }
                       
                   }
               }

           }).Run();

    }
}
