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
    private int[] dx = { 0, 0, -1, 1 };
    private int[] dy = { -1, 1, 0, 0 };

    private void Update()
    {

    }

    public void SpawnTetromino()
    {
        for (int i = 0; i < toggleList.Count; i++)
        {
            grid[i % GRID_SIZE, i / GRID_SIZE] = toggleList[i].isOn ? 1 : 0;
        }
        List<Vector2Int> connectedGroup = GetConnectedPiece();
        if (connectedGroup.Count == 0)
            return;
        GameObject tetrominoInstance = Instantiate(tetrominoPrefab, gridManager.transform);
        tetrominoInstance.transform.position = new Vector3(4, 20, 0);
        for (int i = 0; i < connectedGroup.Count; i++)
        {
            GameObject blockInstance = Instantiate(blockPrefab, tetrominoInstance.transform);
            blockInstance.GetComponent<Block>().indexOffset = connectedGroup[i];
            blockInstance.GetComponent<Block>().playerBlock = true;
            blockInstance.transform.localPosition = new Vector2(connectedGroup[i].x, connectedGroup[i].y) * gridManager.gridSizeScale;
        }
        tetrominoInstance.GetComponent<Tetromino>().gridManager = gridManager;
        tetrominoInstance.GetComponent<Tetromino>().InitTetromino(tetrominoSpawnIndex);
        Debug.Log(tetrominoInstance.transform.position);
        //List<List<Vector2Int>> connectedGroup = GetConnectedPiece();
        //for (int i = 0; i < connectedGroup.Count; i++)
        //{
        //    GameObject tetrominoInstance = Instantiate(tetrominoPrefab, gridManager.transform);
        //    tetrominoInstance.transform.position = new Vector3(4, 10, 0);
        //    for (int j = 0; j < connectedGroup[i].Count; j++)
        //    {
        //        GameObject blockInstance = Instantiate(blockPrefab, tetrominoInstance.transform);
        //        blockInstance.GetComponent<Block>().indexOffset = connectedGroup[i][j];
        //        blockInstance.GetComponent<Block>().playerBlock = true;
        //        blockInstance.transform.localPosition = new Vector2(connectedGroup[i][j].x, connectedGroup[i][j].y) * gridManager.gridSizeScale;
        //    }
        //    tetrominoInstance.GetComponent<Tetromino>().gridManager = gridManager;
        //    tetrominoInstance.GetComponent<Tetromino>().InitTetromino(tetrominoSpawnIndex);
        //}
    }

    private void DFS(int x, int y, List<Vector2Int> positionList)
    {
        visited[x, y] = true;
        positionList.Add(new Vector2Int(x, y));

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && ny >= 0 && nx < 4 && ny < 4 && grid[nx, ny] == 1 && !visited[nx, ny])
            {
                DFS(nx, ny, positionList);
            }
        }
    }

    private List<Vector2Int> GetConnectedPiece()
    {
        List<Vector2Int> pieceList = new List<Vector2Int>();
        visited = new bool[GRID_SIZE, GRID_SIZE];
        for (int y = GRID_SIZE - 1; y >= 0; y--)
        {
            for (int x = 0; x < GRID_SIZE; x++)
            {
                if (grid[x, y] == 1 && !visited[x, y])
                {
                    DFS(x, y, pieceList);
                    return pieceList;
                }
            }
        }
        return pieceList;
    }
}
