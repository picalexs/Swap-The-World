using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnObjectPrefab;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float objectLifetime = 5f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private Vector3 spawnRandomness = Vector3.zero;
    [SerializeField] private AnimationCurve sizeOverLifetime = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [SerializeField] private Gradient colorOverLifetime;

    private float timer = 0f;
    private List<Rigidbody2D> spawnedObjects = new();

    void Start()
    {
        // Pre-warm the object pool
        for (int i = 0; i < 10; i++)
        {
            SpawnObject();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 1f / spawnRate)
        {
            timer = 0f;
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        Vector3 spawnPos = transform.position + spawnOffset + new Vector3(Random.Range(-spawnRandomness.x, spawnRandomness.x), Random.Range(-spawnRandomness.y, spawnRandomness.y), Random.Range(-spawnRandomness.z, spawnRandomness.z));
        GameObject newObj = Instantiate(spawnObjectPrefab, spawnPos, Quaternion.identity, transform);
        Rigidbody2D rb = newObj.GetComponent<Rigidbody2D>();
        spawnedObjects.Add(rb);

        if (rb != null)
        {
            rb.velocity = transform.forward * Random.Range(minSpeed, maxSpeed);
        }

        if (newObj.TryGetComponent<Renderer>(out var objectRenderer))
        {
            float startSize = objectRenderer.transform.localScale.x;
            objectRenderer.transform.localScale = sizeOverLifetime.Evaluate(0f) * startSize * Vector3.one;

            IEnumerator SizeOverLifetimeCoroutine()
            {
                float elapsedTime = 0f;
                while (elapsedTime < objectLifetime)
                {
                    float t = elapsedTime / objectLifetime;
                    objectRenderer.transform.localScale = sizeOverLifetime.Evaluate(t) * startSize * Vector3.one;

                    Color currentColor = colorOverLifetime.Evaluate(t);
                    currentColor.a = t * (1 - t) * 4; 
                    objectRenderer.material.color = currentColor;

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                spawnedObjects.Remove(rb);
                Destroy(newObj);
            }
            StartCoroutine(SizeOverLifetimeCoroutine());
        }
        else
        {
            Destroy(newObj, objectLifetime);
        }
    }

    private void OnDisable()
    {
        foreach (Rigidbody2D rb in spawnedObjects)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnEnable()
    {
        foreach (Rigidbody2D rb in spawnedObjects)
        {
            rb.velocity = transform.forward * Random.Range(minSpeed, maxSpeed);
        }
    }
}
