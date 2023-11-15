using UnityEngine;

// simple quitter to be attached to a gameObject
// can be connected to a ui button


public class quitter : MonoBehaviour
{
  public void quitGame()
  {
    Application.Quit();
  }
}
