using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tetrominoPrefabs;
    [SerializeField] private GameObject blockPrefab;
    public int width = 10;
    public int height = 25;
    public Transform[,] grid;
    public float gridSizeScale;

    private void Start()
    {
        InitGrid();
        gridSizeScale = Mathf.Min(blockPrefab.GetComponent<SpriteRenderer>().bounds.size.x, blockPrefab.GetComponent<SpriteRenderer>().bounds.size.y);
        SpawnTetromino();
    }

    private void InitGrid()
    {
        grid = new Transform[width, height];
    }

    public void AddBlockToGrid(List<Transform> blockList)
    {
        for(int i = blockList.Count-1; i >= 0; i--)
        {
            Vector2 pos = blockList[i].position;
            float xPos = pos.x / gridSizeScale;
            float yPos = pos.y / gridSizeScale;
            grid[(int)xPos, (int)yPos] = blockList[i];
            blockList[i].SetParent(this.transform);
        }
        CheckForLineClear();
        SpawnTetromino();
    }

    private void SpawnTetromino()
    {
        GameObject randomTetromino = tetrominoPrefabs[Random.Range(0, tetrominoPrefabs.Count)];
        GameObject tetrominoInstance = Instantiate(randomTetromino);
        tetrominoInstance.transform.position = new Vector3(2, 8, 0);
        tetrominoInstance.GetComponent<Tetromino>().gridManager = this;
        tetrominoInstance.GetComponent<Tetromino>().InitTetromino(new Vector2Int(4, 20));
    }

    public bool IsInsideGrid(Vector2 posIndex)
    {
        return posIndex.x >= 0 && posIndex.x < width && posIndex.y >= 0;
    }

    public bool IsGridOccupied(Vector2 posIndex)
    {
        float roundingFactor = 1 / gridSizeScale;
        float xPos = Mathf.Round(posIndex.x * roundingFactor) / roundingFactor;
        float yPos = Mathf.Round(posIndex.y * roundingFactor) / roundingFactor;

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
