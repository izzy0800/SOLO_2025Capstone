using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_Puzzle_Manager : MonoBehaviour
{

    public static Slider_Puzzle_Manager Instance;

    public RectTransform puzzleBoardRect;
    public GridLayoutGroup slotGrid;

    public int gridWidth = 6;
    public int gridHeight = 6;

    public Vector2 cellSize;
    public bool[,] grid;

    public Block goalBlock;
    public Vector2Int exitCell;

    public void UpdateGrid() { }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (slotGrid != null)
        {
            cellSize = slotGrid.cellSize;
        }
        grid = new bool[gridWidth, gridHeight];
        UpdateGrid();
    }

    public Vector2 GetNearestCellPosition(RectTransform block)
    {
        Vector2 cellSize = slotGrid.cellSize;
        Vector2 spacing = slotGrid.spacing;
        RectOffset pad = slotGrid.padding;

        Vector2 slotSize = cellSize + spacing;

        Vector2 localPos = block.localPosition;

        int nearestX = Mathf.RoundToInt((localPos.x - pad.left) /  cellSize.x);
        int nearestY = Mathf.RoundToInt((localPos.y - pad.top) / cellSize.y);

        nearestX = Mathf.Clamp(nearestX, 0, slotGrid.constraintCount - 1);
        nearestY = Mathf.Clamp(nearestY, 0, (gridHeight - 1));

        float snappedX = pad.left + nearestX * slotSize.x;
        float snappedY = (pad.top + nearestY * slotSize.y);

        return new Vector2(snappedX, snappedY);
    }

    public bool CheckWinCondition()
    {
        foreach (Vector2Int cell in goalBlock.GetOccupiedCells())
        {
            if (cell == exitCell)
            {
                Debug.Log("Puzzle Solved");
                return true;
            }
        }
        return false;
    }

}
