using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public GridManager gridManager;
    [SerializeField] private float fallTime = 0.8f;
    private Vector2Int currentPositionIndex;
    private List<Transform> childBlocks = new List<Transform>();
    private float fallTimer;
    private float currentFallTime;

    private void Start()
    {
        currentFallTime = fallTime;
        //InitTetromino(Vector2Int.zero);
    }

    public void InitTetromino(Vector2Int spawnIndex)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            childBlocks.Add(child);
            int xPos = Mathf.CeilToInt(child.localPosition.x / gridManager.gridSizeScale);
            int yPos = Mathf.CeilToInt(child.localPosition.y / gridManager.gridSizeScale);
            child.GetComponent<Block>().indexOffset = new Vector2Int(xPos, yPos);
        }
        currentPositionIndex = spawnIndex;
        transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
    }

    public List<Vector2Int> GetOccupiedIndex()
    {
        List<Vector2Int> occupiedIndex = new List<Vector2Int>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            int xPos = Mathf.CeilToInt(child.localPosition.x / gridManager.gridSizeScale);
            int yPos = Mathf.CeilToInt(child.localPosition.y / gridManager.gridSizeScale);
            occupiedIndex.Add(new Vector2Int(xPos, yPos + 3));
        }
        return occupiedIndex;
    }


    void Update()
    {
        fallTimer += Time.deltaTime;
        if (fallTimer >= currentFallTime)
        {
            MoveDown();
        }
    }

    void MoveDown()
    {
        fallTimer = 0;
        currentPositionIndex += new Vector2Int(0, -1);
        transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
        if (!ValidMove())
        {
            currentPositionIndex -= new Vector2Int(0, -1);
            transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
            gridManager.AddBlockToGrid(currentPositionIndex, childBlocks);
            Destroy(this.gameObject);
        }
    }

    bool ValidMove()
    {
        for (int i = 0; i < childBlocks.Count; i++)
        {
            Vector2 posIndex = new Vector2(childBlocks[i].GetComponent<Block>().indexOffset.x + currentPositionIndex.x, childBlocks[i].GetComponent<Block>().indexOffset.y + currentPositionIndex.y);
            if (!gridManager.IsInsideGrid(posIndex)) return false;
            if (gridManager.IsGridOccupied(posIndex)) return false;
        }
        return true;
    }
}

