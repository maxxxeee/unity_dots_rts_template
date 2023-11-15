using UnityEngine;
using Unity.Entities;

// authoring to sync user input stored inside of a gameObject to a system in another scene

// it adds the unitAmountInput_component specifying the amount of units given by the user


public class unitAmountInputSyncAuthoring : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}


public class unitAmountInputSyncAuthoring_Baker : Baker<unitAmountInputSyncAuthoring>
{
 
      public override void Bake(unitAmountInputSyncAuthoring authoring)
       {

           var entity = GetEntity(TransformUsageFlags.None);
              
              
              AddComponent<unitAmountInput_component>(entity);
       } 
}