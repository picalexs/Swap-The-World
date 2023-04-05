using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager: MonoBehaviour
{
    public LayerMask roomLayer;
    [SerializeField] private CinemachineVirtualCamera[] cameras;
    private CinemachineVirtualCamera currentCamera;
    [SerializeField] private GameObject player;

    void Start()
    {
        currentCamera = GetActiveCamera();
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, Mathf.Infinity, roomLayer);
        if (hit.collider != null && hit.collider.CompareTag("Room"))
        {
            CinemachineVirtualCamera roomCamera = hit.collider.GetComponentInChildren<CinemachineVirtualCamera>();
            if (roomCamera != null && roomCamera != currentCamera)
            {
                Debug.Log("roomCamer: "+ roomCamera.name);
                currentCamera.gameObject.SetActive(false);
                currentCamera = roomCamera;
                currentCamera.gameObject.SetActive(true);
            }
        }
    }
    public void FocusOnSelectedObject (Transform selectedObject)
    {
        Debug.Log(currentCamera.name +" follows "+ selectedObject.name);
        currentCamera.Follow = selectedObject.transform;
    }

    private CinemachineVirtualCamera GetActiveCamera()
    {
        Debug.Log("cameras: " + cameras.Length);
        foreach (CinemachineVirtualCamera camera in cameras)
        {
            Debug.Log("camera:" + camera.name);
            if (camera.gameObject.activeSelf && camera.transform.parent.CompareTag("Room"))
            {
                return camera;
            }
        }
        return null;
    }
}
