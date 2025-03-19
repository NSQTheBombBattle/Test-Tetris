using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public GridManager gridManager;
    private Vector2Int currentPositionIndex;
    private List<Transform> childBlocks = new List<Transform>();
    private float fallTime = 0.8f;
    private float previousTime;

    private void Start()
    {
        
    }

    public void InitTetromino(Vector2Int spawnIndex)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            childBlocks.Add(child);
            child.GetComponent<Block>().indexOffset = new Vector2(child.localPosition.x / gridManager.gridSizeScale, child.localPosition.y / gridManager.gridSizeScale);
        }
        currentPositionIndex = spawnIndex;
        transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
    }


    void Update()
    {
        if (Time.time - previousTime > fallTime)
        {
            MoveDown();
            previousTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveTetromino(new Vector2Int(-1,0));
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveTetromino(new Vector2Int(1, 0));
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveDown();
        if (Input.GetKeyDown(KeyCode.UpArrow)) Rotate();
    }

    void Move(Vector2 direction)
    {
        currentPositionIndex.x += Mathf.RoundToInt(direction.x);
        transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
        if (!ValidMove())
        {
            currentPositionIndex.x += Mathf.RoundToInt(direction.x);
            transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
        }
    }

    void MoveDown()
    {
        //MoveTetromino(new Vector2Int(0, -1));
        currentPositionIndex += new Vector2Int(0,-1);
        transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
        if (!ValidMove())
        {
            currentPositionIndex -= new Vector2Int(0, -1);
            transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
            gridManager.AddBlockToGrid(childBlocks);
            Destroy(this.gameObject);
        }
    }

    private void MoveTetromino(Vector2Int indexMoved)
    {
        currentPositionIndex += indexMoved;
        transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
        if (!ValidMove())
        {
            currentPositionIndex -= indexMoved;
            transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
        }
    }

    void Rotate()
    {
        for (int i = 0; i < childBlocks.Count; i++)
        {
            Vector2 originalOffset = childBlocks[i].GetComponent<Block>().indexOffset;
            childBlocks[i].GetComponent<Block>().indexOffset = new Vector2(originalOffset.y, -originalOffset.x);
            childBlocks[i].transform.localPosition = childBlocks[i].GetComponent<Block>().indexOffset * gridManager.gridSizeScale;
        }
        if (!ValidMove()) 
        {
            for (int i = 0; i < childBlocks.Count; i++)
            {
                Vector2 originalOffset = childBlocks[i].GetComponent<Block>().indexOffset;
                childBlocks[i].GetComponent<Block>().indexOffset = new Vector2(-originalOffset.y, originalOffset.x);
                childBlocks[i].transform.localPosition = childBlocks[i].GetComponent<Block>().indexOffset * gridManager.gridSizeScale;
            }
        }

        transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
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

