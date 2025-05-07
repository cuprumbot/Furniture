using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModelMenu : MonoBehaviour
{
    // the button template and where to put it
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    
    // list with all the models in the folder
    private List<ModelData> modelList;

    // expose this so the other script knows which model to place
    public GameObject prefabToPlace;

    // DEBUG
    public TextMeshProUGUI statusText;

    void Start()
    {
        modelList = Resources.LoadAll<ModelData>("ModelData").ToList();
        PopulateMenu();
    }

    void PopulateMenu()
    {
        statusText.text = "Models: " + modelList.Count;

        foreach (var model in modelList)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);

            button.GetComponentInChildren<TMP_Text>().text = model.name;



            Transform iconTransform = button.transform.Find("Icon");
            if (iconTransform != null)
            {
                iconTransform.GetComponent<Image>().sprite = model.icon;
            }

            //button.GetComponentInChildren<Image>().sprite = model.icon;
            button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    prefabToPlace = model.prefab;
                    statusText.text = "Clicked: " + model.name;
                }
            );
        }
    }
}
