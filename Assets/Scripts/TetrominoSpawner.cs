using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrominoSpawner : MonoBehaviour
{
    [SerializeField] private int blockBuildingPhaseTime = 25;
    [SerializeField] private List<Toggle> toggleList;
    [SerializeField] private GameObject tetrominoPrefab;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GridManager gridManager;
    private int debugSpawnHeight = 20;
    private int[,] grid;
    private bool[,] visited;
    private int[] dx = { 0, 0, -1, 1 };
    private int[] dy = { -1, 1, 0, 0 };
    private int currentSpawnSequence;
    private int gridSize;

    private void Start()
    {
        gridSize = gridManager.gridSize;
        grid = new int[gridSize, gridSize]; 
        for (int i = 0; i < toggleList.Count; i++)
        {
            int index = i;
            toggleList[i].onValueChanged.AddListener((isOn) => OnToggleUpdate(index, isOn));
        }
    }

    private void Update()
    {

    }

    private void OnToggleUpdate(int toggleIndex, bool isToggleOn)
    {
        int x = toggleIndex % gridSize;
        int y = toggleIndex / gridSize;
        grid[x, y] = isToggleOn ? 1 : 0;
    }

    public void SpawnTetromino()
    {
        List<Vector2Int> connectedGroup = GetConnectedPiece();
        if (connectedGroup.Count == 0)
            return;
        GameObject tetrominoInstance = Instantiate(tetrominoPrefab, transform);
        tetrominoInstance.transform.position = new Vector3(currentSpawnSequence * gridSize, debugSpawnHeight, 0);
        for (int i = 0; i < connectedGroup.Count; i++)
        {
            GameObject blockInstance = Instantiate(blockPrefab, tetrominoInstance.transform);
            blockInstance.GetComponent<Block>().indexOffset = connectedGroup[i];
            blockInstance.GetComponent<Block>().playerBlock = true;
            blockInstance.transform.localPosition = new Vector2(connectedGroup[i].x, connectedGroup[i].y) * gridManager.gridSizeScale;
        }
        tetrominoInstance.GetComponent<Tetromino>().gridManager = gridManager;
        tetrominoInstance.GetComponent<Tetromino>().InitTetromino(new Vector2Int(currentSpawnSequence * gridSize, debugSpawnHeight));
        currentSpawnSequence += 1;
        currentSpawnSequence %= gridManager.gridColumnCount;
    }

    private void DFS(int x, int y, List<Vector2Int> positionList)
    {
        visited[x, y] = true;
        positionList.Add(new Vector2Int(x, y));

        for (int i = 0; i < gridSize; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && ny >= 0 && nx < gridSize && ny < gridSize && grid[nx, ny] == 1 && !visited[nx, ny])
            {
                DFS(nx, ny, positionList);
            }
        }
    }

    private List<Vector2Int> GetConnectedPiece()
    {
        List<Vector2Int> pieceList = new List<Vector2Int>();
        visited = new bool[gridSize, gridSize];
        for (int y = gridSize - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridSize; x++)
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
