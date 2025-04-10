using System.Collections;
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
    private GameObject[,] gridObject;
    private bool[,] visited;
    private int[] dx = { 0, 0, -1, 1 };
    private int[] dy = { -1, 1, 0, 0 };
    private int currentSpawnSequence;
    private int gridSize;
    private GameObject currentTetromino;

    private void Start()
    {
        gridSize = gridManager.gridSize;
        gridObject = new GameObject[gridSize, gridSize];
        for (int i = 0; i < toggleList.Count; i++)
        {
            int index = i;
            toggleList[i].onValueChanged.AddListener((isOn) => OnToggleUpdate(index, isOn));
        }
        //StartCoroutine(RoundStart());
        SpawnTetrominoBase();
    }

    private void Update()
    {

    }

    private IEnumerator RoundStart()
    {
        SpawnTetrominoBase();
        yield return new WaitForSeconds(blockBuildingPhaseTime);
        FinaliseTetrominoBase();
    }

    private void SpawnTetrominoBase()
    {
        currentTetromino = Instantiate(tetrominoPrefab, transform);
        currentTetromino.GetComponent<Tetromino>().enabled = false;
        currentTetromino.transform.position = new Vector3(currentSpawnSequence * gridSize, debugSpawnHeight, 0);
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject blockInstance = Instantiate(blockPrefab, currentTetromino.transform);
                blockInstance.GetComponent<Block>().indexOffset = new Vector2Int(x, y);
                blockInstance.transform.localPosition = new Vector2(x, y) * gridManager.gridSizeScale;
                gridObject[x, y] = blockInstance;
                blockInstance.SetActive(false);
            }
        }
    }

    public void FinaliseTetrominoBase()
    {
        List<Vector2Int> connectedGroup = GetConnectedPiece();
        if (connectedGroup.Count == 0)
        {
            Destroy(currentTetromino);
            return;
        }

        for (int x = 0; x < gridObject.GetLength(0); x++)
        {
            for (int y = 0; y < gridObject.GetLength(1); y++)
            {
                if (!visited[x, y])
                {
                    Destroy(gridObject[x,y]);
                }
            }
        }
        currentTetromino.GetComponent<Tetromino>().enabled = true;
        currentTetromino.GetComponent<Tetromino>().gridManager = gridManager;
        currentTetromino.GetComponent<Tetromino>().InitTetromino(new Vector2Int(currentSpawnSequence * gridSize, debugSpawnHeight));
        currentSpawnSequence += 1;
        currentSpawnSequence %= gridManager.gridColumnCount;
        SpawnTetrominoBase();
    }

    private void OnToggleUpdate(int toggleIndex, bool isToggleOn)
    {
        int x = toggleIndex % gridSize;
        int y = toggleIndex / gridSize;
        gridObject[x, y].SetActive(isToggleOn);
    }

    private void DFS(int x, int y, List<Vector2Int> positionList)
    {
        visited[x, y] = true;
        positionList.Add(new Vector2Int(x, y));

        for (int i = 0; i < gridSize; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && ny >= 0 && nx < gridSize && ny < gridSize && gridObject[nx, ny] != null && gridObject[nx, ny].activeSelf && !visited[nx, ny])
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
                if (gridObject[x, y] != null && gridObject[x, y].activeSelf && !visited[x, y])
                {
                    DFS(x, y, pieceList);
                    return pieceList;
                }
            }
        }
        return pieceList;
    }
}
