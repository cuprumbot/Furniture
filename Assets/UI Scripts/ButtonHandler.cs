using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonHandler : MonoBehaviour
{
    // Text to be modified
    public TextMeshProUGUI statusText;

    // Show and hide elements
    public DisplayHelper displayHelper;

    // Select
    public void OnSelectButtonClicked()
    {
        statusText.text = "Select";
        ModeHandler.setSelectMode();
        displayHelper.toggleModelSelectorVisibility();
    }

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