using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public GridManager gridManager;
    private List<Transform> childBlocks = new List<Transform>();
    private float fallTime = 0.8f; // Time before falling
    private float previousTime;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            childBlocks.Add(child);
        }
    }

    void Update()
    {
        if (Time.time - previousTime > fallTime)
        {
            MoveDown();
            previousTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2.right);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveDown();
        if (Input.GetKeyDown(KeyCode.UpArrow)) Rotate();
    }

    void Move(Vector2 direction)
    {
        transform.position += (Vector3)direction * gridManager.gridSizeScale;
        if (!ValidMove()) transform.position -= (Vector3)direction * gridManager.gridSizeScale;
    }

    void MoveDown()
    {
        transform.position += Vector3.down * gridManager.gridSizeScale;
        if (!ValidMove())
        {
            transform.position -= Vector3.down * gridManager.gridSizeScale;
            gridManager.AddBlockToGrid(childBlocks);
            Destroy(this.gameObject);
        }
    }

    void Rotate()
    {
        transform.Rotate(0, 0, 90);
        if (!ValidMove()) transform.Rotate(0, 0, -90);
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

