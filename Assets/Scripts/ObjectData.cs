using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectData", order = 1)]
public class ObjectData : ScriptableObject
{
    public string objectName;
    public string objectDescription;
    public GameObject objectModel;
    public Color32 objectColor;
    public float objectGravityScale;
    public float objectMass;
}
