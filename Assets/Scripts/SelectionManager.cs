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

    private ObjectData firstSelectedObjectData;
    private ObjectData secondSelectedObjectData;
    private ObjectManager firstSelectedObjectManager;
    private ObjectManager secondSelectedObjectManager;

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

        if (firstSelectedObjectManager == null || firstSelectedObjectData == null)
        {
            firstSelectedObjectManager = hit.transform.gameObject.GetComponent<ObjectManager>();

            firstSelectedObjectData = firstSelectedObjectManager.objectData;
            if (firstSelectedObjectData != null)
            {
                Debug.Log("Selected first data from: " + firstSelectedObjectData.objectName);
            }
        }
        else if(secondSelectedObjectManager == null || secondSelectedObjectData)
        {
            secondSelectedObjectManager = hit.transform.gameObject.GetComponent<ObjectManager>();
            secondSelectedObjectData = secondSelectedObjectManager.objectData;
            if (secondSelectedObjectData == null)
            {
                return;
            }

            Debug.Log("Swaping data");
            var aux = firstSelectedObjectData;
            firstSelectedObjectManager.objectData = secondSelectedObjectData;
            secondSelectedObjectManager.objectData = aux;
            firstSelectedObjectManager.RefreshData();
            secondSelectedObjectManager.RefreshData();

            firstSelectedObjectManager = null;
            secondSelectedObjectManager = null;
            firstSelectedObjectData = null;
            secondSelectedObjectData = null;

        }
    }
}
