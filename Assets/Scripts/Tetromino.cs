using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public GridManager gridManager;
    [SerializeField] private float fallTime = 0.8f;
    [SerializeField] private float fastFallTime = 0.05f;
    [SerializeField] private float holdTimeForFastFall = 0.25f;
    private Vector2Int currentPositionIndex;
    private List<Transform> childBlocks = new List<Transform>();
    private bool isHoldingDown;
    private float fallTimer;
    private float holdTimer;
    private float currentFallTime;

    private void Start()
    {
        currentFallTime = fallTime;
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
        fallTimer += Time.deltaTime;
        if (fallTimer >= currentFallTime)
        {
            MoveDown();
        }

        //if (Input.GetKeyDown(KeyCode.UpArrow)) Rotate();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveTetromino(new Vector2Int(-1, 0));
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveTetromino(new Vector2Int(1, 0));
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            isHoldingDown = true;
            MoveDown();
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            isHoldingDown = false;
            holdTimer = 0;
            currentFallTime = fallTime;
        }

        CheckMoveDownHold();
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

    private void CheckMoveDownHold()
    {
        if (isHoldingDown)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdTimeForFastFall)
            {
                currentFallTime = fastFallTime;
            }
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

