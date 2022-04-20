using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    public Rigidbody2D rb;

    Vector2 movement;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            RestartLevel();
        }
    }

    void FixedUpdate()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        rb.velocity = movement * speed;
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
