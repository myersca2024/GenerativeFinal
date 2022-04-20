using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CaveBehavior : MonoBehaviour
{
    public float interactionDistance = 2f;
    public GameObject caveRoom;

    // private CinemachineConfiner confiner;
    private GameObject playerReference;
    private GameObject puzzleArea;
    // private Collider2D worldBoundingBox;
    private Collider2D puzzleBoundingBox;
    private TeleportBehavior[] teleporter;
    private GameManager gm;

    void Start()
    {
        playerReference = GameObject.FindGameObjectWithTag("Player");
        puzzleArea = GameObject.FindGameObjectWithTag("PuzzleArea");
        // confiner = FindObjectOfType<CinemachineConfiner>();
        // worldBoundingBox = confiner.m_BoundingShape2D;
        puzzleBoundingBox = GameObject.FindGameObjectWithTag("BoundingBox").GetComponent<PolygonCollider2D>();
        teleporter = FindObjectsOfType<TeleportBehavior>();
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Vector3.Distance(playerReference.transform.position, this.transform.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            playerReference.transform.position = puzzleArea.transform.position;
            // confiner.enabled = false;
            foreach (TeleportBehavior tp in teleporter)
            {
                tp.toTeleport = this.transform;
                tp.startingScore = playerReference.GetComponent<PlayerItemsBehavior>().items;
                // tp.confiner = this.confiner;
            }

            int[,] pattern = { { 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2 },
                               { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                               { 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2 },
                               { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                               { 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 0 },
                               { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                               { 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2 },
                               { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                               { 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2 },
                             };

            // Place random immovable boulders
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (!(i == 4 && (j == 0 || j == 10)) && pattern[i, j] == 0 && gm.CheckRowColumnValidity(i, j, pattern))
                    {
                        pattern[i, j] = Random.Range(0, 5) == 0 ? 2 : 0;
                    }
                }
            }

            // Place random treasure
            List<Vector2Int> openSpaces = new List<Vector2Int>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (pattern[i, j] == 0)
                    {
                        openSpaces.Add(new Vector2Int(i, j));
                    }
                }
            }
            int randomSpace = Random.Range(0, openSpaces.Count);
            pattern[openSpaces[randomSpace].x, openSpaces[randomSpace].y] = 3;

            // Set boulder status in game manager
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    pattern[i, j] = pattern[i, j];
                }
            }

            gm.GenerateGridPuzzle(pattern);
            // puzzleBoundingBox.gameObject.transform.parent.position += new Vector3(0, 0, -65);
        }
    }
}
