using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

//this system rotates the unit's turret towards it's current target
// if there is currently no target, the turret will be moved to its default rotation 


[BurstCompile]
public partial class lookTowardsTarget_System : SystemBase
{
    
    [BurstCompile]
    //rotates the units turret towards the current target
    protected override void OnUpdate()
    {
        ComponentLookup<LocalToWorld> localToWorldFromEntity = GetComponentLookup<LocalToWorld>(true);
        ComponentLookup<NavAgent_Component> navAgentComponentLookup = GetComponentLookup<NavAgent_Component>(true);

        ComponentLookup<targetOverride_component> targetOverriderComponentLookup =
            GetComponentLookup<targetOverride_component>(true);

          Entities
            .WithBurst()
            .WithAll<combat_component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<unitConstruction_component>()
            .WithReadOnly(localToWorldFromEntity)
            .WithReadOnly(navAgentComponentLookup)
            .WithReadOnly(targetOverriderComponentLookup)
            .ForEach((
                Entity localEntity,
                ref LocalTransform localTransform,
                in LocalToWorld localToWorld,
                in Parent localParent,
                in currentTargetComponent localCurrentTargetComponent,
                in hasTargetTag localHasTargetTag,
                in combat_component localCombatComponent
            ) =>
            {
                var currentTarget = localCurrentTargetComponent.currentTarget;


                if (targetOverriderComponentLookup.HasComponent(localEntity))
                {
                    //aim at overriden target if close enough
                    if (targetOverriderComponentLookup[localEntity].closeEnoughToAttack)
                    {
                        currentTarget = targetOverriderComponentLookup[localEntity].targetOverride;
                    }
                }
                
                
                if (currentTarget.Index < 0 || currentTarget.Equals(Entity.Null) || !localHasTargetTag.Value || !navAgentComponentLookup.HasComponent(localCurrentTargetComponent.currentTarget))
                {
                    if (!localTransform.Rotation.Equals(quaternion.Euler(float3.zero)))
                    {
                        //localTransform.Rotation = quaternion.LookRotation(localToWorldFromEntity[localParent.Value].Forward,float3.zero);
                        localTransform.Position = localCombatComponent.originalPosition;
                        localTransform.Rotation = localCombatComponent.originalRotation;
                    }
                    return;
                }

                float3 targetPositionOffset = new float3();

                targetPositionOffset.y = 0.5f;

                if (localToWorldFromEntity.HasComponent(localParent.Value))
                {
                    if (navAgentComponentLookup.HasComponent(currentTarget))
                    {
                        LocalToWorld parentLocalToWorld = localToWorldFromEntity[localParent.Value];
                            
                        LocalToWorld targetLocalToWorld = localToWorldFromEntity[currentTarget];

                        float3 tempTargetPositionInput = targetLocalToWorld.Position + targetPositionOffset;

                        localTransform.Rotation = math.mul(math.inverse(parentLocalToWorld.Rotation), HelperFunctions_system.lookTowardsDestinationSystem(tempTargetPositionInput, localToWorld.Position));
                    }
                }

            }).ScheduleParallel();

    }
}