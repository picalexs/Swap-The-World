using UnityEngine.Events;
using UnityEngine;
public class GameEventListener : MonoBehaviour
{
    public ObjectData EventData;
    public UnityEvent Response;

    private void OnEnable()
    { EventData.RegisterListener(this); }

    private void OnDisable()
    { EventData.UnregisterListener(this); }

    public void OnEventRaised()
    { Response.Invoke(); }
}