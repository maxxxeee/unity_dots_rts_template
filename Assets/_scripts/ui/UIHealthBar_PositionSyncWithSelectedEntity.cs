using UnityEngine;
using Unity.Mathematics;


// script to control and update an individual unit's healthbar

// it receives it's data externally by a coordinator

public class UIHealthBar_PositionSyncWithSelectedEntity : MonoBehaviour
{
    public float3 targetPosition;
    public float currentHealthValue;
    public float maxHealthValue;
    
    public bool isVisible = false;

    public UnityEngine.UI.Slider healthBar;

    private Camera mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<UnityEngine.UI.Slider>();

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isVisible)
        {
            healthBar.gameObject.SetActive(true);
        }
        else
        {
            healthBar.gameObject.SetActive(false);  
        }

        
        healthBar.transform.position = mainCamera.WorldToScreenPoint(targetPosition) - new Vector3(0.0f, 40.0f, 0.0f);
            
        healthBar.value = (healthBar.maxValue / maxHealthValue) * currentHealthValue;
    }
}
