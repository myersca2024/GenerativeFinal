using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TeleportBehavior : MonoBehaviour
{
    public Transform toTeleport;
    public int startingScore;

    private GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.position = toTeleport.position;
            if (collision.gameObject.GetComponent<PlayerItemsBehavior>().items > startingScore)
            {
                Destroy(toTeleport.gameObject);
            }
            gm.ClearGridPuzzle();
        }
    }
}
