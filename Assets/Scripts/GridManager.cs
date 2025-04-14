using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tetrominoPrefabs;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int gameOverHeight = 20;
    [SerializeField] private GameObject blockIndicatorPrefab;
    [SerializeField] private Transform blockIndicatorParent;
    [SerializeField] public int gridSize;
    [SerializeField] public int gridColumnCount;
    [SerializeField] public int gridRowCount;
    private int width = 10;
    private int height = 25;
    public Transform[,] grid;
    public float gridSizeScale;
    private List<Transform> targetBlockLocation = new List<Transform>();
    private List<Transform> occupiedBlockLocation = new List<Transform>();

    private void Awake()
    {
        InitGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GeneratePuzzle();
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

    private bool IsLineComplete(int y)
    {
        for (int x = 0; x < width; x++)
            if (grid[x, y] == null) return false;
        return true;
    }

    private void ClearLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    private void MoveAllLinesDown(int y)
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

    private void GeneratePuzzle()
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
                            GameObject targetBlockInstance = Instantiate(blockPrefab);
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
        targetBlockLocation.Clear();
        AddBlockToGrid(Vector2Int.zero, occupiedBlockLocation);
    }
}
