using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Rendering;

//this system sets the base color of a unit still being constructed
// does not seem to have any effect since using a custom selection shader


[BurstCompile]
public partial class setUnitConstructionColor_System : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        ComponentLookup<unitConstruction_component> unitConstructionComponentLookup =
            GetComponentLookup<unitConstruction_component>();

        ComponentLookup<combatDisabled_tag> combatDisabledTagLookup =
            GetComponentLookup<combatDisabled_tag>();
        
        Entities
            .WithBurst()
            .WithAll<URPMaterialPropertyBaseColor>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithReadOnly(unitConstructionComponentLookup)
            .WithReadOnly(combatDisabledTagLookup)
            .ForEach((
                Entity localEntity,
                ref URPMaterialPropertyBaseColor localURPBaseColorComponent
            ) =>
            {
                if (unitConstructionComponentLookup.HasComponent(localEntity) || combatDisabledTagLookup.HasComponent(localEntity))
                {
                    localURPBaseColorComponent.Value = new float4(0.0f, 0.0f, 0.0f, 0.0f);
                }
                else
                {
                    localURPBaseColorComponent.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }).ScheduleParallel();
    }
}
