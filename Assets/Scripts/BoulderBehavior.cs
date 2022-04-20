using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehavior : MonoBehaviour
{
    public Vector2Int gridPos;
    public bool isImmovable;
    public bool isActive = true;
    public GameManager gm;

    private Collider2D col;
    private SpriteRenderer sr;
    private Color defaultColor;
    private Vector2Int defaultPos;
    private GameObject player;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        defaultColor = sr.color;
        defaultPos = gridPos;
    }

    private void Update()
    {
        ActiveStatus(isActive);
    }

    private void ActiveStatus(bool isActive)
    {
        if (!col.IsTouching(player.GetComponent<BoxCollider2D>())) {
            col.isTrigger = !isActive;
            sr.color = isActive ? defaultColor : Color.blue;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isImmovable && col.gameObject.tag == "Player")
        {
            Rigidbody2D rb = this.GetComponent<Rigidbody2D>();

            Vector3 hit = col.contacts[0].normal;
            Debug.Log(hit);
            float angle = Vector3.Angle(hit, Vector3.up);

            if (Mathf.Approximately(angle, 0))
            {
                bool hasChanged = false;
                transform.position = gm.AttemptMoveBoulder(gridPos, gridPos + new Vector2Int(0, 1), this.transform.position, out hasChanged);
                if (hasChanged) {
                    gridPos += new Vector2Int(0, 1);
                    gm.SetAdjacentBouldersActive(gridPos.x, gridPos.y, false);
                }
            }
            if (Mathf.Approximately(angle, 180))
            {
                bool hasChanged = false;
                transform.position = gm.AttemptMoveBoulder(gridPos, gridPos + new Vector2Int(0, -1), this.transform.position, out hasChanged);
                if (hasChanged) {
                    gridPos += new Vector2Int(0, -1);
                    gm.SetAdjacentBouldersActive(gridPos.x, gridPos.y, false);
                }
            }
            if (Mathf.Approximately(angle, 90))
            {
                Vector3 cross = Vector3.Cross(Vector3.forward, hit);
                if (cross.y > 0)
                {
                    bool hasChanged = false;
                    transform.position = gm.AttemptMoveBoulder(gridPos, gridPos + new Vector2Int(1, 0), this.transform.position, out hasChanged);
                    if (hasChanged) {
                        gridPos += new Vector2Int(1, 0);
                        gm.SetAdjacentBouldersActive(gridPos.x, gridPos.y, false);
                    }
                }
                else
                {
                    bool hasChanged = false;
                    transform.position = gm.AttemptMoveBoulder(gridPos, gridPos + new Vector2Int(-1, 0), this.transform.position, out hasChanged);
                    if (hasChanged) {
                        gridPos += new Vector2Int(-1, 0);
                        gm.SetAdjacentBouldersActive(gridPos.x, gridPos.y, false);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && !isImmovable)
        {
            gm.SetAdjacentBouldersActive(gridPos.x, gridPos.y, true);
            bool hasChanged = false;
            transform.position = gm.AttemptMoveBoulder(gridPos, defaultPos, this.transform.position, out hasChanged);
            gridPos = defaultPos;
        }
    }
}
