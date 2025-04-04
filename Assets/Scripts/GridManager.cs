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
    [SerializeField] private int gridAmount;
    private int width = 10;
    private int height = 25;
    public Transform[,] grid;
    public float gridSizeScale;
    private List<Transform> tempObjects = new List<Transform>();
    private List<Transform> tempObjects2 = new List<Transform>();

    private void Start()
    {
        InitGrid();
        //SpawnTetromino();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestingFunction();
        }
    }

    private void InitGrid()
    {
        width = gridSize * gridAmount;
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
                    grid[x, i] = grid[x, i + 1];
                    grid[x, i].position += Vector3.down * gridSizeScale;
                    grid[x, i + 1] = null;
                }
            }
        }
    }

    private void TestingFunction()
    {
        for (int i = tempObjects.Count - 1; i >= 0; i--)
        {
            Destroy(tempObjects[i].gameObject);
        }
        for (int i = tempObjects2.Count - 1; i >= 0; i--)
        {
            Destroy(tempObjects2[i].gameObject);
        }
        tempObjects.Clear();
        tempObjects2.Clear();
        float chanceToOccupied = 0.6f;
        float yOffset = 0;
        for (int j = 0; j < 3; j++)
        {
            float xOffset = 0;
            for (int i = 0; i < gridAmount; i++)
            {
                List<(int, int)> occupiedTiles = new List<(int, int)>();
                int highestY = 0;
                for (int y = gridSize - 1; y >= 0; y--)
                {
                    for (int x = 0; x < gridSize; x++)
                    {
                        float radom = Random.Range(0, 1f);
                        if (radom <= chanceToOccupied && (y == gridSize - 1 || y >= highestY || occupiedTiles.Contains((x, y + 1))))
                        {
                            GameObject instance = Instantiate(blockPrefab);
                            instance.transform.position = new Vector2(x * gridSizeScale + xOffset, y * gridSizeScale + yOffset);
                            instance.GetComponent<Block>().indexOffset = new Vector2Int(x, y);
                            tempObjects.Add(instance.transform);
                            occupiedTiles.Add((x, y));
                            if (y > highestY)
                            {
                                highestY = y;
                            }
                        }
                        else
                        {
                            if (y > highestY)
                            {
                                GameObject instance2 = Instantiate(blockPrefab);
                                instance2.transform.position = new Vector2(x * gridSizeScale + xOffset, y * gridSizeScale + yOffset);
                                tempObjects.Add(instance2.transform);
                                occupiedTiles.Add((x, y));
                                continue;
                            }
                            GameObject instance = Instantiate(blockPrefab2);
                            instance.transform.position = new Vector2(x * gridSizeScale + xOffset, y * gridSizeScale + yOffset);
                            instance.GetComponent<Block>().indexOffset = new Vector2Int(x + i * gridSize, y + j * gridSize);
                            tempObjects2.Add(instance.transform);
                        }
                    }
                }
                xOffset += gridSize * gridSizeScale;
            }
            yOffset += gridSize * gridSizeScale;
        }
        AddBlockToGrid(Vector2Int.zero, tempObjects2);
        for (int i = tempObjects.Count - 1; i >= 0; i--)
        {
            Destroy(tempObjects[i].gameObject);
        }
    }

}
