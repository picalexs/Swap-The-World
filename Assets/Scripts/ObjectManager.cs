using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ObjectManager : MonoBehaviour
{
    public ObjectData objectData;
    private TextMeshPro _textMeshPro;
    private SpriteRenderer _sprite;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
        _sprite = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        RefreshData();
    }

    public void RefreshData()
    {
        _textMeshPro.text = objectData.objectDescription;
        _sprite.color = objectData.objectColor;
        _rigidbody.mass= objectData.objectMass;
        _rigidbody.gravityScale= objectData.objectGravityScale;
    }
}
