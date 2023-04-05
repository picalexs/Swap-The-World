using AllIn1SpriteShader;
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
    [SerializeField] private GameObject playerObject;
    private Material playerMaterial;
    private RewindTimeAbility timeAbility;
    private PlayerScript playerScript;
    private CameraManager cameraManager;
    private MaterialSwapper materialSwapper;
    [SerializeField] private AudioSource swapEndSound;

    [SerializeField] private float selectRange = 1f;
    [SerializeField] private float swapPlayerCooldown = 4f;
    private float playerTimer;
    [SerializeField] private float abilityCooldown = 4f;
    private float abilityTimer;
    private GameObject playerObj, objectObj;
    private bool isSwaped = false;

    private GameObject firstObject;
    private List<MonoBehaviour> firstScripts = new();
    private List<MonoBehaviour> secondScripts = new();

    // Variables to store the game objects and their clones
    [SerializeField] private Color highlightColor;
    private GameObject highlightedObject;
    private GameObject clone;
    public LayerMask swapLayerMask;
    public int cloneLayer;

    public List<MonoBehaviour> FirstScripts { get => FirstScripts1; set => FirstScripts1 = value; }
    public List<MonoBehaviour> FirstScripts1 { get => firstScripts; set => firstScripts = value; }

    private void Start()
    {
        timeAbility = GetComponent<RewindTimeAbility>();
        playerScript = movmentManager.GetComponent<PlayerScript>();
        playerMaterial = playerObject.GetComponent<Renderer>().material;
        materialSwapper = GetComponent<MaterialSwapper>();
        cameraManager = FindAnyObjectByType<CameraManager>();
    }
    private void Update()
    {
        if (isSwaped)
        {
            if (playerTimer < 0f)
            {
                Debug.Log("changed playerObject to " + playerObj);
                SwapObjects(objectObj, playerObj);
                cameraManager.FocusOnSelectedObject(playerObj.transform);
                playerScript.ChangePlayerObjectTo(playerObj);
                swapEndSound.Play();
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

        abilityTimer -= Time.deltaTime;

        if (hit != null && abilityTimer < 0f)
        {
            HighlightObject(hit.gameObject);
            if (Input.GetMouseButtonDown(0))
            {
                if (materialSwapper.swaped == false)
                {
                    materialSwapper.SwapMaterials();
                }
                playerMaterial.SetFloat("_OutlineAlpha", 1f);
                timeAbility.SlowTimeDown();
                firstObject = hit.gameObject;
                CloneObject(hit.gameObject);
                Debug.Log(hit.gameObject.name);
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            RemoveHighlight();
            RemoveClone();
            if (materialSwapper.swaped == true)
            {
                materialSwapper.EndSwapMaterials();
            }
        }

        if (Input.GetMouseButton(0) && clone != null)
        {
            MoveCloneToMouse();
            if(timeAbility.slowMotionTimeLeft <= 0f)
            {
                RemoveHighlight();
                RemoveClone();
                playerMaterial.SetFloat("_OutlineAlpha", 0f);
                abilityTimer = abilityCooldown;
                if (materialSwapper.swaped == true)
                {
                    materialSwapper.EndSwapMaterials();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (clone != null) {
                Debug.Log(hit != null ? hit.gameObject.name + "--" + firstObject.name : "");

                if (hit != null && firstObject != hit.gameObject)
                {
                    SwapObjects(firstObject, hit.gameObject);
                    abilityTimer = abilityCooldown;
                    firstObject = null;
                }
            }
            timeAbility.CancelSlowDown();
            RemoveClone();
            RemoveHighlight();
            playerMaterial.SetFloat("_OutlineAlpha", 0f);
            if (materialSwapper.swaped == true)
            {
                materialSwapper.EndSwapMaterials();
            }
        }
    }
    void HighlightObject(GameObject obj)
    {
        if (highlightedObject != obj)
        {
            RemoveHighlight();
            highlightedObject = obj;
            highlightedObject.GetComponent<Renderer>().material.color = highlightColor;
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

        clone.transform.SetPositionAndRotation(obj.transform.position, obj.transform.rotation);
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
        Collider2D firstCollider = firstObj.GetComponent<Collider2D>();
        Collider2D secondCollider = secondObj.GetComponent<Collider2D>();

        if (firstRigidbody != null && secondRigidbody != null)
        {
            firstRigidbody.velocity = new Vector2(0, 0);
            secondRigidbody.velocity = new Vector2(0, 0);

            (secondRigidbody.gravityScale, firstRigidbody.gravityScale) = (firstRigidbody.gravityScale, secondRigidbody.gravityScale);
            (secondRigidbody.mass, firstRigidbody.mass) = (firstRigidbody.mass, secondRigidbody.mass);
            PhysicsMaterial2D object1Material = firstCollider.sharedMaterial;
            PhysicsMaterial2D object2Material = secondCollider.sharedMaterial;

            (object2Material.friction, object1Material.friction) = (object1Material.friction, object2Material.friction);

            // Update the shared materials of the colliders
            firstCollider.sharedMaterial = object1Material;
            secondCollider.sharedMaterial = object2Material;
        }   
        if (!isSwaped)
        {
            if (firstObj.CompareTag("Player"))
            {
                playerObj = firstObj;
                objectObj = secondObj;
                Debug.Log("objectObj:" + objectObj);
                cameraManager.FocusOnSelectedObject(secondObj.transform);
                playerScript.ChangePlayerObjectTo(secondObj);
                Debug.Log("changed playerObject to " + secondObj);
                playerTimer = swapPlayerCooldown;
                isSwaped = true;
            }
            else if (secondObj.CompareTag("Player"))
            {
                playerObj = secondObj;
                objectObj = firstObj;
                Debug.Log("objectObj:" + objectObj);
                cameraManager.FocusOnSelectedObject(firstObj.transform);
                playerScript.ChangePlayerObjectTo(firstObj);
                Debug.Log("changed playerObject to " + firstObj);
                playerTimer = swapPlayerCooldown;
                isSwaped = true;
            }
        }
        Debug.Log("swaped rb");
        SwapScripts(firstObj, secondObj);
    }
    void SwapScripts(GameObject firstObject, GameObject secondObject)
    {
        GetScriptsToSwap(firstObject, FirstScripts);
        GetScriptsToSwap(secondObject, secondScripts);
        FirstScripts = FirstScripts.Distinct().ToList();
        secondScripts = secondScripts.Distinct().ToList();
        FirstScripts.RemoveAll(s => s == null || s.enabled == false);
        secondScripts.RemoveAll(s => s == null || s.enabled == false);

        foreach (MonoBehaviour script in FirstScripts)
        {
            script.enabled = false;
        }
        foreach (MonoBehaviour script in secondScripts)
        {
            script.enabled = false;
        }
        foreach (MonoBehaviour script in FirstScripts)
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
            if (script is SwapAbility || script.enabled || FirstScripts.IndexOf(script) != -1)
            {
                continue;
            }
            Destroy(script);
        }
        MonoBehaviour[] allSecondScripts = secondObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allSecondScripts)
        {
            if (script is not SwapAbility && !script.enabled && secondScripts.IndexOf(script) == -1)
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
            if (script is AllIn1Shader) continue;
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