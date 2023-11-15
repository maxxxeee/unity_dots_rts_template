using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class existingAmountOfUnits_ui_logic : MonoBehaviour
{
    public int amountOfExistingUnits;
    public int amountOfExistingFactories;

    public TMP_Text text_amountOfExistingUnitsPlusFactories;

    private void Start()
    {
        text_amountOfExistingUnitsPlusFactories = GetComponentInChildren<TMP_Text>();
    }

    private void FixedUpdate()
    {
        text_amountOfExistingUnitsPlusFactories.text =
            amountOfExistingUnits.ToString() + "U" + "\n" + amountOfExistingFactories.ToString() + "F";
    }
}
