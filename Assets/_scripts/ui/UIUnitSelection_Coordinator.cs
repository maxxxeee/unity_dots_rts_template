using System.Collections.Generic;
using UnityEngine;


// legacy 
// this script was used to sync gameObject visualizers of selected units 
// it functioned similar to the factory progress and health bars

// it was replaced by selection shader as it was MUCH more performant


public class UIUnitSelection_Coordinator : MonoBehaviour
{
   public GameObject unitSelectionUIPrefab;

   public List<unitSelectionUISyncStruct> selectedUnitsUiSyncStruct = new List<unitSelectionUISyncStruct>();

   public List<GameObject> selectedUnitsUIElementList = new List<GameObject>();
   
   
   
   private void Update()
   {

      updateListOfExistingUIElements();
      matchNeededAmountOfFactoryUiElements();
      updateSelectedUnitUIPosition();
   }


   private void updateListOfExistingUIElements()
   {
      var tempUnitSelectionUIElements = FindObjectsByType<unit_selection_visualizer_logic>(FindObjectsSortMode.InstanceID);

      foreach (var unitUISelectionElementInstance in tempUnitSelectionUIElements)
      {
         if (!selectedUnitsUIElementList.Contains(unitUISelectionElementInstance.gameObject))
         {
            selectedUnitsUIElementList.Add(unitUISelectionElementInstance.gameObject);
         }
      }
      
      
   }

   private void matchNeededAmountOfFactoryUiElements()
   {
      //need to dynamically allocate enough ui elements as there are factories

      int tempAmountOfUnitSelectionUIElements = selectedUnitsUIElementList.Count;

      
      int amountOfUnitsCurrentlySelected = selectedUnitsUiSyncStruct.Count;

      //Debug.Log("amount of ui factory progress elements: " + tempAmountOfFactoryProgressUIElements);
      //Debug.Log("amount of active factories: " + amountOfFactorysCurrentlyActive);
      
      if (amountOfUnitsCurrentlySelected > tempAmountOfUnitSelectionUIElements )
      {
         while (amountOfUnitsCurrentlySelected > tempAmountOfUnitSelectionUIElements)
         {
            var newUnitSelectionUIElement = Instantiate(unitSelectionUIPrefab);
            selectedUnitsUIElementList.Add(newUnitSelectionUIElement);
            tempAmountOfUnitSelectionUIElements = selectedUnitsUIElementList.Count;
         }
      }
      
      if (amountOfUnitsCurrentlySelected < tempAmountOfUnitSelectionUIElements )
      {
         while (amountOfUnitsCurrentlySelected < tempAmountOfUnitSelectionUIElements)
         {
            var unitSelectionUIToRemove = selectedUnitsUIElementList[selectedUnitsUIElementList.Count - 1];
            selectedUnitsUIElementList.Remove(unitSelectionUIToRemove);
            Destroy(unitSelectionUIToRemove);
            tempAmountOfUnitSelectionUIElements = selectedUnitsUIElementList.Count;
         }
      }
   }

   public void updateSelectedUnitUIPosition()
   {
      for (int i = 0; i < selectedUnitsUIElementList.Count; i++)
      {
         unit_selection_visualizer_logic unitSelectionVisualizerLogicInstance;
       
         if(selectedUnitsUIElementList[i] == null)
         {
            selectedUnitsUIElementList.RemoveAt(i);
            continue;
         }

         unitSelectionVisualizerLogicInstance = selectedUnitsUIElementList[i].GetComponent<unit_selection_visualizer_logic>();



         unitSelectionVisualizerLogicInstance.isVisible = true;
         unitSelectionVisualizerLogicInstance.selectedUnitPosition = selectedUnitsUiSyncStruct[i].unitPosition;

      }
   }
   
}

public struct unitSelectionUISyncStruct {
   
   public Vector3 unitPosition;

   //Constructor (not necessary, but helpful)
   public unitSelectionUISyncStruct(Vector3 unitPositionInput) {
      this.unitPosition = unitPositionInput;
   }
}