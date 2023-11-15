using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;


//this system writes the position of the unit being hovered over into a ecs data component to be synced to a gameObject in another system

[BurstCompile]
public partial class UI_write_taged_units_position_to_component_system : SystemBase
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

    [BurstCompile]
    protected override void OnUpdate()
    {

        ComponentLookup<LocalToWorld> LocalToWorldFromEntityLookup = GetComponentLookup<LocalToWorld>();

        Entities
            .WithBurst()
            .WithDisposeOnCompletion(LocalToWorldFromEntityLookup)
            .WithAll<UIUnitHealthBarSync_component>()
            .ForEach((
                ref UIUnitHealthBarSync_component localUIUnitHealthBarSyncComponent
            ) =>
            {
                if (localUIUnitHealthBarSyncComponent.detected_Entity.Equals(Entity.Null) ||
                    localUIUnitHealthBarSyncComponent.detected_Entity.Index < 0)
                {
                    return;
                }
                
                var localToWorldFromEntity =
                    LocalToWorldFromEntityLookup.GetRefRO(localUIUnitHealthBarSyncComponent.detected_Entity);

                localUIUnitHealthBarSyncComponent.syncPosition = localToWorldFromEntity.ValueRO.Position;

            }).Schedule();
            
    }
}
