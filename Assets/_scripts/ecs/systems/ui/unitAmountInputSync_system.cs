using Unity.Entities;
using UnityEngine;

//this system syncs information from a gameobject holding the user input given earlier and puts it into a ecs data component

public partial class unitAmountInputSync_system : SystemBase
{
     protected override void OnUpdate()
    {
        var menuInputLogicGameObject = GameObject.FindFirstObjectByType<menuInputLogic>();
        
        Entities
            .WithoutBurst()
            .WithAll<unitAmountInput_component>()
            .ForEach((
                ref unitAmountInput_component localUnitAmountInputComponent
            ) =>
            {
                if (menuInputLogicGameObject == null)
                {
                    return;
                }
                
                localUnitAmountInputComponent.inputCountX = menuInputLogicGameObject.amountOfUnitsToSpawnPerAxis;
                localUnitAmountInputComponent.inputCountY = menuInputLogicGameObject.amountOfUnitsToSpawnPerAxis;

                localUnitAmountInputComponent.requestedAmount = (menuInputLogicGameObject.amountOfUnitsToSpawn / 2);
                
                localUnitAmountInputComponent.hasBeenSetYet = true;

            }).Run();

    }
}
