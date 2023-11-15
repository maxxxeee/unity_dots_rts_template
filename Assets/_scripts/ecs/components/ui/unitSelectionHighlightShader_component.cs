using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("highlightStrength")]
public struct unitSelectionHighlightShader_component : IComponentData
{
   public float Value;
}
