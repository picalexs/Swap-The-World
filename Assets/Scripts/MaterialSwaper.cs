using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MaterialSwapper : MonoBehaviour
{
    public Material swapMaterial;
    public Material hideMaterial;
    public bool swapped = false;

    public GameObject lightPrefab;
    private readonly Dictionary<Renderer, Material> originalMaterials = new();
    private readonly List<GameObject> torchObjects = new();
    private readonly List<GameObject> hideObjects = new();
    private readonly List<GameObject> lightObjects = new();

    void Start()
    {
        UpdateMaterials();
    }

    public void UpdateMaterials()
    {
        SpriteRenderer[] spriteRenderers = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer.CompareTag("Player")) continue; // skip player object
            if (spriteRenderer.CompareTag("Swappable")) continue; // skip swapable objects
            originalMaterials[spriteRenderer] = spriteRenderer.material;
        }

        TilemapRenderer[] tilemapRenderers = FindObjectsOfType<TilemapRenderer>();

        foreach (TilemapRenderer tilemapRenderer in tilemapRenderers)
        {
            originalMaterials[tilemapRenderer] = tilemapRenderer.material;
        }
    }

    public void SwapMaterials()
    {
        swapped = true;
        foreach (KeyValuePair<Renderer, Material> entry in originalMaterials)
        {
            entry.Key.material = swapMaterial;
        }

        GameObject[] torches = GameObject.FindGameObjectsWithTag("Torch");
        foreach (GameObject torch in torches)
        {
            torchObjects.Add(torch);
            torch.SetActive(false);
        }

        GameObject[] hideObjects = GameObject.FindGameObjectsWithTag("HideObject");
        foreach (GameObject hideObject in hideObjects)
        {
            hideObject.SetActive(false);
            this.hideObjects.Add(hideObject);
        }

        GameObject[] swapableObjects = GameObject.FindGameObjectsWithTag("Swappable");
        foreach (GameObject swapableObject in swapableObjects)
        {
            GameObject newLightObject = Instantiate(lightPrefab, swapableObject.transform.position, Quaternion.identity);
            lightObjects.Add(newLightObject);
        }
    }

    public void EndSwapMaterials()
    {
        swapped = false;
        foreach (KeyValuePair<Renderer, Material> entry in originalMaterials)
        {
            entry.Key.material = entry.Value;
        }

        foreach (GameObject torch in torchObjects)
        {
            torch.SetActive(true);
        }

        foreach (GameObject hideObject in hideObjects)
        {
            hideObject.SetActive(true);
        }

        foreach (GameObject lightObject in lightObjects)
        {
            Destroy(lightObject);
        }
    }
}
