using Unity.Entities;
using UnityEngine;


// this selection shader authoring simply adds the corresponding component to be able to manipulate the applied shader

public class SelectionShaderAuthoring : MonoBehaviour
{
}

public class SelectionShaderAuthoring_Baker : Baker<SelectionShaderAuthoring>
{
       public override void Bake(SelectionShaderAuthoring authoring)
       {
         
              var entity = GetEntity(TransformUsageFlags.None);
              AddComponent<unitSelectionHighlightShader_component>(entity);

       } 
       
}