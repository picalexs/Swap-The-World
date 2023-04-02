using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MaterialSwapper : MonoBehaviour
{
    public Material swapMaterial;
    public bool swaped = false;

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();

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
            if (spriteRenderer.CompareTag("Swapable")) continue; // skip swapable objects
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
        swaped = true;
        foreach (KeyValuePair<Renderer, Material> entry in originalMaterials)
        {
            entry.Key.material = swapMaterial;
        }
    }
    public void EndSwapMaterials()
    {
        swaped = false;
        foreach (KeyValuePair<Renderer, Material> entry in originalMaterials)
        {
            entry.Key.material = entry.Value;
        }
    }
}
