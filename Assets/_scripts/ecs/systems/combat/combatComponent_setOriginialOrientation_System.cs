using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

//this system saves the original orientation of the turret inside of the combat_component

[BurstCompile]
public partial class combatComponent_setOriginialOrientation_System : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities
            .WithBurst()
            .WithAll<combat_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<unitConstruction_component>()
            .ForEach((
                ref combat_component localCombatComponent,
                in LocalTransform localTransform
            ) =>
            {

                if (localCombatComponent.originalOrientationHasBeenSetYet)
                {
                    return;
                }

                localCombatComponent.originalPosition = localTransform.Position;
                localCombatComponent.originalRotation = localTransform.Rotation;

                localCombatComponent.originalOrientationHasBeenSetYet = true;

            }).ScheduleParallel();

    }
}
