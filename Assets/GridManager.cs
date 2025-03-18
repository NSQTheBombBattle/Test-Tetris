using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject tetrominoPrefab;
    [SerializeField] private GameObject blockPrefab;
    public int width = 10;
    public int height = 20;
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
        GameObject tetrominoInstance = Instantiate(tetrominoPrefab);
        tetrominoInstance.transform.position = new Vector3(2, 5, 0);
        tetrominoInstance.GetComponent<Tetromino>().gridManager = this;
    }

    public bool IsInsideGrid(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < width * gridSizeScale && pos.y >= 0;
    }

    public bool IsGridOccupied(Vector2 pos)
    {
        float xPos = pos.x / gridSizeScale;
        float yPos = pos.y / gridSizeScale;

        return grid[(int)xPos, (int)yPos] != null;
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
