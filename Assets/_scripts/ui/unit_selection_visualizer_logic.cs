using UnityEngine;


//legacy
// script to update individual selection visualizers
// was replaced by a selection shader

public class unit_selection_visualizer_logic : MonoBehaviour
{
   public bool isVisible;

   public Vector3 selectedUnitPosition;

   public MeshRenderer[] localMeshRenders;

   private void Awake()
   {
       localMeshRenders = GetComponentsInChildren<MeshRenderer>();
   }

   private void Update()
   {
       if (isVisible)
       {
           
           //set position from given entity
           transform.position = selectedUnitPosition + new Vector3(0.0f, 0.1f, 0.0f);
           
           
           //rotate the selection object
           var current_Rotation = transform.rotation.eulerAngles;
           current_Rotation += new Vector3(0.0f, 0.11f, 0.0f);
           
           transform.rotation = Quaternion.Euler(current_Rotation);
       }

       //set mesh renderes visiblity according to given visiblity
       foreach (var meshRendrererInstance in localMeshRenders)
       {
           meshRendrererInstance.gameObject.SetActive(isVisible);
       }
       
   }
}
