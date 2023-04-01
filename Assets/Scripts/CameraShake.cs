using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.15f;
    public float shakeAmplitude = 2f;
    public float shakeFrequency = 1.0f;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    bool shake = false;
    private void Update()
    {
        if(shake==false && PlayerScript._isActive == false)
        {
            Debug.Log("shake started");
            Shake();
            shake = true;
        }
    }
    public void Shake()
    {
        noise.m_AmplitudeGain = shakeAmplitude;
        noise.m_FrequencyGain = shakeFrequency;
        Invoke("StopShaking", shakeDuration);
    }

    void StopShaking()
    {
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
        shake = false;
    }
}