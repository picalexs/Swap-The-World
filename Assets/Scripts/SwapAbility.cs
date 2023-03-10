using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwapAbility : MonoBehaviour
{
    [SerializeField] private float selectRange = 0.25f; // The range of the swap ability
    private GameObject firstObject; // The first object to be swapped
    private GameObject secondObject; // The second object to be swapped
    private List<MonoBehaviour> firstScripts = new List<MonoBehaviour>(); // The scripts to be swapped from the first object
    private List<MonoBehaviour> secondScripts = new List<MonoBehaviour>(); // The scripts to be swapped from the second object

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (firstObject == null)
            {
                Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                RaycastHit2D hit = Physics2D.CircleCast(rayPos, selectRange, Vector2.zero, 0f);
                if (hit.collider != null && hit.collider.gameObject.tag == "Swapable")
                {
                    firstObject = hit.collider.gameObject;
                }
            }
        }
        // Check for mouse button release to finish selecting game objects and initiate swap
        if (Input.GetMouseButtonUp(0))
        {
            if (secondObject == null)
            {
                Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                RaycastHit2D hit = Physics2D.CircleCast(rayPos, selectRange, Vector2.zero, 0f);
                if (hit.collider != null && hit.collider.gameObject.tag == "Swapable")
                {
                    secondObject = hit.collider.gameObject;
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

        // Swap the scripts
        SwapScripts(firstObj,secondObj);
    }
    void SwapScripts(GameObject firstObject, GameObject secondObject)
    {
        // Get the scripts to swap from each game object
        GetScriptsToSwap(firstObject, firstScripts);
        GetScriptsToSwap(secondObject, secondScripts);

        // Remove any duplicate or disabled scripts from the lists
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

    // Helper method to get the game object selected by the player
    private GameObject GetSelectedObject(Vector3 mousePosition)
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y);
        RaycastHit2D hit = Physics2D.CircleCast(rayPos, selectRange, Vector2.zero, 0f);
        if (hit.collider != null && hit.collider.gameObject.tag=="Swapable")
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    // Helper method to get the scripts to swap from a game object
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

    // Helper method to copy the properties of a script from one object to another
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
