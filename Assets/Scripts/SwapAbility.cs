using AllIn1SpriteShader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwapAbility : MonoBehaviour
{
    [SerializeField] private GameObject movmentManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject playerObject;
    [Space(10)]
    [SerializeField] private AudioSource swapEndSound;
    [SerializeField] private ParticleSystem abilityParticleSystem;
    private Material playerMaterial;
    private RewindTimeAbility timeAbility;
    private BlinkingScript blinkingScript;
    private PlayerScript playerScript;
    private CameraManager cameraManager;
    private MaterialSwapper materialSwapper;
   
    [Space(10)]
    [SerializeField] private float selectRange = 1f;
    [SerializeField] private float swapPlayerCooldown = 4f;
    [SerializeField] private float swapPlayerProprietyCooldown = 5f;
    [SerializeField] private float abilityCooldown = 4f;
    [SerializeField] private float failedAbilityCooldown = 1f;
    private float playerTimer;
    private float proprietyTimer;
    private float abilityTimer;

    private GameObject playerObj, objectObj;
    private Rigidbody2D firstRigidbody;
    private Rigidbody2D secondRigidbody;
    private Collider2D firstCollider;
    private Collider2D secondCollider;
    private GameObject firstObjToSwap;
    private GameObject secondObjToSwap;

    private bool isSwapped = false;
    private bool isSwappedPropriety = false;
    private bool isWaitingToSwap = false;

    private GameObject firstObject;
    private List<MonoBehaviour> firstScripts = new();
    private List<MonoBehaviour> secondScripts = new();

    [SerializeField] private Material highlightMaterial;
    private GameObject highlightedObject;
    private Material originalMaterial;
    private GameObject clone;
    public LayerMask swapLayerMask;
    public int cloneLayer;

    public List<MonoBehaviour> FirstScripts { get => FirstScripts1; set => FirstScripts1 = value; }
    public List<MonoBehaviour> FirstScripts1 { get => firstScripts; set => firstScripts = value; }

    private void Start()
    {
        blinkingScript = GetComponent<BlinkingScript>();
        timeAbility = GetComponent<RewindTimeAbility>();
        playerScript = movmentManager.GetComponent<PlayerScript>();
        playerMaterial = playerObject.GetComponent<Renderer>().material;
        materialSwapper = GetComponent<MaterialSwapper>();
        cameraManager = FindAnyObjectByType<CameraManager>();
    }
    private void Update()
    {
        if (isWaitingToSwap && firstObjToSwap !=null && secondObjToSwap !=null)
        {
            SwapObjects(firstObjToSwap, secondObjToSwap);
            firstObjToSwap = null;
            secondObjToSwap = null;
            isWaitingToSwap = false;
        }

        if (isSwapped)
        {
            if (playerTimer < 0f || Input.GetMouseButtonDown(1))
            {
                ResetSwapPlayerObject();
            }
            else
            {
                playerTimer -= Time.deltaTime;
            }
        }

        if (isSwappedPropriety)
        {
            if (proprietyTimer < 0f)
            {
                SwapObjects(objectObj, playerObj);
                swapEndSound.Play();
                isSwappedPropriety = false;
                playerScript._isSwappedPropriety = false;
            }
            else
            {
                proprietyTimer -= Time.deltaTime;
            }
        }

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = mainCamera.nearClipPlane;
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Collider2D[] hits = Physics2D.OverlapCircleAll(mouseWorldPosition, selectRange, swapLayerMask);

        float minDistance = Mathf.Infinity;
        Collider2D hit = null;

        foreach (Collider2D hitObject in hits)
        {
            float distance = Vector2.Distance(hitObject.transform.position, mouseWorldPosition);

            if (distance < minDistance)
            {
                hit = hitObject;
                minDistance = distance;
            }
        }

        abilityTimer -= Time.deltaTime;

        if (hit != null && abilityTimer < 0f)
        {
            HighlightObject(hit.gameObject);
            if (Input.GetMouseButtonDown(0))
            {
                if (materialSwapper.swapped == false)
                {
                    materialSwapper.SwapMaterials();
                }
                if (abilityParticleSystem != null)
                {
                    abilityParticleSystem.Play();
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
            if (abilityParticleSystem != null)
            {
                abilityParticleSystem.Stop();
            }
            if (materialSwapper.swapped == true)
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
                if (abilityParticleSystem != null)
                {
                    abilityParticleSystem.Stop();
                }
                playerMaterial.SetFloat("_OutlineAlpha", 0f);
                abilityTimer = failedAbilityCooldown;
                if (materialSwapper.swapped == true)
                {
                    materialSwapper.EndSwapMaterials();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            RemoveHighlight();
            if (clone != null) {
                if (hit != null && firstObject != hit.gameObject)
                {
                    SwapObjects(firstObject, hit.gameObject);
                    abilityTimer = abilityCooldown;
                    firstObject = null;
                }
            }
            timeAbility.CancelSlowDown();
            RemoveClone();
            if (abilityParticleSystem != null)
            {
                abilityParticleSystem.Stop();
            }
            playerMaterial.SetFloat("_OutlineAlpha", 0f);
            if (materialSwapper.swapped == true)
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
            originalMaterial = highlightedObject.GetComponent<Renderer>().material;
            highlightedObject.GetComponent<Renderer>().material = highlightMaterial;
        }
    }

    void RemoveHighlight()
    {
        if (highlightedObject != null)
        {
            highlightedObject.GetComponent<Renderer>().material = originalMaterial;
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
        if (firstObj == null || secondObj == null)
        {
            Debug.Log("one of the selected objects is null!!");
            firstObjToSwap = firstObj;
            secondObjToSwap = secondObj;
            isWaitingToSwap = true;
            return;
        }
        if (firstObj == secondObj)
        {
            Debug.Log("same object selected!!");
            return;
        }

        if (!isSwapped)
        {
            if (firstObj.CompareTag("Player") && secondObj.CompareTag("Swappable"))
            {
                playerObj = firstObj;
                objectObj = secondObj;

                Debug.Log("focusing on: " + secondObj.name);
                cameraManager.FocusOnSelectedObject(secondObj.transform);
                playerScript.ChangePlayerObjectTo(secondObj);
                blinkingScript.ChangePlayerObjectTo(secondObj);
                Debug.Log("changed playerObject to " + secondObj);
                playerTimer = swapPlayerCooldown;
                isSwapped = true;
            }
            else if (secondObj.CompareTag("Player") && firstObj.CompareTag("Swappable"))
            {
                playerObj = secondObj;
                objectObj = firstObj;
                Debug.Log("focusing on: " + firstObj.name);
                cameraManager.FocusOnSelectedObject(firstObj.transform);
                playerScript.ChangePlayerObjectTo(firstObj);
                blinkingScript.ChangePlayerObjectTo(firstObj);
                Debug.Log("changed playerObject to " + firstObj);
                playerTimer = swapPlayerCooldown;
                isSwapped = true;
            }
        }
        SwapRigidbodys(firstObj,secondObj);
        SwapScripts(firstObj, secondObj);
    }
    void SwapRigidbodys(GameObject firstObj, GameObject secondObj)
    {
        firstRigidbody = firstObj.GetComponent<Rigidbody2D>();
        secondRigidbody = secondObj.GetComponent<Rigidbody2D>();
        firstCollider = firstObj.GetComponent<Collider2D>();
        secondCollider = secondObj.GetComponent<Collider2D>();

        if (firstRigidbody != null && secondRigidbody != null)
        {
            if (!isSwapped)
            {
                isSwappedPropriety = true;
                playerScript._isSwappedPropriety = true;
                proprietyTimer = swapPlayerProprietyCooldown;
            }
            (secondRigidbody.gravityScale, firstRigidbody.gravityScale) = (firstRigidbody.gravityScale, secondRigidbody.gravityScale);
            (secondRigidbody.mass, firstRigidbody.mass) = (firstRigidbody.mass, secondRigidbody.mass);

            PhysicsMaterial2D object1Material = firstCollider.sharedMaterial;
            PhysicsMaterial2D object2Material = secondCollider.sharedMaterial;
            if (object1Material != null && object2Material != null)
            {
                (object2Material.friction, object1Material.friction) = (object1Material.friction, object2Material.friction);
                firstCollider.sharedMaterial = object1Material;
                secondCollider.sharedMaterial = object2Material;
                Debug.Log("swapped materials");
            }
            Debug.Log("swapped rigidbodys");
        }
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
        Debug.Log("swapped scritps");
    }

    public void ResetSwapPlayerObject()
    {
        Debug.Log("changed playerObject to " + playerObj);
        SwapObjects(objectObj, playerObj);
        cameraManager.FocusOnSelectedObject(playerObj.transform);
        playerScript.ChangePlayerObjectTo(playerObj);
        blinkingScript.ChangePlayerObjectTo(playerObj);
        swapEndSound.Play();
        blinkingScript.StopAbility();
        isSwapped = false;
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

    private void OnApplicationQuit()
    {
        if(isSwapped) {
            PhysicsMaterial2D object1Material = firstCollider.sharedMaterial;
            PhysicsMaterial2D object2Material = secondCollider.sharedMaterial;

            (object2Material.friction, object1Material.friction) = (object1Material.friction, object2Material.friction);
            firstCollider.sharedMaterial = object1Material;
            secondCollider.sharedMaterial = object2Material;
        }
    }
}