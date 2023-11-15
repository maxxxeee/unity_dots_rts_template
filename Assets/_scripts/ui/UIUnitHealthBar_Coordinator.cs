using System.Collections.Generic;
using UnityEngine;

//this script creates and keeps track of health bars for selected units

// it receivers it data from a dots system 
//  then it keeps the amount of ui elements in sync with the amount of selected units
//  finally it updates the location and status of the health bars with the given data

// each health will only receive the data to their public values, they will update their location and health bar themselfs



public class UIUnitHealthbar_Coordinator : MonoBehaviour
{
   public GameObject unitSelectionUIPrefab;
   
   public GameObject unitSelectionUIPrefabConsolidated;
   

   public List<unitHealthBarUISyncStruct> healthBarUiSyncStruct = new List<unitHealthBarUISyncStruct>();

   public List<GameObject> healthBarUIElementList = new List<GameObject>();
   
   public float cameraMaxHeightToRenderUI = 200.0f;

   public int maxAmountOfSelectedUnitsBeforeHealthBarConsolidation = 100;
   
   
   private bool cameraAboveMaxHeight = false;

   private Camera mainCamera;

   private bool consolidationActive = false;
   
   private GameObject consolidatedHealthBar;
   private UIHealthBar_PositionSyncWithSelectedEntity consolidateHealthBarLogic;

   private float consolidatedAverageHealthAmount;
   
   private float consolidatedAverageMaxHealthAmount;
   
   private void Start()
   {
      mainCamera = Camera.main;
      
     
   }

   private void Update()
   {
      checkIfCameraIsAboveMaxHeight();
      checkIfConsolidationShouldBeActive();
      
      updateListOfExistinghealthBarUIElements();
      matchNeededAmountOfHealthBarUiElements();
      updateHealthBarUIPosition();
      
      consolidateHealthBars();
      
   }
   

   /// <summary>
   /// consolidate health bars if a certain amount of units gets selected
   /// </summary>
   
   private void consolidateHealthBars()
   {

     

      if (consolidationActive && !cameraAboveMaxHeight)
      {
         
         if (consolidatedHealthBar == null)
         {
            consolidatedHealthBar = Instantiate(unitSelectionUIPrefabConsolidated);
            consolidateHealthBarLogic = consolidatedHealthBar.GetComponent<UIHealthBar_PositionSyncWithSelectedEntity>();

            consolidatedHealthBar.name = "consoldiated_unit_healthbar";

         }
         
         Vector3 posOne;
         Vector3 posTwo;

         posOne = healthBarUiSyncStruct[0].unitPosition;
         posTwo = healthBarUiSyncStruct[1].unitPosition;
         
         float tempAllHealthValuesAdded = 0.0f;
         float tempAllMaxHealthValues = 0.0f;

         var currentMaxDistance = Vector3.Distance(posOne, posTwo);
         
         // gather the middle point of all selected units
         
         foreach (var unitInformationInstance in healthBarUiSyncStruct)
         {
            if (Vector3.Distance(posOne, unitInformationInstance.unitPosition) > currentMaxDistance)
            {
               posTwo = unitInformationInstance.unitPosition;
               currentMaxDistance = Vector3.Distance(posOne, posTwo);
            }
         }
         
         currentMaxDistance = Vector3.Distance(posOne, posTwo);
         
         foreach (var unitInformationInstance in healthBarUiSyncStruct)
         {
            if (Vector3.Distance(posTwo, unitInformationInstance.unitPosition) > currentMaxDistance)
            {
               posOne = unitInformationInstance.unitPosition;
               currentMaxDistance = Vector3.Distance(posOne, posTwo);
            }

         }

        
         consolidateHealthBarLogic.targetPosition = posOne + (posTwo - posOne) / 2;
         
         
         
         foreach (var unitInformationInstance in healthBarUiSyncStruct)
         {
            tempAllHealthValuesAdded += unitInformationInstance.currentHealthValue;

            tempAllMaxHealthValues += unitInformationInstance.maxHealthValue;
         }

         //put healthbar under the middle point of all selected units and display their collective average health
         
         
         consolidatedAverageMaxHealthAmount = tempAllMaxHealthValues / healthBarUiSyncStruct.Count;
         
         consolidateHealthBarLogic.maxHealthValue = consolidatedAverageMaxHealthAmount;
         
         
         
         consolidatedAverageHealthAmount = tempAllHealthValuesAdded / healthBarUiSyncStruct.Count;

         consolidateHealthBarLogic.currentHealthValue = consolidatedAverageHealthAmount;


         consolidateHealthBarLogic.isVisible = true;

      }
      
      else
      {
         if (consolidatedHealthBar != null)
         {
            consolidateHealthBarLogic.isVisible = false;
            Destroy(consolidatedHealthBar);
         }
         
      }
      
   }
   
