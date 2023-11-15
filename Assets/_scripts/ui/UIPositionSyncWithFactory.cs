using TMPro;
using UnityEngine;
using Unity.Mathematics;

// this script updates its position and progress bar status with externally provided data

public class UIPositionSyncWithFactory : MonoBehaviour
{

    public float3 targetPosition;
    public float currentProgress;

    public bool isVisible = false;

    public UnityEngine.UI.Slider progressBar;

    public int buildQuqueSize;
    
    private Camera mainCamera;

    public TMP_Text buildQuqueSizeText;
    
    void Start()
    {
        progressBar = GetComponentInChildren<UnityEngine.UI.Slider>();
        buildQuqueSizeText = GetComponentInChildren<TMP_Text>();
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (isVisible)
        {
            progressBar.gameObject.SetActive(true);
            
            progressBar.transform.position = mainCamera.WorldToScreenPoint(targetPosition) - new Vector3(0.0f, 60.0f, 0.0f);
            progressBar.value = currentProgress;
            // add 1 to build quque size as the user will think that the unit currently being build should count aswell
            buildQuqueSizeText.text = (buildQuqueSize + 1).ToString();
        }
        else
        {
            progressBar.gameObject.SetActive(false);  
        }

    }
}
