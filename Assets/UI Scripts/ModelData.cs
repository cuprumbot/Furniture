using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AR/Model Data")]
public class ModelData : ScriptableObject
{
    public string modelName;
    public GameObject prefab;
    public Sprite icon;
}
