using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript2 : MonoBehaviour
{
    public Vector2 vector = new Vector2(1f, 1f);
    public float value = 10;
    public string s = "157109741";
    public bool debug = false;
    private float timer = 2f;
    private float counterTimer;
    private float move = 4f;
    private float moveTimer;
    private Rigidbody2D rb;
    private float f = -1,dir=1;
    private void Start()
    {
        rb= GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (moveTimer <= 0f)
        {
            moveTimer = move;
            dir *= f;
            rb.AddForce(Vector2.left* dir * 3f, ForceMode2D.Impulse);
        }
        else
        {
            moveTimer -= Time.deltaTime;
        }
        if (!debug)
            return;
        if (counterTimer <= 0f)
        {
            Debug.Log(gameObject.name + "-> script 2");
            counterTimer = timer;
        }
        else
        {
            counterTimer -= Time.deltaTime;
        }
    }
}
