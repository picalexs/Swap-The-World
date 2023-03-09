using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    private PlayerActionControler playerAction;
    bool _isActive = false;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float _detectionCircleRadius = 0.2f;
    [SerializeField] private ObjectData firstObjectData;
    [SerializeField] private ObjectData secondObjectData;
    [SerializeField] private ObjectManager firstObjectManager;
    [SerializeField] private ObjectManager secondObjectManager;

    private void OnEnable()
    {
        playerAction.Enable();
    }
    private void OnDisable()
    {
        playerAction.Disable();
    }
    private void Awake()
    {
        playerAction = new PlayerActionControler();
        playerAction.Player.Ability.started += _ => SearchStarted();
        playerAction.Player.Ability.canceled += _ => SearchCanceled();
    }
    private void SearchStarted()
    {
        _isActive = true;
    }
    private void SearchCanceled()
    {
        _isActive = false;
    }
    private void Update()
    {
        if (_isActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Hover();
            }
        }
    }
    private void Hover()
    {
        //Converting Mouse Pos to 2D (vector2) World Pos
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.CircleCast(rayPos, _detectionCircleRadius, Vector2.zero, 0f);
        if (!hit)
        {
            return;
        }
        var selectedObjectManager = hit.transform.gameObject.GetComponent<ObjectManager>();
        if (firstObjectManager == null)
        {
            firstObjectManager = selectedObjectManager;
            firstObjectData = firstObjectManager._objectData;
            if (firstObjectData != null)
            {
                Debug.Log("Selected first data from: " + firstObjectData.RuntimeObjectName);
            }
        }
        else if (secondObjectManager == null)
        {
            secondObjectManager = selectedObjectManager;
            secondObjectData = secondObjectManager._objectData;
            if (secondObjectData == null || firstObjectData == null)
            {
                return;
            }

            Debug.Log("Swaping data");
            var aux = firstObjectData;
            firstObjectManager._objectData = secondObjectData;
            secondObjectManager._objectData = aux;
            firstObjectManager.RefreshData();
            secondObjectManager.RefreshData();
            
            firstObjectManager = null;
            secondObjectManager = null;
            firstObjectData = null;
            secondObjectData = null;
        }
    }
}
