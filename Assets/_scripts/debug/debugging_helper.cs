using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugging_helper : MonoBehaviour
{

    public float inputTimeScale;

    public bool applyTimeScale;
    // Start is called before the first frame update
    void Start()
    {
        if (applyTimeScale)
        {
            Time.timeScale = inputTimeScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
