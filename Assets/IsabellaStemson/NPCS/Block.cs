using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    //public int correctIndex;
    public Vector2Int Position;
    public Vector2Int Size = Vector2Int.one;
    public bool isGoal;
    private Slider_Puzzle_Manager manager;

    public bool MovesHorizontally => Size.x > Size.y;
    public bool MovesVertically => Size.y > Size.x;

    public Vector2Int GetBlockSize()
    {
        return Size;
    }

    private void Start()
    {
        manager = GetComponentInParent<Slider_Puzzle_Manager>();
    }

    public void UpdateGridPosition(Vector2Int newPosition)
    {
        Position = newPosition;
        //Debug.Log($"{gameObject.name} moved to grid position: {Position}");
    }

    public List<Vector2Int> GetOccupiedCells(Vector2Int? overridePos = null)
    {
        Vector2Int basePos = overridePos ?? Position;
        List<Vector2Int> cells = new List<Vector2Int>();

        for (int x = 0; x <Size.x; x++)
        {
            for (int y = 0; y <Size.y; y++)
            {
                cells.Add(new Vector2Int(basePos.x + x, basePos.y + y));
            }
        }
        return cells;
    }

}
