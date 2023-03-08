using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript3 : MonoBehaviour
{
    public Rigidbody2D rb;
    public float value = 0;
    public string s = "a a a a a";
    public bool debug = false;
    private float timer = 2f;
    private float counterTimer;
    private void Update()
    {
        if (!debug)
            return;
        if (counterTimer <= 0f)
        {
            Debug.Log(gameObject.name + "-> script 3");
            counterTimer = timer;
        }
        else
        {
            counterTimer -= Time.deltaTime;
        }
    }
}
