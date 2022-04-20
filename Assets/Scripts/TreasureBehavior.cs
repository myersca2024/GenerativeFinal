using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBehavior : MonoBehaviour
{
    public Vector2Int gridPos;
    public GameManager gm;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerItemsBehavior>().AddItem();
            gm.SetGridSpace(gridPos.x, gridPos.y, 0);
            Destroy(this.gameObject);
        }
    }
}
