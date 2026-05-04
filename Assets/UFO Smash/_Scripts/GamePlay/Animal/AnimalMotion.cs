using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMotion : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 distanceTravelled = (Vector2)transform.right * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + distanceTravelled);
    }
}
