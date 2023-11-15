using UnityEngine;

// script to visualize an ongoing drag box selection
// the actual selection is happening in a system
// both the dragBox system and this script use the same mouse data

//  its important to keep the "minimumDistanceForDragSelection" values in sync
//      with the corresponding value in the dragBox system 


public class UIMouseDragSelection : MonoBehaviour
{
    public float minimumDistanceForDragSelection = 50.0f;
    
    private Vector3 mousePositionOnMouseDown = Vector3.zero;

    private bool lastLeftMouseButtonStatus;


    private bool _isDraggingMouseBox;
 
    
    void Update()
    {
        checkForMouseDragBoxSelection();
        updateMouseStatus();
    }

    private void OnGUI()
    {
        if (_isDraggingMouseBox)
        {
            // Create a rect from both mouse positions
            var rect = UIUtils.GetScreenRect(mousePositionOnMouseDown, Input.mousePosition);
            UIUtils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
            UIUtils.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f)); 
        }
    }

    //should be executed last!
    public void updateMouseStatus()
    {
        lastLeftMouseButtonStatus = Input.GetMouseButton(0);
    }

    public void checkForMouseDragBoxSelection()
    {
        if (Input.GetMouseButton(0) && !lastLeftMouseButtonStatus)
        {
            mousePositionOnMouseDown = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (Vector3.Distance(Input.mousePosition, mousePositionOnMouseDown) > minimumDistanceForDragSelection)
            {
                //drag selection visualisation logic
                _isDraggingMouseBox = true;
            }
        }


        if (!Input.GetMouseButton(0) && lastLeftMouseButtonStatus)
        {
          _isDraggingMouseBox = false;
        }
    }
}
