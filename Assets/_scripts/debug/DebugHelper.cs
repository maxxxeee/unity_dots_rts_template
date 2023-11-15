using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    public float SimulationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = SimulationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
