using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CaveBehavior : MonoBehaviour
{
    public float interactionDistance = 2f;
    public GameObject caveRoom;

    private CinemachineConfiner confiner;
    private GameObject playerReference;
    private GameObject puzzleArea;
    private Collider2D worldBoundingBox;
    private Collider2D puzzleBoundingBox;
    private TeleportBehavior teleporter;
    private GameManager gm;

    void Start()
    {
        playerReference = GameObject.FindGameObjectWithTag("Player");
        puzzleArea = GameObject.FindGameObjectWithTag("PuzzleArea");
        confiner = FindObjectOfType<CinemachineConfiner>();
        worldBoundingBox = confiner.m_BoundingShape2D;
        puzzleBoundingBox = GameObject.FindGameObjectWithTag("BoundingBox").GetComponent<PolygonCollider2D>();
        teleporter = GameObject.FindGameObjectWithTag("Teleporter").GetComponent<TeleportBehavior>();
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Vector3.Distance(playerReference.transform.position, this.transform.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            playerReference.transform.position = puzzleArea.transform.position;
            confiner.enabled = false;
            teleporter.toTeleport = this.transform;
            teleporter.confiner = this.confiner;
            teleporter.setCollider = this.worldBoundingBox;
            bool[,] pattern = { { false, false, false, false, true, false, false, false},
                                { false, false, false, false, true, false, false, false},
                                { false, false, false, false, true, false, false, false},
                                { false, false, false, false, true, false, false, false},
                                { false, false, false, false, true, false, false, false},
                                { false, false, false, false, true, false, false, false},
                                { false, false, false, false, true, false, false, false},
                                { false, false, false, false, true, false, false, false},
                                };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bool randBoulder = Random.Range(0, 4) == 0 ? true : false;
                    pattern[i, j] = randBoulder;
                }
            }

            gm.GenerateGridPuzzle(pattern);
            // puzzleBoundingBox.gameObject.transform.parent.position += new Vector3(0, 0, -65);
        }
    }
}
