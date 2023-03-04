using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectData", order = 1)]
public class ObjectData : ScriptableObject, ISerializationCallbackReceiver
{
    public string initialObjectName;
    public string initialObjectDescription;
    public GameObject initialGameObject;
    public float initialObjectGravityScale;
    public float initialObjectMass;

    [HideInInspector] public string RuntimeObjectName;
    [HideInInspector] public string RuntimeObjectDescription;
    [HideInInspector] public GameObject RuntimeGameObject;
    [HideInInspector] public float RuntimeObjectGravityScale;
    [HideInInspector] public float RuntimeObjectMass;
    public void OnAfterDeserialize()
    {
        RuntimeObjectName = initialObjectName;
        RuntimeObjectDescription=initialObjectDescription;
        RuntimeGameObject = initialGameObject;
        RuntimeObjectGravityScale = initialObjectGravityScale;
        RuntimeObjectMass = initialObjectMass;
    }

    public void OnBeforeSerialize() { }

    private List<GameEventListener> listeners = new List<GameEventListener>();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    public void RegisterListener(GameEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(GameEventListener listener)
    { listeners.Remove(listener); }

}
