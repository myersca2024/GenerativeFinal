using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBehavior : MonoBehaviour
{
    public float interactionDistance = 2f;

    private GameManager gm;
    private PlayerItemsBehavior playerReference;

    void Start()
    {
        playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItemsBehavior>();
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Vector3.Distance(playerReference.transform.position, this.transform.position) <= interactionDistance && playerReference.items >= 4 && Input.GetKeyDown(KeyCode.E))
        {
            gm.GameWon();
        }
    }
}
