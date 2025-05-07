using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonHandler : MonoBehaviour
{
    // Text to be modified
    public TextMeshProUGUI statusText;
    public GameObject xrOrigin;
    public GameObject table;
    public GameObject chair;

    // Place
    public void OnPlaceButtonClicked()
    {
        statusText.text = "Place";
        ModeHandler.setPlaceMode();
    }

    // Move
    public void OnMoveButtonClicked()
    {
        statusText.text = "Move";
        ModeHandler.setMoveMode();
    }

    // Delete
    public void OnDeleteButtonClicked()
    {
        statusText.text = "Delete";
        ModeHandler.setDeleteMode();
    }
}