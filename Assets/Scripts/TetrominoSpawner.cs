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
        for (int i = 0; i < toggleList.Count; i++)
        {
            if (toggleList[i].isOn == false)
            {
                grid[i % GRID_SIZE, i / GRID_SIZE] = 0;
                continue;
            }
            grid[i % GRID_SIZE, i / GRID_SIZE] = 1;
        }
        List<List<Vector2Int>> connectedGroup = CountConnectedGroups();
        for (int i = 0; i < connectedGroup.Count; i++)
        {
            GameObject tetrominoInstance = Instantiate(tetrominoPrefab, gridManager.transform);
            tetrominoInstance.transform.position = new Vector3(4, 10, 0);
            for (int j = 0; j < connectedGroup[i].Count; j++)
            {
                GameObject blockInstance = Instantiate(blockPrefab, tetrominoInstance.transform);
                blockInstance.GetComponent<Block>().indexOffset = connectedGroup[i][j];
                blockInstance.transform.localPosition = new Vector2(connectedGroup[i][j].x, connectedGroup[i][j].y) * gridManager.gridSizeScale;
            }
            tetrominoInstance.GetComponent<Tetromino>().gridManager = gridManager;
            tetrominoInstance.GetComponent<Tetromino>().InitTetromino(tetrominoSpawnIndex);
        }
    }

    private void DFS(int x, int y, List<Vector2Int> group)
    {
        visited[x, y] = true;
        group.Add(new Vector2Int(x, y)); // Store the current cell as part of the group

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && ny >= 0 && nx < 4 && ny < 4 && grid[nx, ny] == 1 && !visited[nx, ny])
            {
                DFS(nx, ny, group); // Continue DFS in all directions
            }
        }
    }

    private List<List<Vector2Int>> CountConnectedGroups()
    {
        visited = new bool[GRID_SIZE, GRID_SIZE]; // Reset visited array
        List<List<Vector2Int>> groups = new List<List<Vector2Int>>(); // List of groups
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                if (grid[i, j] == 1 && !visited[i, j])
                {
                    // Found a new group, start DFS and store the connected cells
                    List<Vector2Int> group = new List<Vector2Int>();
                    DFS(i, j, group);
                    groups.Add(group); // Add the group to the list of groups
                }
            }
        }
        return groups;
    }
}
