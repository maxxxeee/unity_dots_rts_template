using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;

// this system adjusts the shader of an unit being hovered over or already selected
// if a unit is neither of the above it will revert the shader

// the system will also apply the above to any children contaning any mesh renderers 

[BurstCompile]
public partial class unit_shader_highlight_system : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        var unitWasSelectedByUserComponentLookup = GetComponentLookup<thisUnitWasSelectedByUser_tag>(true);

        var unitHoverComponentLookup = GetComponentLookup<UIMouseHoveringOverThisEntity_tag>(true);

        var unitSelectionShaderComponentLookup = GetComponentLookup<unitSelectionHighlightShader_component>();

        var getChildrenBuffer = GetBufferLookup<Child>(true);


        Entities
            .WithAll<unitSelectionHighlightShader_component>()
            .WithAll<NavAgent_Component>()
            .WithNone<AboutToBeDestroyed_Tag>()
            .WithNone<combatDisabled_tag>()
            .WithBurst()
            .WithReadOnly(getChildrenBuffer)
            .WithReadOnly(unitWasSelectedByUserComponentLookup)
            .WithReadOnly(unitHoverComponentLookup)
            .WithDisposeOnCompletion(unitSelectionShaderComponentLookup)
            .ForEach((
                Entity localEntity
            ) =>
            {
                if (unitHoverComponentLookup.HasComponent(localEntity) ||
                    unitWasSelectedByUserComponentLookup.HasComponent(localEntity))
                {
                    unitSelectionShaderComponentLookup[localEntity] = new unitSelectionHighlightShader_component
                    {
                        Value = 0.1f
                    };
                    
                    //set the same shader value for every child of this entity
                    
                    foreach (var childInstance in getChildrenBuffer[localEntity])
                    {
                        if (unitSelectionShaderComponentLookup.HasComponent(childInstance.Value))
                        {
                            unitSelectionShaderComponentLookup[childInstance.Value] =
                                new unitSelectionHighlightShader_component
                                {
                                    Value = 0.1f
                                };
                        }

                        if (getChildrenBuffer.HasBuffer(childInstance.Value))
                        {
                            foreach (var childInstanceNested in getChildrenBuffer[childInstance.Value])
                            {
                                if (unitSelectionShaderComponentLookup.HasComponent(childInstanceNested.Value))
                                {
                                    unitSelectionShaderComponentLookup[childInstanceNested.Value] =
                                        new unitSelectionHighlightShader_component
                                        {
                                            Value = 0.1f
                                        };
                                }
                            }
                        }
                    }
                    
                }
                    
                else
                {
                    unitSelectionShaderComponentLookup[localEntity] = new unitSelectionHighlightShader_component
                    {
                        Value = 5000.0f
                    };

                    if (getChildrenBuffer.HasBuffer(localEntity))
                    {
                        foreach (var childInstance in getChildrenBuffer[localEntity])
                        {
                            if (unitSelectionShaderComponentLookup.HasComponent(childInstance.Value))
                            {

                                unitSelectionShaderComponentLookup[childInstance.Value] =
                                    new unitSelectionHighlightShader_component
                                    {
                                        Value = 5000.0f
                                    };
                            }

                            if (getChildrenBuffer.HasBuffer(childInstance.Value))
                            {
                                foreach (var childInstanceNested in getChildrenBuffer[childInstance.Value])
                                {
                                    if (unitSelectionShaderComponentLookup.HasComponent(childInstanceNested.Value))
                                    {
                                        unitSelectionShaderComponentLookup[childInstanceNested.Value] =
                                            new unitSelectionHighlightShader_component
                                            {
                                                Value = 5000.0f
                                            };
                                    }
                                
                               
                                }
                            }
                        
                        }
                    }
                    
                  
                }

            }).Schedule();

    }
}