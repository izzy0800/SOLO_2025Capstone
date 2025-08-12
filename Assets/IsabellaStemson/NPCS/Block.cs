using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public Vector2Int Position;
    public Vector2Int Size;
    public bool isHorizontal;

    private Slider_Puzzle_Manager manager;
    private Vector3 dragStartPos;
    private Vector2Int originalGridPos;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<Slider_Puzzle_Manager>();
    }

    public List<Vector2Int> GetOccupiedCells()
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                cells.Add(new Vector2Int(Position.x + x, Position.y + y));
            }
        }
        return cells;
    }

    public void TryMove(int amount)
    {
        Vector2Int moveDir = isHorizontal ? new Vector2Int(amount, 0) : new Vector2Int(0, amount);
        Vector2Int newPos = Position + moveDir;

        if (CanMoveTo(newPos))
        {
            Position = newPos;
            transform.localPosition = new Vector3(Position.x, -Position.y, 0);
            manager.UpdateGrid();
        }
    }

    private bool CanMoveTo(Vector2Int newPos)
    {
        foreach (Vector2Int cell in GetOccupiedCells(newPos))
        {
            if (cell.x < 0 || cell.x >= manager.gridWidth || cell.y < 0 || cell.y >= manager.gridHeight)
                return false;
            if (manager.grid[cell.x, cell.y] && !GetOccupiedCells().Contains(cell))
                return false;
        }
        return true;
    }

    private List<Vector2Int> GetOccupiedCellsAt(Vector2Int pos)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                cells.Add(new Vector2Int(pos.x + x, pos.y + y));
            }
        }
        return cells;
    }

    void OnMouseDown()
    {
        dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        originalGridPos = Position;
    }

    void OnMouseDrag()
    {
        Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dragDelta = currentPos - dragStartPos;

        if (isHorizontal)
        {
            int moveAmount = Mathf.RoundToInt(dragDelta.x);
            TryMove(moveAmount);
        }
        else
        {
            int moveAmount = Mathf.RoundToInt(-dragDelta.y); // inverted Y
            TryMove(moveAmount);
        }
    }

    //private void OnMouseDrag()
    //{
    //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    if (isHorizontal)
    //    {
    //        int moveAmount = Mathf.RoundToInt(worldPos.x - transform.position.x);
    //        TryMove(moveAmount);
    //    }
    //    else
    //    {
    //        int moveAmount = Mathf.RoundToInt(worldPos.y - transform.position.y);
    //        TryMove(-moveAmount);
    //    }
    //}

}
