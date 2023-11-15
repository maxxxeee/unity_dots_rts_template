using UnityEngine;
using Unity.Mathematics;

// this script syncs it's position with the current unit that is being hovered over by the mouse

// it receives it's data from the "syncToGameObject" system

public class UIPositionSyncWithHoverEntity : MonoBehaviour
{
    public float3 targetPosition;
    public float currentHealthValue;
    public float maxHealthValue;
    
    public bool isVisible = false;

    public UnityEngine.UI.Slider healthBar;

    private Camera mainCamera;
    
    void Start()
    {
        healthBar = GetComponentInChildren<UnityEngine.UI.Slider>();
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (isVisible)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.transform.position = mainCamera.WorldToScreenPoint(targetPosition) - new Vector3(0.0f, 40.0f, 0.0f);
            healthBar.value = (healthBar.maxValue / maxHealthValue) * currentHealthValue;
        }
        else
        {
            healthBar.gameObject.SetActive(false);  
        }
    }
}
