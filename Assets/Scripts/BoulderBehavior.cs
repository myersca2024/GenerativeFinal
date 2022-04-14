using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehavior : MonoBehaviour
{
    public Vector2Int gridPos;
    public GameManager gm;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Rigidbody2D rb = this.GetComponent<Rigidbody2D>();

            Vector3 hit = col.contacts[0].normal;
            Debug.Log(hit);
            float angle = Vector3.Angle(hit, Vector3.up);

            if (Mathf.Approximately(angle, 0))
            {
                //Down
                Debug.Log("Down");
                //transform.position += new Vector3(0, 1, 0);
                bool hasChanged = false;
                transform.position = gm.AttemptMoveBoulder(gridPos, gridPos + new Vector2Int(0, 1), this.transform.position, out hasChanged);
                if (hasChanged) { gridPos += new Vector2Int(0, 1); }
            }
            if (Mathf.Approximately(angle, 180))
            {
                //Up
                Debug.Log("Up");
                //transform.position += new Vector3(0, -1, 0);
                bool hasChanged = false;
                transform.position = gm.AttemptMoveBoulder(gridPos, gridPos + new Vector2Int(0, -1), this.transform.position, out hasChanged);
                if (hasChanged) { gridPos += new Vector2Int(0, -1); }
            }
            if (Mathf.Approximately(angle, 90))
            {
                // Sides
                Vector3 cross = Vector3.Cross(Vector3.forward, hit);
                if (cross.y > 0)
                { // left side of the player
                    Debug.Log("Left");
                    //transform.position += new Vector3(1, 0, 0);
                    bool hasChanged = false;
                    transform.position = gm.AttemptMoveBoulder(gridPos, gridPos + new Vector2Int(1, 0), this.transform.position, out hasChanged);
                    if (hasChanged) { gridPos += new Vector2Int(1, 0); }
                }
                else
                { // right side of the player
                    Debug.Log("Right");
                    //transform.position += new Vector3(-1, 0, 0);
                    bool hasChanged = false;
                    transform.position = gm.AttemptMoveBoulder(gridPos, gridPos + new Vector2Int(-1, 0), this.transform.position, out hasChanged);
                    if (hasChanged) { gridPos += new Vector2Int(-1, 0); }
                }
            }
        }
    }
}
