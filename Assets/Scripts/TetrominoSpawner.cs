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
    private GameObject currentTetromino;
    private int debugSpawnHeight = 20;
    private GameObject[,] gridObject;
    private bool[,] visited;
    private int[] dx = { 0, 0, -1, 1 };
    private int[] dy = { -1, 1, 0, 0 };
    private int currentSpawnSequence;
    private int gridSize;

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
        List<Vector2Int> connectedGroup = GetConnectedPiece();
        if (connectedGroup.Count == 0)
            yield break;
        for (int i = 0; i < connectedGroup.Count; i++)
        {
            GameObject blockInstance = Instantiate(blockPrefab, currentTetromino.transform);
            blockInstance.GetComponent<Block>().indexOffset = connectedGroup[i];
            blockInstance.transform.localPosition = new Vector2(connectedGroup[i].x, connectedGroup[i].y) * gridManager.gridSizeScale;
            blockInstance.SetActive(false);
        }
        currentTetromino.GetComponent<Tetromino>().gridManager = gridManager;
        currentTetromino.GetComponent<Tetromino>().InitTetromino(new Vector2Int(currentSpawnSequence * gridSize, debugSpawnHeight));
        currentSpawnSequence += 1;
        currentSpawnSequence %= gridManager.gridColumnCount;
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
            }
        }
    }

    private void OnToggleUpdate(int toggleIndex, bool isToggleOn)
    {
        int x = toggleIndex % gridSize;
        int y = toggleIndex / gridSize;
        //gridInt[x, y] = isToggleOn ? 1 : 0;
        if (isToggleOn)
        {
            GameObject blockInstance = Instantiate(blockPrefab, currentTetromino.transform);
            blockInstance.GetComponent<Block>().indexOffset = new Vector2Int(x, y);
            blockInstance.transform.localPosition = new Vector2(x, y) * gridManager.gridSizeScale;
        }
        else
        {
            if (gridObject[x, y] != null)
            {
                Destroy(gridObject[x, y]);
                gridObject[x, y] = null;
            }
        }
    }

    private void DFS(int x, int y, List<Vector2Int> positionList)
    {
        visited[x, y] = true;
        positionList.Add(new Vector2Int(x, y));

        for (int i = 0; i < gridSize; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && ny >= 0 && nx < gridSize && ny < gridSize && gridObject[nx, ny] != null && !visited[nx, ny])
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
                if (gridObject[x, y] != null && !visited[x, y])
                {
                    DFS(x, y, pieceList);
                    return pieceList;
                }
            }
        }
        return pieceList;
    }
}
