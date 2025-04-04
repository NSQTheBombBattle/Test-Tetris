using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrominoSpawner : MonoBehaviour
{
    [SerializeField] private List<Toggle> toggleList;
    [SerializeField] private GameObject tetrominoPrefab;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Vector2Int tetrominoSpawnIndex = new Vector2Int(4, 20);
    private const int GRID_SIZE = 4;
    private int[,] grid = new int[GRID_SIZE, GRID_SIZE];
    private bool[,] visited = new bool[GRID_SIZE, GRID_SIZE];
    private int[] dx = { 0, 0, -1, 1 }; // Left, Right, Up, Down
    private int[] dy = { -1, 1, 0, 0 };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnTetromino()
    {
        //GameObject tetrominoInstance = Instantiate(tetrominoPrefab, gridManager.transform);
        int biggestX = 0;
        int biggestY = 0;
        for (int i = 0; i < toggleList.Count; i++)
        {
            if (toggleList[i].isOn == false)
                continue;
            int xIndex = i % GRID_SIZE;
            int yIndex = i / GRID_SIZE;
            if (xIndex > biggestX)
            {
                biggestX = xIndex;
            }
            if (yIndex > biggestY)
            {
                biggestY = yIndex;
            }
        }
        float xOffset = Mathf.CeilToInt(biggestX / 2f) * gridManager.gridSizeScale;
        float yOffset = Mathf.CeilToInt(biggestY / 2f) * gridManager.gridSizeScale;
        for (int i = 0; i < toggleList.Count; i++)
        {
            if (toggleList[i].isOn == false)
            {
                grid[i % GRID_SIZE, i / GRID_SIZE] = 0;
                continue;
            }
            //GameObject blockInstance = Instantiate(blockPrefab, tetrominoInstance.transform);
            //float xPos = (i % GRID_SIZE * gridManager.gridSizeScale) - xOffset;
            //float yPos = (i / GRID_SIZE * gridManager.gridSizeScale) - yOffset;
            //blockInstance.transform.localPosition = new Vector2(xPos, yPos);
            grid[i % GRID_SIZE, i / GRID_SIZE] = 1;
        }

        //if (tetrominoInstance.transform.childCount == 0)
        //{
        //    Destroy(tetrominoInstance);
        //    return;
        //}
        //tetrominoInstance.GetComponent<Tetromino>().gridManager = gridManager;
        //tetrominoInstance.GetComponent<Tetromino>().InitTetromino(tetrominoSpawnIndex);
        Debug.Log(ArePiecesConnected());
    }
    private void DFS(int x, int y)
    {
        visited[x, y] = true;

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && ny >= 0 && nx < 4 && ny < 4 && grid[nx, ny] == 1 && !visited[nx, ny])
            {
                DFS(nx, ny);
            }
        }
    }

    private bool ArePiecesConnected()
    {
        int startX = -1, startY = -1;
        int totalPieces = 0;
        visited = new bool[GRID_SIZE, GRID_SIZE];

        // Find a starting piece and count total pieces
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (grid[i, j] == 1)
                {
                    totalPieces++;
                    if (startX == -1)
                    {
                        startX = i;
                        startY = j;
                    }
                }
            }
        }

        if (startX == -1) return true; // No pieces, trivially connected

        // Start DFS from the first found piece
        DFS(startX, startY);

        // Count visited pieces
        int visitedCount = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (grid[i, j] == 1 && visited[i, j])
                {
                    visitedCount++;
                }
            }
        }

        return visitedCount == totalPieces;
    }

}
