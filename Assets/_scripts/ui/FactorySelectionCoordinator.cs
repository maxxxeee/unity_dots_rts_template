using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

//this script coordinates the location and status of all currently selected factories visualizers

//  it checks for if a factory was selected by a left click
//  then it keeps the amount of ui elements in sync with the amount of selected factories
//  finally it updates the location of the visualizer with selected factories location

// each visualizer will only receive the data to their public values, they will update their location themselfs

public class FactorySelectionCoordinator : MonoBehaviour
{
    public List<FactoryAuthoring> currentlySelectedFactories;

    private bool atLeastOneFactoryIsCurrentlySelected;

    public GameObject selectedFactoryUIPrefab;

    public List<GameObject> selectedFactoryVisualizerUI;

    public GameObject factoryProductionUIElement;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (factoryProductionUIElement == null)
        {
            factoryProductionUIElement = GameObject.FindGameObjectWithTag("factoryProductionUI");
        }

        currentlySelectedFactories = new List<FactoryAuthoring>();
        currentlySelectedFactories.Clear();

        selectedFactoryVisualizerUI = new List<GameObject>();
        selectedFactoryVisualizerUI.Clear();

    }

    private void FixedUpdate()
    {
        checkIfFactoryWasSelected();
        syncAmountOfFactorySelectedUIToAmountOfExistingFactories();
        syncSelectedFactoryUIPosition();
        checkProductionUIStatus();
    }

    public void checkIfFactoryWasSelected()
    {
        if (isHoveringUIElement)
        {
            return;
        }
        
        if (Input.GetMouseButton(0))
        {
            FactoryAuthoring localFactoryAuthComponent;
            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if ( Physics.Raycast (ray, out hit, Mathf.Infinity))
            {
                var factoryHolderGameObject = hit.transform.gameObject;

                if (factoryHolderGameObject.tag == "factoryHolder")
                {
                    localFactoryAuthComponent = factoryHolderGameObject.GetComponentInChildren<FactoryAuthoring>();
                    
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        currentlySelectedFactories.Add(localFactoryAuthComponent);
                    }
                    else
                    {
                        currentlySelectedFactories.Clear();
                        currentlySelectedFactories.Add(localFactoryAuthComponent);
                    }
                }
                else
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        currentlySelectedFactories.Clear();  
                    }
                }
                
            }
            
            if (currentlySelectedFactories.Count > 0)
            {
                atLeastOneFactoryIsCurrentlySelected = true;
            }
            else
            {
                atLeastOneFactoryIsCurrentlySelected = false;
            }
        }
    }


    public void syncAmountOfFactorySelectedUIToAmountOfExistingFactories()
    {

        int tempAmountOfUIElements = selectedFactoryVisualizerUI.Count;

        int amountOfFactoriesActive = currentlySelectedFactories.Count;

        if (tempAmountOfUIElements == amountOfFactoriesActive)
        {
            return;
        }
        
        
        if (amountOfFactoriesActive > tempAmountOfUIElements )
        {
            while (amountOfFactoriesActive > tempAmountOfUIElements)
            {
                var newFactorySelectionUIElement = Instantiate(selectedFactoryUIPrefab);
                selectedFactoryVisualizerUI.Add(newFactorySelectionUIElement);
                tempAmountOfUIElements = selectedFactoryVisualizerUI.Count;
            }
        }
      
        if (amountOfFactoriesActive < tempAmountOfUIElements )
        {
            while (amountOfFactoriesActive < tempAmountOfUIElements)
            {
                var unitSelectionUIToRemove = selectedFactoryVisualizerUI[selectedFactoryVisualizerUI.Count - 1];
                selectedFactoryVisualizerUI.Remove(unitSelectionUIToRemove);
                Destroy(unitSelectionUIToRemove);
                tempAmountOfUIElements = selectedFactoryVisualizerUI.Count;
            }
        } 
    }
    
    public void syncSelectedFactoryUIPosition()
    {

        if (currentlySelectedFactories.Count < 1)
        {
            return;
        }
        
        for (int i = 0; i < currentlySelectedFactories.Count; i++)
        {
            var tempFactoryVisualizerComponent =
                selectedFactoryVisualizerUI[i].GetComponent<factory_selection_visualizer_logic>();

            tempFactoryVisualizerComponent.selectedFactoryPosition = currentlySelectedFactories[i].transform.position;

            tempFactoryVisualizerComponent.selectedFactoryPosition.y = 0.12f;
            
            tempFactoryVisualizerComponent.isVisible = true;

        }
    }

    public void checkProductionUIStatus()
    {
        if (atLeastOneFactoryIsCurrentlySelected)
        {
            factoryProductionUIElement.SetActive(true);
        }
        else
        {
            factoryProductionUIElement.SetActive(false);
        }
    }
    
    //
    // functions called by ui
    //

    public void addUnitToProductionQuque(int inputUnitNumber)
    {
        foreach (var factoryInstance in currentlySelectedFactories)
        {
            factoryInstance.buildQuque.Add(inputUnitNumber);
        }
    }
    
    public static bool isHoveringUIElement
    {
        get
        {
            var mouseOveredObjectName = "";
            var mouseOveredObjectTag = "";
 
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };
 
            pointerData.position = Input.mousePosition;
 
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
 
            if (results.Count > 0)
            {
                mouseOveredObjectName = results[0].gameObject.name;
                mouseOveredObjectTag = results[0].gameObject.tag;
 
                return results[0].gameObject.layer == 5; // 5 is Unity's UI layer
            }
 
            return false;
        }
    }
    
}
