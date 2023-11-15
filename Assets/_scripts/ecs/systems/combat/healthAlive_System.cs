using Unity.Entities;
using Unity.Burst;


//this system checks if an entity with a attached health component still has above 0 hit points
// if not it will attach a AboutToBeDestroyed_Tag to the entity

[BurstCompile]
public partial class healthAlive_System : SystemBase
{
   
   [BurstCompile]
   protected override void OnUpdate()
   {

      ComponentType Type_aboutToBeDestroyed = typeof(AboutToBeDestroyed_Tag);
      
      Entities
         .WithAll<health_component>()
         .WithNone<AboutToBeDestroyed_Tag>()
         .WithNone<unitConstruction_component>()
         .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
         .WithBurst()
         .ForEach((
            Entity localEntity,
            EntityCommandBuffer commandBuffer,
            in health_component local_healtComponent
         ) =>
         {

            if (local_healtComponent.Value <= 0)
            {

               commandBuffer.AddComponent(localEntity, Type_aboutToBeDestroyed);
            }
                
         }).ScheduleParallel();

   }
}
