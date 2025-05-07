using UnityEngine;

public class DisplayHelper : MonoBehaviour
{
    public GameObject modelSelector;
    private bool isModelSelectorVisible = false;

    public void toggleModelSelectorVisibility ()
    {
        isModelSelectorVisible = !isModelSelectorVisible;
        modelSelector.SetActive(isModelSelectorVisible);
    }

    public void showModelSelector () => modelSelector.SetActive(true);
    public void hideModelSelector() => modelSelector.SetActive(false);
}
