using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    public Rigidbody2D rb;

    Vector2 movement;

    void FixedUpdate()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
