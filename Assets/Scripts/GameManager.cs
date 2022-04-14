using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] OverlapWFC WFCOutput;
    public int size = 20;
    public GameObject missionLocation;
    public GameObject spawnLocation;
    public GameObject playerReference;
    public GameObject gameWinMessage;
    public BoulderBehavior boulderObject;
    public int totalMissions;
    public int maxIterations;

    private Transform generatedObject;
    private Transform[,] grid;
    private List<Tuple<int, char>> partitions = new List<Tuple<int, char>>();
    private Vector3 offset = new Vector3(0.5f, -0.5f, 0);
    private bool mapGenerated = false;
    private bool missionsGenerated = false;

    private bool[,] boulderGrid = new bool[8, 8];
    private List<BoulderBehavior> boulders = new List<BoulderBehavior>();

    void Start()
    {
        grid = new Transform[size - 1, size - 1];
        StartCoroutine(GenerateWorld());
    }

    void Update()
    {
        if (generatedObject == null) { generatedObject = WFCOutput.transform.GetChild(0).GetChild(0); }
    }

    void PopulateGrid()
    {
        List<Transform> tiles = new List<Transform>();
        for (int i = 0; i < generatedObject.childCount; i++)
        {
            Transform child = generatedObject.GetChild(i);
            tiles.Add(child);
            // Debug.Log(child.localPosition);
        }
        foreach (Transform child in tiles)
        {
            Vector3 childPos = child.localPosition;
            grid[Mathf.RoundToInt(childPos.x), Mathf.RoundToInt(childPos.y)] = child;
        }
    }

    IEnumerator GenerateWorld()
    {
        while (generatedObject == null) { generatedObject = WFCOutput.transform.GetChild(0).GetChild(0); }
        while (generatedObject != null && generatedObject.childCount == 0)
        {
            if (generatedObject == null) { generatedObject = WFCOutput.transform.GetChild(0).GetChild(0); }
            WFCOutput.Generate();
            WFCOutput.Run();
            yield return new WaitForSeconds(0.25f);
        }

        mapGenerated = true;
        PopulateGrid();
        GenerateMissions();
        GenerateSpawn();
    }

    void GenerateMissions()
    {
        for (int i = 0; i < 2; i++)
        {
            int part_coor = UnityEngine.Random.Range(1, size - 2);
            Tuple<int, char> partition = i % 2 == 0 ? new Tuple<int, char>(part_coor, 'x') : new Tuple<int, char>(part_coor, 'y');
            partitions.Add(partition);
        }

        // Generate top-left location
        InstantiateCave(0, partitions[0].Item1, 0, partitions[1].Item1);

        // Generate top-right location
        InstantiateCave(partitions[0].Item1, size - 1, 0, partitions[1].Item1);

        // Generate bottom-left location
        InstantiateCave(0, partitions[0].Item1, partitions[1].Item1, size - 1);

        // Generate bottom-right location
        InstantiateCave(partitions[0].Item1, size - 1, partitions[1].Item1, size - 1);
    }

    void GenerateSpawn()
    {
        int currIterations = maxIterations;
        Transform positionToSpawn = null;
        while (positionToSpawn == null && currIterations > 0)
        {
            int coor_x = UnityEngine.Random.Range(0, size - 1);
            int coor_y = UnityEngine.Random.Range(0, size - 1);
            if (grid[coor_x, coor_y].gameObject.layer != 6)
            {
                positionToSpawn = grid[coor_x, coor_y];
            }
            currIterations--;
        }
        if (currIterations <= 0)
        {
            Debug.Log("Failed to generate cave in valid area within given iterations.");
            return;
        }
        GameObject spawn = Instantiate(spawnLocation, positionToSpawn.position + offset, this.transform.localRotation);
        playerReference.transform.position = spawn.transform.position;
    }

    void InstantiateCave(int xMin, int xMax, int yMin, int yMax)
    {
        int currIterations = maxIterations;
        Transform positionToSpawn = null;
        while (positionToSpawn == null && currIterations > 0)
        {
            int coor_x = UnityEngine.Random.Range(xMin, xMax);
            int coor_y = UnityEngine.Random.Range(yMin, yMax);
            if (grid[coor_x, coor_y].gameObject.layer != 6)
            {
                positionToSpawn = grid[coor_x, coor_y];
            }
            currIterations--;
        }
        if (currIterations <= 0) {
            Debug.Log("Failed to generate cave in valid area within given iterations.");
            return;
        }
        Instantiate(missionLocation, positionToSpawn.position + offset, this.transform.localRotation);
    }

    public void GameWon()
    {
        gameWinMessage.SetActive(true);
    }

    public void GenerateGridPuzzle(bool[,] pattern)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pattern[i, j])
                {
                    BoulderBehavior boulder = Instantiate(boulderObject, GridSpaceToWorldPosition(i, j), this.transform.localRotation);
                    boulder.gridPos = new Vector2Int(i, j);
                    boulder.gm = this;
                    boulderGrid[i, j] = true;
                    boulders.Add(boulder);
                }
            }
        }
    }

    public void ClearGridPuzzle()
    {
        List<BoulderBehavior> toRemove = new List<BoulderBehavior>();
        foreach (BoulderBehavior boulder in boulders)
        {
            boulderGrid[boulder.gridPos.x, boulder.gridPos.y] = false;
            toRemove.Add(boulder);
        }

        foreach (BoulderBehavior boulder in toRemove)
        {
            boulders.Remove(boulder);
            Destroy(boulder.gameObject, 0.1f);
        }
    }

    public Vector3 AttemptMoveBoulder(Vector2Int fromPos, Vector2Int toPos, Vector3 boulderPos, out bool hasChanged)
    {
        int fromX = fromPos.x;
        int fromY = fromPos.y;
        int toX = toPos.x;
        int toY = toPos.y;
        if (toX < 0 || toX > 7 || toY < 0 || toY > 7)
        {
            hasChanged = false;
            return boulderPos;
        }
        if (!boulderGrid[toX, toY])
        {
            boulderGrid[fromX, fromY] = false;
            boulderGrid[toX, toY] = true;
            hasChanged = true;
            return (boulderPos + new Vector3(toPos.x - fromPos.x, toPos.y - fromPos.y, 0));
        }
        else
        {
            hasChanged = false;
            return boulderPos;
        }
    }

    /*
    public Vector2Int WorldPositionToGridSpace(Vector3 position)
    {

    }
    */

    public Vector3 GridSpaceToWorldPosition(int x, int y)
    {
        Vector3 offset = new Vector3(0.5f, -0.5f, 0);
        Vector3 bottomCorner = new Vector3(-10.608703f, 8.128969f, 0);
        return bottomCorner + new Vector3(x, y, 0) + offset;
    }

    void PrintGrid()
    {
        string output = "";
        for (int i = 0; i < size - 1; i++)
        {
            for (int j = 0; j < size - 1; j++)
            {
                string coords = grid[i, j] == null ? "" : grid[i, j].localPosition.ToString();
                output += "[" + coords + "]";
            }
            output +="\n";
        }
        Debug.Log(output);
    }
}
