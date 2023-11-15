using Unity.Entities;

[DisableAutoCreation]
public partial class NavAgent_System_PreProcess : SystemBase
{
    //private BeginSimulationEntityCommandBufferSystem bs_ECB;

    protected override void OnCreate()
    {
      //  bs_ECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        //var ecb = bs_ECB.CreateCommandBuffer();
        int maxEntitiesRoutedPerFrame = NavAgent_GlobalSettings.instance.maxEntitiesRoutedPerFrame;
        int entitiesRoutedThisFrame = 0;
        Entities
            .WithAll<NavAgent_ToBeRoutedTag>()
            .WithBurst()
            .ForEach((
                Entity e,
                ref NavAgent_Component nc,
                ref NavAgent_ToBeRoutedTag localToBeRoutedTag
                ) =>
            {
                if (entitiesRoutedThisFrame < maxEntitiesRoutedPerFrame && !nc.routed)
                {
                    //ecb.AddComponent<NavAgent_ToBeRoutedTag>(e);
                    localToBeRoutedTag.Value = true;
                    entitiesRoutedThisFrame++;
                }
            }).Schedule();
        
        //bs_ECB.AddJobHandleForProducer(Dependency);
    }
}
