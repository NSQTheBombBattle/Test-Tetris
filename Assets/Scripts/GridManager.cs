using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tetrominoPrefabs;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int gameOverHeight = 20;
    [SerializeField] private GameObject blockIndicatorPrefab;
    [SerializeField] private Transform blockIndicatorParent;
    public int width = 10;
    public int height = 25;
    public Transform[,] grid;
    public float gridSizeScale;

    private void Start()
    {
        InitGrid();
        SpawnTetromino();
    }

    private void InitGrid()
    {
        gridSizeScale = Mathf.Min(blockPrefab.GetComponent<SpriteRenderer>().bounds.size.x, blockPrefab.GetComponent<SpriteRenderer>().bounds.size.y);
        grid = new Transform[width, height];
        for(int i = 0; i <= gameOverHeight; i++)
        {
            for(int j = 0; j < width; j++)
            {
                GameObject indicatorInstance = Instantiate(blockIndicatorPrefab, blockIndicatorParent);
                indicatorInstance.transform.position = new Vector3(j * gridSizeScale, i * gridSizeScale, 0);
            }
        }
    }

    public void AddBlockToGrid(Vector2Int currentIndex, List<Transform> blockList)
    {
        for(int i = blockList.Count-1; i >= 0; i--)
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
            SpawnTetromino();
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
}
