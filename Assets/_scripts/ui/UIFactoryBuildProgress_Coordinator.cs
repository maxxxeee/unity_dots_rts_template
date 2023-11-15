using System.Collections.Generic;
using UnityEngine;

//this script coordinates the location and status of all currently selected factories progression bars

// it receivers it data from a dots system 
//  then it keeps the amount of ui elements in sync with the amount of selected factories
//  finally it updates the location and status of the progress bars with the given data

// each progress bar will only receive the data to their public values, they will update their location and progress bar themselfs



public class UIFactoryBuildProgress_Coordinator : MonoBehaviour
{
   public GameObject factoryUIStatusPrefab;

   public List<factoryBuildProgressUISyncStruct> factoryBuildProgressStructList = new List<factoryBuildProgressUISyncStruct>();

   public List<GameObject> factoryUiProgressList = new List<GameObject>();
   
   
   private void Update()
   {

      updateListOfExistingFactoryUIElements();
      matchNeededAmountOfFactoryUiElements();
      updateProgressAccordingToGivenData();
   }


   private void updateListOfExistingFactoryUIElements()
   {
      var tempFactoryUIElements = FindObjectsByType<UIPositionSyncWithFactory>(FindObjectsSortMode.InstanceID);

      foreach (var factoryUIElementInstance in tempFactoryUIElements)
      {
         if (!factoryUiProgressList.Contains(factoryUIElementInstance.gameObject))
         {
            factoryUiProgressList.Add(factoryUIElementInstance.gameObject);
         }
      }
      
      
   }

   private void matchNeededAmountOfFactoryUiElements()
   {

      int tempAmountOfFactoryProgressUIElements = factoryUiProgressList.Count;

      
      int amountOfFactorysCurrentlyActive = factoryBuildProgressStructList.Count;
      
      
      if (amountOfFactorysCurrentlyActive > tempAmountOfFactoryProgressUIElements )
      {
         while (amountOfFactorysCurrentlyActive > tempAmountOfFactoryProgressUIElements)
         {
            var newFactoryProgressUIElement = Instantiate(factoryUIStatusPrefab);
            factoryUiProgressList.Add(newFactoryProgressUIElement);
            tempAmountOfFactoryProgressUIElements = factoryUiProgressList.Count;
         }
      }
      
      if (amountOfFactorysCurrentlyActive < tempAmountOfFactoryProgressUIElements )
      {
         while (amountOfFactorysCurrentlyActive < tempAmountOfFactoryProgressUIElements)
         {
            var UIFactoryProgressElementToRemove = factoryUiProgressList[factoryUiProgressList.Count - 1];
            factoryUiProgressList.Remove(UIFactoryProgressElementToRemove);
            Destroy(UIFactoryProgressElementToRemove);
            tempAmountOfFactoryProgressUIElements = factoryUiProgressList.Count;
         }
      }
   }

   private void updateProgressAccordingToGivenData()
   {
      for (int i = 0; i < factoryUiProgressList.Count; i++)
      {
         var uiFactoryPositionSyncComponentInstance =
            factoryUiProgressList[i].GetComponent<UIPositionSyncWithFactory>();
         
         
         if (uiFactoryPositionSyncComponentInstance == null)
         {
            factoryUiProgressList.RemoveAt(i);
            continue;
         }
         
         if (factoryBuildProgressStructList[i].factoryCurrentlyBuilding)
         {

            uiFactoryPositionSyncComponentInstance.buildQuqueSize =
               factoryBuildProgressStructList[i].factoryBuildQuqueSize;
            

            uiFactoryPositionSyncComponentInstance.currentProgress =
               factoryBuildProgressStructList[i].factoryBuildProgress;
            
            uiFactoryPositionSyncComponentInstance.targetPosition = factoryBuildProgressStructList[i].factoryPosition;
            
            uiFactoryPositionSyncComponentInstance.isVisible = true;
            
         }
         else
         {
            uiFactoryPositionSyncComponentInstance.isVisible = false;
         }
      }
   }
}


public struct factoryBuildProgressUISyncStruct {
   
   public bool factoryCurrentlyBuilding;
   public Vector3 factoryPosition;
   public int entityIndex;
   public float factoryBuildProgress;
   public int factoryBuildQuqueSize;
   
   public factoryBuildProgressUISyncStruct(bool factoryCurrentlyBuildingInput, Vector3 factoryPositionInput, int entityIndexInput, float factoryBuildProgressInput, int inputFactoryBuildQuqueSize) {
      this.factoryCurrentlyBuilding = factoryCurrentlyBuildingInput;
      this.factoryPosition = factoryPositionInput;
      this.entityIndex = entityIndexInput;
      this.factoryBuildProgress = factoryBuildProgressInput;
      this.factoryBuildQuqueSize = inputFactoryBuildQuqueSize;
   }
}

