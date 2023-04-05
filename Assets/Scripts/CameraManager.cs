using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraManager: MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] cameras;
    private CinemachineVirtualCamera currentCamera;
    [SerializeField] private GameObject player;
    public void FocusOnSelectedObject (Transform selectedObject)
    {
        currentCamera = GetActiveCamera();
        if(currentCamera == null)
        {
            Debug.LogWarning("no active virtual camera in the scene!");
            return;
        }
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
