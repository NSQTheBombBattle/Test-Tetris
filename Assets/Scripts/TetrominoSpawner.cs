using System.Collections;
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
        GameObject tetrominoInstance = Instantiate(tetrominoPrefab, gridManager.transform);
        int biggestX = 0;
        int biggestY = 0;
        for (int i = 0; i < toggleList.Count; i++)
        {
            if (toggleList[i].isOn == false)
                continue;
            int xIndex = i % GRID_SIZE;
            int yIndex = i / GRID_SIZE;
            if(xIndex > biggestX)
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
                continue;
            GameObject blockInstance = Instantiate(blockPrefab, tetrominoInstance.transform);
            float xPos = (i % GRID_SIZE * gridManager.gridSizeScale) - xOffset;
            float yPos = (i / GRID_SIZE * gridManager.gridSizeScale) - yOffset;
            blockInstance.transform.localPosition = new Vector2(xPos, yPos);
        }

        if (tetrominoInstance.transform.childCount == 0)
        {
            Destroy(tetrominoInstance);
            return;
        }
        tetrominoInstance.GetComponent<Tetromino>().gridManager = gridManager;
        tetrominoInstance.GetComponent<Tetromino>().InitTetromino(tetrominoSpawnIndex);
    }
}
