using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleSpawner : MonoBehaviour
{
    public ParticleSystem my_particleSystem;
    public GameObject particlePrefab;

    private GameObject[] particles;
    private float originalGravityScale;

    void Start()
    {
        // Initialize array to hold gameobjects
        particles = new GameObject[my_particleSystem.main.maxParticles];

        // Spawn gameobjects to match the particle system
        for (int i = 0; i < my_particleSystem.main.maxParticles; i++)
        {
            GameObject particle = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity, transform);
            particle.SetActive(false);
            particles[i] = particle;
            originalGravityScale = particle.GetComponent<Rigidbody2D>().gravityScale;
        }
        
    }

    void Update()
    {
        // Get current state of particle system
        ParticleSystem.Particle[] systemParticles = new ParticleSystem.Particle[my_particleSystem.main.maxParticles];
        int numParticles = my_particleSystem.GetParticles(systemParticles);

        // Update gameobject positions and velocities to match particle system
        for (int i = 0; i < numParticles; i++)
        {
            GameObject particle = particles[i];
            ParticleSystem.Particle systemParticle = systemParticles[i];
            particle.SetActive(true);

            // Transform the position of the particle from local to world space
            Vector3 particlePosition = my_particleSystem.transform.TransformPoint(systemParticle.position);
            particle.transform.position = particlePosition;

            particle.GetComponent<Rigidbody2D>().velocity = systemParticle.velocity;
        }

        // Disable or reset any remaining gameobjects
        for (int i = numParticles; i < my_particleSystem.main.maxParticles; i++)
        {
            GameObject particle = particles[i];
            if (particle.activeSelf)
            {
                ResetParticle(particle);
            }
            particle.SetActive(false);
        }
    }

    void ResetParticle(GameObject particle)
    {
        // Reset particle to its original prefab state
        particle.transform.position = Vector3.zero;
        particle.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        particle.GetComponent<Rigidbody2D>().gravityScale = originalGravityScale;
        // Add additional reset code here as needed
    }
}
