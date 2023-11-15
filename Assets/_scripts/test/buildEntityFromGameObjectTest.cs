using UnityEngine;

// for testing
// this logic adds a unit to build to all factories it can find on start of the scene

public class buildEntityFromGameObjectTest : MonoBehaviour
{
    private FactoryAuthoring[] factoryGameObjects;
    
    void Start()
    {
        factoryGameObjects = GameObject.FindObjectsByType<FactoryAuthoring>(FindObjectsSortMode.InstanceID);
        addUnitToBuildQuque(0);

    }

    
    public void addUnitToBuildQuque(int unitIndexFromBuildOptions)
    {

        foreach (var factoryInstance in factoryGameObjects)
        {
            factoryInstance.buildQuque.Add(unitIndexFromBuildOptions);
        }
    }
    
}
