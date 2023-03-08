using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class TestScript1 : MonoBehaviour
{
    public float value = 3;
    public string s = "wowowow";
    public bool debug = false;
    private float timer = 2f;
    private float counterTimer;
    private float jump = 3f;
    private float jumpTimer;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (jumpTimer <= 0f)
        {
            jumpTimer = jump;
            rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        }
        else
        {
            jumpTimer -= Time.deltaTime;
        }

        if (!debug)
            return;
        if (counterTimer <= 0f)
        {
            Debug.Log(gameObject.name + "-> script 1");
            counterTimer = timer;
        }
        else
        {
            counterTimer -= Time.deltaTime;
        }
    }
}
