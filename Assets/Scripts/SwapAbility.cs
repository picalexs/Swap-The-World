using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwapAbility : MonoBehaviour
{
    private RewindTimeAbility timeAbility;
    [SerializeField] private float selectRange = 0.25f; // The range of the swap ability
    private GameObject firstObject; // The first object to be swapped
    private GameObject secondObject; // The second object to be swapped
    private List<MonoBehaviour> firstScripts = new List<MonoBehaviour>(); // The scripts to be swapped from the first object
    private List<MonoBehaviour> secondScripts = new List<MonoBehaviour>(); // The scripts to be swapped from the second object
    [SerializeField] private Camera mainCamera;

    private void Start()
    {
        timeAbility = GetComponent<RewindTimeAbility>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            timeAbility.SlowTimeDown();
            if (firstObject == null)
            {
                Vector3 mousePosition = Mouse.current.position.ReadValue();
                mousePosition.z = mainCamera.nearClipPlane;
                Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
                Collider2D hit = Physics2D.OverlapCircle(mouseWorldPosition, selectRange);

                if (hit != null && hit.gameObject.tag == "Swapable")
                {
                    SpriteRenderer renderer = hit.gameObject.GetComponent<SpriteRenderer>();
                    if (renderer == null)
                    {
                        Debug.LogError("Object must have a SpriteRenderer component");
                        return;
                    }
                    Bounds bounds = renderer.bounds;

                    if (bounds.Contains(mouseWorldPosition))
                    {
                        firstObject = hit.gameObject;
                        Debug.Log("Selected object: " + firstObject.name);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            timeAbility.CancelSlowDown();
            if (secondObject == null)
            {
                Vector3 mousePosition = Mouse.current.position.ReadValue();
                mousePosition.z = mainCamera.nearClipPlane;
                Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
                Collider2D hit = Physics2D.OverlapCircle(mouseWorldPosition, selectRange);

                if (hit != null && hit.gameObject.tag == "Swapable")
                {
                    SpriteRenderer renderer = hit.gameObject.GetComponent<SpriteRenderer>();
                    if (renderer == null)
                    {
                        Debug.LogError("Object must have a SpriteRenderer component");
                        return;
                    }
                    Bounds bounds = renderer.bounds;

                    if (bounds.Contains(mouseWorldPosition))
                    {
                        secondObject = hit.gameObject;
                        Debug.Log("Selected object: " + secondObject.name);
                    }
                }
                if (firstObject != null && secondObject != null && firstObject != secondObject)
                {
                    SwapObjects(firstObject, secondObject);
                    Debug.Log("ability used");
                }
            }
            firstObject = null;
            secondObject = null;
        }
    }

    public void SwapObjects(GameObject firstObj, GameObject secondObj)
    {
        // Swap the rigidbodies
        Rigidbody2D firstRigidbody = firstObj.GetComponent<Rigidbody2D>();
        Rigidbody2D secondRigidbody = secondObj.GetComponent<Rigidbody2D>();
        if (firstRigidbody != null && secondRigidbody != null)
        {
            Vector2 firstVelocity = firstRigidbody.velocity;
            firstRigidbody.velocity = secondRigidbody.velocity;
            secondRigidbody.velocity = firstVelocity;

            var firstGravityScale = firstRigidbody.gravityScale;
            firstRigidbody.gravityScale = secondRigidbody.gravityScale;
            secondRigidbody.gravityScale = firstGravityScale;

            var firstMass = firstRigidbody.mass;
            firstRigidbody.mass = secondRigidbody.mass;
            secondRigidbody.mass = firstMass;
        }
        SwapScripts(firstObj, secondObj);
    }
    void SwapScripts(GameObject firstObject, GameObject secondObject)
    {
        GetScriptsToSwap(firstObject, firstScripts);
        GetScriptsToSwap(secondObject, secondScripts);

        firstScripts = firstScripts.Distinct().ToList();
        secondScripts = secondScripts.Distinct().ToList();
        firstScripts.RemoveAll(s => s == null || s.enabled == false);
        secondScripts.RemoveAll(s => s == null || s.enabled == false);

        // Swap the scripts
        foreach (MonoBehaviour script in firstScripts)
        {
            script.enabled = false;
        }
        foreach (MonoBehaviour script in secondScripts)
        {
            script.enabled = false;
        }
        foreach (MonoBehaviour script in firstScripts)
        {
            MonoBehaviour newScript = secondObject.AddComponent(script.GetType()) as MonoBehaviour;
            CopyComponent(script, newScript);
            newScript.enabled = true;
        }
        foreach (MonoBehaviour script in secondScripts)
        {
            MonoBehaviour newScript = firstObject.AddComponent(script.GetType()) as MonoBehaviour;
            CopyComponent(script, newScript);
            newScript.enabled = true;
        }

        MonoBehaviour[] allFirstScripts = firstObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allFirstScripts)
        {
            if (!(script is SwapAbility) && !script.enabled && firstScripts.IndexOf(script) == -1)
            {
                Destroy(script);
            }
        }
        MonoBehaviour[] allSecondScripts = secondObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allSecondScripts)
        {
            if (!(script is SwapAbility) && !script.enabled && secondScripts.IndexOf(script) == -1)
            {
                Destroy(script);
            }
        }
    }
    private void GetScriptsToSwap(GameObject gameObject, List<MonoBehaviour> scripts)
    {
        scripts.Clear();
        MonoBehaviour[] allScripts = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script is SwapAbility) continue; // Exclude the SwapAbility script
            if (!script.enabled) continue;// Exclude disabled scripts
            if (scripts.Contains(script)) continue; // Exclude duplicate scripts
            scripts.Add(script);
        }
    }
    private void CopyComponent(MonoBehaviour source, MonoBehaviour target)
    {
        System.Type type = source.GetType();
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(target, field.GetValue(source));
        }
    }
}