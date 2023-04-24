using AllIn1SpriteShader;
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
    [Space(10)]
    [SerializeField] private AudioSource swapEndSound;
    [SerializeField] private ParticleSystem abilityParticleSystem;
    [SerializeField] private GameObject cancelAbilityObject;

    private Material playerMaterial;
    private BlinkingScript blinkingScript;
    private PlayerScript playerScript;
    private CameraManager cameraManager;
    private MaterialSwapper materialSwapper;

    private GameObject playerObjectCopy;
    private GameObject objectToSwap;
    private Rigidbody2D playerRigidbody;
    private Rigidbody2D objToSwapRigidbody;
    private Collider2D playerCollider;
    private Collider2D objToSwapCollider;
    private GameObject playerObjectToSwapLater;
    private GameObject objectToSwapLater;

    [Space(10)]
    [SerializeField] private float selectRange = 1f;
    [SerializeField] private float swapPlayerDuration = 4f;
    [SerializeField] private float abilityCooldown = 4f;
    private float playerTimer;
    private float abilityTimer;

    private bool isSwapped = false;
    private bool isProprietySwapped = false;
    private bool isWaitingToSwap = false;

    private List<MonoBehaviour> firstScripts = new();
    private List<MonoBehaviour> ObjectToSwapScripts = new();
    public List<MonoBehaviour> PlayerScripts { get => FirstScripts1; set => FirstScripts1 = value; }
    public List<MonoBehaviour> FirstScripts1 { get => firstScripts; set => firstScripts = value; }

    [SerializeField] private Material highlightMaterial;
    [SerializeField] private LayerMask swapLayerMask;
    private GameObject highlightedObject;
    private Material originalMaterial;
    private void Start()
    {
        blinkingScript = GetComponent<BlinkingScript>();
        playerScript = movmentManager.GetComponent<PlayerScript>();
        playerMaterial = playerObject.GetComponent<Renderer>().material;
        materialSwapper = GetComponent<MaterialSwapper>();
        cameraManager = FindAnyObjectByType<CameraManager>();
    }
    private void Update()
    {
        abilityTimer -= Time.deltaTime;
        if (isWaitingToSwap && playerObjectToSwapLater != null && objectToSwapLater != null)
        {
            SwapObjects(playerObjectToSwapLater, objectToSwapLater);
            playerObjectToSwapLater = null;
            objectToSwapLater = null;
            isWaitingToSwap = false;
        }

        if (isSwapped)
        {
            if(playerTimer < 0f || Input.GetMouseButtonDown(1))
            {
                ResetSwapPlayerObject();
                playerTimer = swapPlayerDuration;
                isSwapped = false;
            }
            else
            {
                playerTimer -= Time.deltaTime;
            }
            return;
        }

        //Checking if there is a gameobject that is swappable in the range of the mouse
        Collider2D hit = null;
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = mainCamera.nearClipPlane;
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Collider2D[] hits = Physics2D.OverlapCircleAll(mouseWorldPosition, selectRange, swapLayerMask);

        float minDistance = Mathf.Infinity;
        foreach (Collider2D hitObject in hits)
        {
            float distance = Vector2.Distance(hitObject.transform.position, mouseWorldPosition);
            if (distance < minDistance)
            {
                hit = hitObject;
                minDistance = distance;
            }
        }
        if (abilityTimer > 0f)
        {
            return;
        }
        if (hit)
        {
            HighlightObject(hit.gameObject);
        }
        if(Input.GetMouseButtonUp(0))
        {
            RemoveHighlight();
            playerTimer = swapPlayerDuration;

            //attempting to swap the player with the objectToSwap if they exist
            if (hit != null && playerObject != hit.gameObject)
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
                SwapObjects(playerObject, hit.gameObject);
                abilityTimer = abilityCooldown;
            }
        }
        else if(!Input.GetMouseButton(0) && !hit)
        {
            RemoveHighlight();
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
    public void SwapObjects(GameObject playerObj, GameObject objToSwap)
    {
        if (playerObj == objToSwap)
        {
            Debug.LogWarning("can't swap the object with itself");
            return;
        }
        if (playerObj == null || objToSwap == null)
        {
            Debug.LogWarning("one of the selected objects is null");
            playerObjectToSwapLater = playerObj;
            objectToSwapLater = objToSwap;
            isWaitingToSwap = true;
            return;
        }
        if (!isSwapped)
        {
            isSwapped = true;
            playerObjectCopy = playerObject;
            objectToSwap = objToSwap;
            if (objToSwap.CompareTag("Swappable"))
            {
                FocusOn(objToSwap);
            }
            else
            {
                isProprietySwapped = true;
                playerScript._isSwappedPropriety = true;
            }
        }
        SwapRigidbodys(playerObj, objToSwap);
        if (objToSwap.CompareTag("Swappable"))
        {
            Debug.Log("Swaping scripts:");
            SwapScripts(playerObj, objToSwap);
        }
        cancelAbilityObject.SetActive(true);
    }
    void SwapRigidbodys(GameObject playerObj, GameObject objToSwap)
    {
        playerRigidbody = playerObj.GetComponent<Rigidbody2D>();
        objToSwapRigidbody = objToSwap.GetComponent<Rigidbody2D>();
        playerCollider = playerObj.GetComponent<Collider2D>();
        objToSwapCollider = objToSwap.GetComponent<Collider2D>();


        if (playerRigidbody == null || objToSwapRigidbody == null)
        {
            Debug.LogWarning("one of the gameobjects you want to swap doesn't have a rigidbody");
            return;
        }

        //Swap the gravityScales and the masses
        (objToSwapRigidbody.gravityScale, playerRigidbody.gravityScale) = (playerRigidbody.gravityScale, objToSwapRigidbody.gravityScale);
        (objToSwapRigidbody.mass, playerRigidbody.mass) = (playerRigidbody.mass, objToSwapRigidbody.mass);

        //Swap the physics materials
        PhysicsMaterial2D playerPMat = playerCollider.sharedMaterial;
        PhysicsMaterial2D objToSwapPMat = objToSwapCollider.sharedMaterial;
        if (playerPMat != null && objToSwapPMat != null)
        {
            //Swap the friction values and update the materials
            (objToSwapPMat.friction, playerPMat.friction) = (playerPMat.friction, objToSwapPMat.friction);
            playerCollider.sharedMaterial = playerPMat;
            objToSwapCollider.sharedMaterial = objToSwapPMat;
            Debug.Log("Swapped materials");
        }

        Debug.Log("Swapped rigidbodys");
    }

    void SwapScripts(GameObject playerObject, GameObject objectToSwap)
    {
        //Get all the scripts for the player and for the objectToSwap
        GetScriptsToSwap(playerObject, PlayerScripts);
        GetScriptsToSwap(objectToSwap, ObjectToSwapScripts);
        PlayerScripts = PlayerScripts.Distinct().ToList();
        ObjectToSwapScripts = ObjectToSwapScripts.Distinct().ToList();

        //Remove all the scripts that are double or disabled
        PlayerScripts.RemoveAll(s => s == null || s.enabled == false);
        ObjectToSwapScripts.RemoveAll(s => s == null || s.enabled == false);

        //Disable all the scripts
        foreach (MonoBehaviour script in PlayerScripts)
        {
            script.enabled = false;
        }
        foreach (MonoBehaviour script in ObjectToSwapScripts)
        {
            script.enabled = false;
        }

        //Copy the objectToSwap scripts in the player list
        foreach (MonoBehaviour script in PlayerScripts)
        {
            MonoBehaviour newScript = objectToSwap.AddComponent(script.GetType()) as MonoBehaviour;
            CopyComponent(script, newScript);
            newScript.enabled = true;
        }
        //Copy the player scripts in the objectToSwap list
        foreach (MonoBehaviour script in ObjectToSwapScripts)
        {
            MonoBehaviour newScript = playerObject.AddComponent(script.GetType()) as MonoBehaviour;
            CopyComponent(script, newScript);
            newScript.enabled = true;
        }
        //Remove all the initial player scripts
        MonoBehaviour[] allFirstScripts = playerObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allFirstScripts)
        {
            if (script is SwapAbility || script.enabled || PlayerScripts.IndexOf(script) != -1)
            {
                continue;
            }
            Destroy(script);
        }
        //Remove all the initial object scripts
        MonoBehaviour[] allSecondScripts = objectToSwap.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allSecondScripts)
        {
            if (script is not SwapAbility && !script.enabled && ObjectToSwapScripts.IndexOf(script) == -1)
            {
                Destroy(script);
            }
        }
        Debug.Log("Swapped scritps");
    }

    public void ResetSwapPlayerObject()
    {
        playerMaterial.SetFloat("_OutlineAlpha", 0f);
        if (abilityParticleSystem != null)
        {
            abilityParticleSystem.Stop();
        }
        if (materialSwapper.swapped == true)
        {
            materialSwapper.EndSwapMaterials();
        }
        SwapObjects(playerObjectCopy, objectToSwap);
        Debug.Log("changed playerObject to " + playerObjectCopy);

        if (!isProprietySwapped) 
        {
            FocusOn(playerObjectCopy);
        }

        cancelAbilityObject.SetActive(false);
        swapEndSound.Play();
        blinkingScript.StopAbility();
        playerScript._isSwappedPropriety = false;
        playerTimer = swapPlayerDuration;
        isSwapped = false;
        isProprietySwapped = false;
    }
    private void FocusOn(GameObject target)
    {
        Debug.Log("focusing on: " + target.name);
        cameraManager.FocusOnSelectedObject(target.transform);
        blinkingScript.ChangePlayerObjectTo(target);
        playerScript.ChangePlayerObjectTo(target);
        Debug.Log("changed playerObject to " + target);
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
        //Resets the materials if the application quits while swapped.
        if(isSwapped) {
            PhysicsMaterial2D object1Material = playerCollider.sharedMaterial;
            PhysicsMaterial2D object2Material = objToSwapCollider.sharedMaterial;

            if (object1Material != null && object2Material != null)
            {
                (object2Material.friction, object1Material.friction) = (object1Material.friction, object2Material.friction);
                playerCollider.sharedMaterial = object1Material;
                objToSwapCollider.sharedMaterial = object2Material;
            }
        }
    }
}