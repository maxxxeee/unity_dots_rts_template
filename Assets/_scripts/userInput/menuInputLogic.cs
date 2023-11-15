using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//this takes user input and holds it to be used by a spawner system in another scene

public class menuInputLogic : MonoBehaviour
{
    public int amountOfUnitsToSpawn;
    private int amountOfUnitsToSpawnPerTeam;
    public int amountOfUnitsToSpawnPerAxis;


    public TMP_InputField inputField;
    public GameObject feedbackTextGameObject;
    
    
    void Start()
    {
        DontDestroyOnLoad(this);
        feedbackTextGameObject.SetActive(false);
        
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        userSubmittedNumber();
    }

    public void userSubmittedNumber()
    {
       
        amountOfUnitsToSpawn = int.Parse(inputField.text,System.Globalization.NumberStyles.Integer);

        amountOfUnitsToSpawnPerTeam = amountOfUnitsToSpawn / 2;

        amountOfUnitsToSpawnPerAxis = (int)Mathf.Sqrt(amountOfUnitsToSpawnPerTeam);
    }

    public void loadBenchmarkScene()
    {
        SceneManager.LoadScene("combat_mass_spawn_userInput");
    }
    
}
