using Unity.Entities;
using Unity.Burst;
using UnityEngine;

//this system receives the shift button input status and writes it into a component

// was migrated to the sync from gameObject system

[DisableAutoCreation]
[BurstCompile]
public partial class unitSelection_shiftButtonStatus_System : SystemBase
{
       [BurstCompile]
    protected override void OnUpdate()
    {
        Entities
           .WithAll<unitSelection_shiftButtonStatus_component>()
           .WithBurst()
           .ForEach((
              ref unitSelection_shiftButtonStatus_component localShiftStatusComponent
               
            ) =>
           {

               localShiftStatusComponent.Value = Input.GetKey(KeyCode.LeftShift);

           }).Run();
        
    }
}
