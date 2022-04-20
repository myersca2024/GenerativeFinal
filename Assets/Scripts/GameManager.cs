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
    public BoulderBehavior immovableBoulderObject;
    public TreasureBehavior treasureObject;
    public int totalMissions;
    public int maxIterations;

    private Transform generatedObject;
    private Transform[,] grid;
    private List<Tuple<int, char>> partitions = new List<Tuple<int, char>>();
    private Vector3 offset = new Vector3(0.5f, -0.5f, 0);
    private bool mapGenerated = false;
    private bool missionsGenerated = false;

    private int[,] boulderGrid = new int[9, 11];
    private List<BoulderBehavior> boulders = new List<BoulderBehavior>();
    private TreasureBehavior treasureInstance;

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
        this.gameObject.GetComponent<TimerObject>().TimerOn();
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
            Debug.Log("Failed to generate cave in valid area within " + maxIterations.ToString() + " iterations.");
            return;
        }
        Instantiate(missionLocation, positionToSpawn.position + offset, this.transform.localRotation);
    }

    public void GameWon()
    {
        this.gameObject.GetComponent<TimerObject>().TimerOff();
        gameWinMessage.SetActive(true);
    }

    public void GenerateGridPuzzle(int[,] pattern)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                if (pattern[i, j] == 1)
                {
                    BoulderBehavior boulder = Instantiate(boulderObject, GridSpaceToWorldPosition(i, j), this.transform.localRotation);
                    boulder.gridPos = new Vector2Int(i, j);
                    boulder.gm = this;
                    boulderGrid[i, j] = 1;
                    boulders.Add(boulder);
                }
                else if (pattern[i, j] == 2)
                {
                    BoulderBehavior boulder = Instantiate(immovableBoulderObject, GridSpaceToWorldPosition(i, j), this.transform.localRotation);
                    boulder.gridPos = new Vector2Int(i, j);
                    boulder.gm = this;
                    boulderGrid[i, j] = 2;
                    boulders.Add(boulder);
                }
                else if (pattern[i, j] == 3)
                {
                    treasureInstance = Instantiate(treasureObject, GridSpaceToWorldPosition(i, j), this.transform.localRotation);
                    treasureInstance.gridPos = new Vector2Int(i, j);
                    treasureInstance.gm = this;
                    boulderGrid[i, j] = 3;
                }
            }
        }
    }

    public void ClearGridPuzzle()
    {
        List<BoulderBehavior> toRemove = new List<BoulderBehavior>();
        foreach (BoulderBehavior boulder in boulders)
        {
            boulderGrid[boulder.gridPos.x, boulder.gridPos.y] = 0;
            toRemove.Add(boulder);
        }

        foreach (BoulderBehavior boulder in toRemove)
        {
            boulders.Remove(boulder);
            Destroy(boulder.gameObject, 0.1f);
        }

        if (treasureInstance != null) { Destroy(treasureInstance.gameObject, 0.1f); }
    }

    public bool CheckRowColumnValidity(int x, int y, int[,] pattern)
    {
        int counter = 0;

        // Check horizontal
        for (int i = 0; i < 9; i++)
        {
            if (pattern[i, y] == 2) { counter++; }
            else { counter = 0; }

            if (counter == 3) { return false; }
        }

        // Check vertical
        counter = 0;
        for (int i = 0; i < 11; i++)
        {
            if (pattern[x, i] == 2) { counter++; }
            else { counter = 0; }

            if (counter == 3) { return false; }
        }

        return true;
    }

    public Vector3 AttemptMoveBoulder(Vector2Int fromPos, Vector2Int toPos, Vector3 boulderPos, out bool hasChanged)
    {
        int fromX = fromPos.x;
        int fromY = fromPos.y;
        int toX = toPos.x;
        int toY = toPos.y;
        if (!IsValidGridSpace(toX, toY))
        {
            hasChanged = false;
            return boulderPos;
        }
        if (boulderGrid[toX, toY] == 0)
        {
            boulderGrid[fromX, fromY] = 0;
            boulderGrid[toX, toY] = 1;
            hasChanged = true;
            return (boulderPos + new Vector3(toPos.x - fromPos.x, toPos.y - fromPos.y, 0));
        }
        else
        {
            hasChanged = false;
            return boulderPos;
        }
    }

    public void SetAdjacentBouldersActive(int x, int y, bool isActive)
    {
        List<BoulderBehavior> bouldersToActivate = GetValidAdjacentBoulders(x, y);
        foreach (BoulderBehavior boulder in bouldersToActivate)
        {
            boulder.isActive = isActive;
        }
    }

    private List<BoulderBehavior> GetValidAdjacentBoulders(int x, int y)
    {
        List<BoulderBehavior> boulders = new List<BoulderBehavior>();
        // Horizontal
        for (int i = x - 1; i <= x + 1; i+=2)
        {
            if (IsValidGridSpace(i, y) && boulderGrid[i, y] == 2)
            {
                boulders.Add(GetBoulderAtPosition(i, y));
            }
        }

        // Vertical
        for (int i = y - 1; i <= y + 1; i += 2)
        {
            if (IsValidGridSpace(x, i) && boulderGrid[x, i] == 2)
            {
                boulders.Add(GetBoulderAtPosition(x, i));
            }
        }

        if (boulders.Count != 0) { boulders.Add(GetBoulderAtPosition(x, y)); }

        return boulders;
    }
    
    public BoulderBehavior GetBoulderAtPosition(int x, int y)
    {
        foreach (BoulderBehavior boulder in boulders)
        {
            if (boulder.gridPos.x == x && boulder.gridPos.y == y)
            {
                return boulder;
            }
        }

        Debug.Log("Fatal error! Boulder with coordinates [" + x.ToString() + "," + y.ToString() + "] not found!");
        return null;
    }

    public Vector3 GridSpaceToWorldPosition(int x, int y)
    {
        Vector3 offset = new Vector3(0.5f, -0.5f, 0);
        Vector3 bottomCorner = new Vector3(-7.78f, 12.14f, 0);
        return bottomCorner + new Vector3(x, y, 0) + offset;
    }

    public bool IsValidGridSpace(int x, int y)
    {
        return !(x < 0 || x > 8 || y < 0 || y > 10);
    }

    public void SetGridSpace(int x, int y, int val)
    {
        boulderGrid[x, y] = val;
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
