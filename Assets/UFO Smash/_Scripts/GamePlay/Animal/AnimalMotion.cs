using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMotion : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool isAbducted = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isAbducted) return;
        Vector2 distanceTravelled = (Vector2)transform.right * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + distanceTravelled);
    }
    public void SetAbduct() => isAbducted = true;
}
