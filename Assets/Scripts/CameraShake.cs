using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public float deathShakeDuration = 0.15f;
    public float deathShakeAmplitude = 2f;
    public float deathShakeFrequency = 2.0f;
    public float runShakeDuration = 0.15f;
    public float runShakeAmplitude = 2f;
    public float runShakeFrequency = 1.0f;

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
        if(shake==false)
        {
            if (PlayerScript._isActive == false)
            {
                Debug.Log("shake started");
                ShakeDeath();
                shake = true;
            }
            //else if(PlayerScript._isRunning == true)
            //{
            //    ShakeRun();
            //    shake = true;
            //}
        }
    }
    private void ShakeDeath()
    {
        noise.m_AmplitudeGain = deathShakeAmplitude;
        noise.m_FrequencyGain = deathShakeFrequency;
        Invoke(nameof(StopShaking), deathShakeDuration);
    }

    private void ShakeRun()
    {
        noise.m_AmplitudeGain = runShakeAmplitude;
        noise.m_FrequencyGain = runShakeFrequency;
        Invoke(nameof(StopShaking), runShakeDuration);
    }

    void StopShaking()
    {
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
        shake = false;
    }
}