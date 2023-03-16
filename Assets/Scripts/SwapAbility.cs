using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwapAbility : MonoBehaviour
{
    [SerializeField] private GameObject movmentManager;
    [SerializeField] private Camera mainCamera;
    private RewindTimeAbility timeAbility;
    private PlayerScript playerScript;

    [SerializeField] private float selectRange = 1f;
    [SerializeField] private float swapPlayerTime = 4f;
    private float playerTimer;
    private GameObject playerObj, ObjectObj;
    private bool isSwaped = false;

    private GameObject firstObject;
    private List<MonoBehaviour> firstScripts = new List<MonoBehaviour>();
    private List<MonoBehaviour> secondScripts = new List<MonoBehaviour>();

    
    // Variables to store the game objects and their clones
    private GameObject highlightedObject;
    private GameObject clone;
    public LayerMask swapLayerMask;
    public int cloneLayer;


    private void Start()
    {
        timeAbility = GetComponent<RewindTimeAbility>();
        playerScript = movmentManager.GetComponent<PlayerScript>();
    }
    private void Update()
    {
        if (isSwaped)
        {
            if (playerTimer < 0f)
            {
                Debug.Log("changed playerObject to " + playerObj);
                SwapObjects(playerObj, ObjectObj);
                playerScript.ChangePlayerObjectTo(playerObj);
                isSwaped = false;
            }
            else
            {
                playerTimer -= Time.deltaTime;
            }
        }

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = mainCamera.nearClipPlane;
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Collider2D hit = Physics2D.OverlapCircle(mouseWorldPosition, selectRange, swapLayerMask);

        if (hit != null)
        {
            HighlightObject(hit.gameObject);
            if (Input.GetMouseButtonDown(0))
            {
                timeAbility.SlowTimeDown();
                firstObject = hit.gameObject;
                CloneObject(hit.gameObject);
                Debug.Log(hit.gameObject.name);
            }
        }
        else if(!Input.GetMouseButton(0))
        {
            RemoveHighlight();
            RemoveClone();
        }

        if (Input.GetMouseButton(0) && clone != null)
        {
            MoveCloneToMouse();
        }

        if (Input.GetMouseButtonUp(0) && clone != null)
        {
            Debug.Log(hit != null ? hit.gameObject.name : "");
            if (hit!=null && firstObject != hit.gameObject)
            {
                SwapObjects(firstObject, hit.gameObject);
                firstObject = null;
            }
            timeAbility.CancelSlowDown();
            RemoveClone();
            RemoveHighlight();
        }
    }
    void HighlightObject(GameObject obj)
    {
        if (highlightedObject != obj)
        {
            RemoveHighlight();
            highlightedObject = obj;
            highlightedObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
    void RemoveHighlight()
    {
        if (highlightedObject != null)
        {
            highlightedObject.GetComponent<Renderer>().material.color = Color.white;
            highlightedObject = null;
        }
    }
    void CloneObject(GameObject obj)
    {
        RemoveClone();
        clone = new GameObject(obj.name + " (Clone)");
        SpriteRenderer spriteRenderer = clone.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = obj.GetComponent<SpriteRenderer>().sprite;

        clone.transform.position = obj.transform.position;
        clone.transform.rotation = obj.transform.rotation;
        clone.transform.localScale = obj.transform.localScale;

        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
    }
    void RemoveClone()
    {
        if (clone != null)
        {
            Destroy(clone);
            clone = null;
        }
    }

    void MoveCloneToMouse()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        clone.transform.position = new Vector3(mousePosition.x, mousePosition.y, clone.transform.position.z);
    }
    public void SwapObjects(GameObject firstObj, GameObject secondObj)
    {
        Debug.Log("start swap");
        if (firstObj == secondObj)
            return;
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

        if (firstObj.gameObject.tag == "Player")
        {
            playerObj = firstObj;
            ObjectObj = secondObj;
            playerScript.ChangePlayerObjectTo(secondObj);
            Debug.Log("changed playerObject to " + secondObj);
            playerTimer = swapPlayerTime;
            isSwaped = true;
        }
        else if (secondObj.gameObject.tag == "Player")
        {
            playerObj = secondObj;
            ObjectObj = firstObj;
            playerScript.ChangePlayerObjectTo(firstObj);
            Debug.Log("changed playerObject to " + firstObj);
            playerTimer = swapPlayerTime;
            isSwaped = true;
        }

        Debug.Log("swaped rb");
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