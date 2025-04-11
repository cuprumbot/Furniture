using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonHandler : MonoBehaviour
{
    // Text to be modified
    public TextMeshProUGUI statusText;

    // Place
    public void OnPlaceButtonClicked()
    {
        statusText.text = "Place";
    }

    // Move
    public void OnMoveButtonClicked()
    {
        statusText.text = "Move";
    }

    // Delete
    public void OnDeleteButtonClicked()
    {
        statusText.text = "Delete";
    }
}