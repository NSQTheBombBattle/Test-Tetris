using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public GridManager gridManager;
    private Vector2Int currentPositionIndex;
    private List<Transform> childBlocks = new List<Transform>();
    private float fallTime = 0.8f; // Time before falling
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

        //if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2.left);
        //if (Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2.right);
        //if (Input.GetKeyDown(KeyCode.DownArrow)) MoveDown();
        //if (Input.GetKeyDown(KeyCode.UpArrow)) Rotate();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveTetromino(new Vector2Int(-1,0));
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveTetromino(new Vector2Int(1, 0));
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveTetromino(new Vector2Int(0, -1));
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
        currentPositionIndex.y -= 1;
        transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
        if (!ValidMove())
        {
            currentPositionIndex.y += 1;
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
        transform.Rotate(0, 0, 90);
        if (!ValidMove()) transform.Rotate(0, 0, -90);
        transform.position = new Vector3(currentPositionIndex.x * gridManager.gridSizeScale, currentPositionIndex.y * gridManager.gridSizeScale, 0);
    }

    bool ValidMove()
    {
        foreach (Transform child in transform)
        {
            Vector2 pos = child.position;
            if (!gridManager.IsInsideGrid(pos)) return false;
            if (gridManager.IsGridOccupied(pos)) return false;
        }
        return true;
    }
}

