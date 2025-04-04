using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tetrominoPrefabs;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject blockPrefab2;
    [SerializeField] private int gameOverHeight = 20;
    [SerializeField] private GameObject blockIndicatorPrefab;
    [SerializeField] private Transform blockIndicatorParent;
    [SerializeField] private int gridSize;
    [SerializeField] private int gridColumnCount;
    [SerializeField] private int gridRowCount;
    private int width = 10;
    private int height = 25;
    public Transform[,] grid;
    public float gridSizeScale;
    private List<Transform> targetBlockLocation = new List<Transform>();
    private List<Transform> occupiedBlockLocation = new List<Transform>();

    private void Awake()
    {
        InitGrid();
        //SpawnTetromino();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GeneratePuzzle2();
        }
    }

    private void InitGrid()
    {
        width = gridSize * gridColumnCount;
        gridSizeScale = Mathf.Min(blockPrefab.GetComponent<SpriteRenderer>().bounds.size.x, blockPrefab.GetComponent<SpriteRenderer>().bounds.size.y);
        grid = new Transform[width, height];
        for (int i = 0; i <= gameOverHeight; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject indicatorInstance = Instantiate(blockIndicatorPrefab, blockIndicatorParent);
                indicatorInstance.transform.position = new Vector3(j * gridSizeScale, i * gridSizeScale, 0);
            }
        }
    }

    public void AddBlockToGrid(Vector2Int currentIndex, List<Transform> blockList)
    {
        for (int i = blockList.Count - 1; i >= 0; i--)
        {
            Vector2 pos = currentIndex + blockList[i].GetComponent<Block>().indexOffset;
            grid[(int)pos.x, (int)pos.y] = blockList[i];
            blockList[i].SetParent(this.transform);
        }
        CheckForLineClear();
        if (CheckForLineExceeded())
        {
            Debug.Log("Game Over!");
        }
        else
        {
            //SpawnTetromino();
        }
    }

    private void SpawnTetromino()
    {
        GameObject randomTetromino = tetrominoPrefabs[Random.Range(0, tetrominoPrefabs.Count)];
        GameObject tetrominoInstance = Instantiate(randomTetromino);
        tetrominoInstance.GetComponent<Tetromino>().gridManager = this;
        tetrominoInstance.GetComponent<Tetromino>().InitTetromino(new Vector2Int(4, 20));
    }

    public bool IsInsideGrid(Vector2 posIndex)
    {
        return posIndex.x >= 0 && posIndex.x < width && posIndex.y >= 0;
    }

    public bool IsGridOccupied(Vector2 posIndex)
    {
        return grid[(int)posIndex.x, (int)posIndex.y] != null;
    }

    private void CheckForLineClear()
    {
        for (int y = height - 1; y >= 0; y--)
        {
            if (IsLineComplete(y))
            {
                ClearLine(y);
                MoveAllLinesDown(y);
            }
        }
    }

    private bool CheckForLineExceeded()
    {
        for (int y = gameOverHeight; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                if (grid[x, y] != null) return true;
        }
        return false;
    }

    bool IsLineComplete(int y)
    {
        for (int x = 0; x < width; x++)
            if (grid[x, y] == null) return false;
        return true;
    }

    void ClearLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    void MoveAllLinesDown(int y)
    {
        for (int i = y; i < height - 1; i++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, i + 1] != null)
                {
                    int newY = i;
                    if (grid[x, i + 1].GetComponent<Block>().playerBlock)
                    {
                        while (newY > 0 && grid[x, newY - 1] == null)
                        {
                            newY--;
                        }
                    }
                    grid[x, newY] = grid[x, i + 1];
                    grid[x, newY].position += Vector3.down * (i + 1 - newY) * gridSizeScale;
                    grid[x, i + 1] = null;
                    //grid[x, i] = grid[x, i + 1];
                    //grid[x, i].position += Vector3.down * gridSizeScale;
                    //grid[x, i + 1] = null;
                }
            }
        }
    }

    private void GeneratePuzzle()
    {
        for (int i = occupiedBlockLocation.Count - 1; i >= 0; i--)
        {
            Destroy(occupiedBlockLocation[i].gameObject);
        }
        targetBlockLocation.Clear();
        occupiedBlockLocation.Clear();

        float chanceToOccupied = 0.6f;
        float yOffset = 0;
        for (int j = 0; j < gridRowCount; j++)
        {
            float xOffset = 0;
            for (int i = 0; i < gridColumnCount; i++)
            {
                List<(int, int)> targetBlockList = new List<(int, int)>();
                int highestY = 0;
                for (int y = gridSize - 1; y >= 0; y--)
                {
                    for (int x = 0; x < gridSize; x++)
                    {
                        if (Random.Range(0, 1f) <= chanceToOccupied && (y == gridSize - 1 || y >= highestY || targetBlockList.Contains((x, y + 1))))
                        {
                            GameObject targetBlockInstance = Instantiate(blockPrefab);
                            targetBlockInstance.transform.position = new Vector2(x * gridSizeScale + xOffset, y * gridSizeScale + yOffset);
                            targetBlockInstance.GetComponent<Block>().indexOffset = new Vector2Int(x, y);
                            targetBlockLocation.Add(targetBlockInstance.transform);
                            targetBlockList.Add((x, y));
                            if (y > highestY)
                            {
                                highestY = y;
                            }
                        }
                        else
                        {
                            if (y > highestY)
                            {
                                GameObject occupiedBlockInstance = Instantiate(blockPrefab);
                                occupiedBlockInstance.transform.position = new Vector2(x * gridSizeScale + xOffset, y * gridSizeScale + yOffset);
                                targetBlockLocation.Add(occupiedBlockInstance.transform);
                                targetBlockList.Add((x, y));
                                continue;
                            }
                            GameObject targetBlockInstance = Instantiate(blockPrefab2);
                            targetBlockInstance.transform.position = new Vector2(x * gridSizeScale + xOffset, y * gridSizeScale + yOffset);
                            targetBlockInstance.GetComponent<Block>().indexOffset = new Vector2Int(x + i * gridSize, y + j * gridSize);
                            occupiedBlockLocation.Add(targetBlockInstance.transform);
                        }
                    }
                }
                xOffset += gridSize * gridSizeScale;
            }
            yOffset += gridSize * gridSizeScale;
        }
        AddBlockToGrid(Vector2Int.zero, occupiedBlockLocation);
        for (int i = targetBlockLocation.Count - 1; i >= 0; i--)
        {
            Destroy(targetBlockLocation[i].gameObject);
        }
        targetBlockLocation.Clear();
    }

    private void GeneratePuzzle2()
    {
        for (int i = occupiedBlockLocation.Count - 1; i >= 0; i--)
        {
            Destroy(occupiedBlockLocation[i].gameObject);
        }
        targetBlockLocation.Clear();
        occupiedBlockLocation.Clear();

        float yOffset = 0;
        for (int j = 0; j < gridRowCount; j++)
        {
            float xOffset = 0;
            for (int i = 0; i < gridColumnCount; i++)
            {
                GameObject randomTetromino = tetrominoPrefabs[Random.Range(0, tetrominoPrefabs.Count)];
                randomTetromino.GetComponent<Tetromino>().gridManager = this;
                List<Vector2Int> occuppiedIndex = randomTetromino.GetComponent<Tetromino>().GetOccupiedIndex();
                for (int y = gridSize - 1; y >= 0; y--)
                {
                    for (int x = 0; x < gridSize; x++)
                    {
                        if (!occuppiedIndex.Contains(new Vector2Int(x, y)))
                        {
                            GameObject targetBlockInstance = Instantiate(blockPrefab2);
                            targetBlockInstance.transform.position = new Vector2(x * gridSizeScale + xOffset, y * gridSizeScale + yOffset);
                            targetBlockInstance.GetComponent<Block>().indexOffset = new Vector2Int(x + i * gridSize, y + j * gridSize);
                            occupiedBlockLocation.Add(targetBlockInstance.transform);
                            Debug.Log("Spawn");
                        }
                    }
                }
                xOffset += gridSize * gridSizeScale;
            }
            yOffset += gridSize * gridSizeScale;
        }
        //AddBlockToGrid(Vector2Int.zero, occupiedBlockLocation);
        //for (int i = targetBlockLocation.Count - 1; i >= 0; i--)
        //{
        //    Destroy(targetBlockLocation[i].gameObject);
        //}
        targetBlockLocation.Clear();
    }
}