   /// <summary>
   /// camera height check to hide healthbars at certain threshold
   /// </summary>
   
   
   private void checkIfCameraIsAboveMaxHeight()
   {
      if (mainCamera.transform.position.y > cameraMaxHeightToRenderUI)
      {
         cameraAboveMaxHeight = true;
      }
      else
      {
         cameraAboveMaxHeight = false;
      }
   }

   private void checkIfConsolidationShouldBeActive()
   {
      if (healthBarUiSyncStruct.Count > maxAmountOfSelectedUnitsBeforeHealthBarConsolidation)
      {
         consolidationActive = true;
      }
      else
      {
         consolidationActive = false;
      }
   }
   
   
   /// <summary>
   ///  healthbar logic for each unit
   /// </summary>
   
   

   private void updateListOfExistinghealthBarUIElements()
   {
      if (consolidationActive)
      {
         return;
      }
      
      var tempHealthBarUIElements = FindObjectsByType<UIHealthBar_PositionSyncWithSelectedEntity>(FindObjectsSortMode.InstanceID);

      foreach (var healtBarUIElementInstance in tempHealthBarUIElements)
      {
         if (consolidatedHealthBar == healtBarUIElementInstance.gameObject)
         {
            continue;
         }
         
         if (!healthBarUIElementList.Contains(healtBarUIElementInstance.gameObject))
         {
            healthBarUIElementList.Add(healtBarUIElementInstance.gameObject);
         }
      }
      
      
   }

   private void matchNeededAmountOfHealthBarUiElements()
   {
      //need to dynamically allocate enough ui elements as there are factories

      int tempAmountOfUnitSelectionUIElements = healthBarUIElementList.Count;

      
      int amountOfUnitsCurrentlySelected = healthBarUiSyncStruct.Count;


      if (consolidationActive)
      {
         amountOfUnitsCurrentlySelected = 0;
      }

      if (amountOfUnitsCurrentlySelected > tempAmountOfUnitSelectionUIElements )
      {
         while (amountOfUnitsCurrentlySelected > tempAmountOfUnitSelectionUIElements)
         {
            var newUnitSelectionUIElement = Instantiate(unitSelectionUIPrefab);
            healthBarUIElementList.Add(newUnitSelectionUIElement);
            tempAmountOfUnitSelectionUIElements = healthBarUIElementList.Count;
         }
      }
      
      if (amountOfUnitsCurrentlySelected < tempAmountOfUnitSelectionUIElements )
      {
         while (amountOfUnitsCurrentlySelected < tempAmountOfUnitSelectionUIElements)
         {
            var unitSelectionUIToRemove = healthBarUIElementList[healthBarUIElementList.Count - 1];
            healthBarUIElementList.Remove(unitSelectionUIToRemove);
            Destroy(unitSelectionUIToRemove);
            tempAmountOfUnitSelectionUIElements = healthBarUIElementList.Count;
         }
      }
   }

   
   public void updateHealthBarUIPosition()
   {
      if (consolidationActive)
      {
         return;
      }
      
      for (int i = 0; i < healthBarUIElementList.Count; i++)
      {
         var healtBarVisualizerLogicInstance =
            healthBarUIElementList[i].GetComponent<UIHealthBar_PositionSyncWithSelectedEntity>();
         
         
         if (healtBarVisualizerLogicInstance == null)
         {
            healthBarUIElementList.RemoveAt(i);
            continue;
         }


         if (cameraAboveMaxHeight)
         {
            healtBarVisualizerLogicInstance.isVisible = false;
         }
         else
         {
            healtBarVisualizerLogicInstance.isVisible = true;
            healtBarVisualizerLogicInstance.targetPosition = healthBarUiSyncStruct[i].unitPosition;
            healtBarVisualizerLogicInstance.maxHealthValue = healthBarUiSyncStruct[i].maxHealthValue;
            healtBarVisualizerLogicInstance.currentHealthValue = healthBarUiSyncStruct[i].currentHealthValue;
         }
      }
   }
   
}

public struct unitHealthBarUISyncStruct {
   
   public Vector3 unitPosition;
   public float currentHealthValue;
   public float maxHealthValue;

   //Constructor (not necessary, but helpful)
   public unitHealthBarUISyncStruct(Vector3 unitPositionInput, float currentHealthValueInput, float maxHealthValueInput) {
      this.unitPosition = unitPositionInput;
      this.currentHealthValue = currentHealthValueInput;
      this.maxHealthValue = maxHealthValueInput;
   }
}