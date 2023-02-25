using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ObjectManager : MonoBehaviour
{
    public ObjectData objectData;
    private TextMeshPro _textMeshPro;
    private SpriteRenderer _sprite;

    private void Awake()
    {
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
        _sprite = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        RefreshData();
    }

    public void RefreshData()
    {
        _textMeshPro.text = objectData.objectDescription;
        _sprite.color = objectData.objectColor;
    }
}
