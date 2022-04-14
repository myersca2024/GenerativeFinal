using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TeleportBehavior : MonoBehaviour
{
    public Transform toTeleport;
    public CinemachineConfiner confiner;
    public Collider2D setCollider;

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
            confiner.m_BoundingShape2D = setCollider;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItemsBehavior>().AddItem();
            gm.ClearGridPuzzle();
        }
    }
}
