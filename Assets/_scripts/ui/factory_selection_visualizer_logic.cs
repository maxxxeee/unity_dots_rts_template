using UnityEngine;

//this script controls the status of a progress bar displaying the building progress of a unit

// it receives its data externaly from a Coordinator

public class factory_selection_visualizer_logic : MonoBehaviour
{
   public bool isVisible;

   public Vector3 selectedFactoryPosition;

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
           transform.position = selectedFactoryPosition + new Vector3(0.0f, 0.1f, 0.0f);
           
           
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
