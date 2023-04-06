using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(ParticleSystem))]
public class AttachGameObjectsToParticles : MonoBehaviour
{
    public Light2D lightPrefab;
    [SerializeField] private float lightIntensity;
    [SerializeField] private float endTime;

    private ParticleSystem m_ParticleSystem;
    private List<Light2D> particleLights = new();
    private ParticleSystem.Particle[] m_Particles;

    // Start is called before the first frame update
    void Start()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
    }

    // Update is called once per frame
    void LateUpdate()
    {
        int count = m_ParticleSystem.GetParticles(m_Particles);

        for (int i = particleLights.Count; i < count; i++)
        {
            Light2D light = Instantiate(lightPrefab, m_ParticleSystem.transform);
            particleLights.Add(light);
        }

        bool worldSpace = (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);
        for (int i = 0; i < particleLights.Count; i++)
        {
            if (i < count)
            {
                if (worldSpace)
                    particleLights[i].transform.position = m_Particles[i].position;
                else
                    particleLights[i].transform.localPosition = m_Particles[i].position;

                if (m_Particles[i].remainingLifetime <= endTime)
                {
                    float ratio = 1f - m_Particles[i].remainingLifetime / endTime;
                    particleLights[i].intensity = Mathf.Lerp(lightIntensity, 0f, ratio);
                }
                else
                {
                    particleLights[i].intensity = lightIntensity;
                }

                if (m_Particles[i].remainingLifetime <= 0f)
                {
                    particleLights[i].enabled = false;
                    Destroy(particleLights[i]);
                    particleLights.RemoveAt(i);
                }
                else
                {
                    particleLights[i].enabled = true;
                }
            }
            else
            {
                particleLights[i].enabled = false;
            }
        }
    }
}
