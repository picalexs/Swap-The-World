using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ObjectManager : MonoBehaviour
{
    private TextMeshPro _textMeshPro;
    private Rigidbody2D _rigidbody;
    private GameObject _gameObject;
    [SerializeField] private ObjectData objectData;
    public ObjectData _objectData
    {
        get => objectData;
        set
        {
            objectData = value;
            Destroy(_gameObject);
            GameObject newContent = Instantiate(objectData.RuntimeGameObject, this.transform);
            _gameObject = newContent;
        }
    }
    private void Awake()
    {
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _gameObject = GetComponent<GameObject>();
    }

    
    private void Start()
    {
       // RefreshData();
    }

    public void RefreshData()
    {
        _textMeshPro.text = _objectData.RuntimeObjectDescription;
        _rigidbody.mass= _objectData.RuntimeObjectMass;
        _rigidbody.gravityScale= _objectData.RuntimeObjectGravityScale;
    }
}
