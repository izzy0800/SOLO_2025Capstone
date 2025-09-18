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
        DebugGridInfo();

        GameObject topLeftBlockobj = GameObject.Find("G block");
        if (topLeftBlockobj != null)
        {
            AnalyzeCorrectBlock(topLeftBlockobj.GetComponent<RectTransform>(), 0, 0);
        }
        else
        {
            Debug.LogWarning("could not find block with that name");
        }

        UpdateGrid();
    }
    
    public Vector2 GetNearestCellPositionWithCollision(RectTransform block, out bool isValidMove)
    {
        //Debug.Log("GetNearestCellPosition called - waiting for analysis results");
        //return block.anchoredPosition;

        Vector2 cellSize = slotGrid.cellSize;
        Vector2 spacing = slotGrid.spacing;
        Vector2 totalCellSize = cellSize + spacing;

        Vector2 blockPos = block.anchoredPosition;
        Vector2 gridOffset = new Vector2(-342.30f, 426.40f); //most important line in this whole shit lowkey - DO NOT TOUCH!

        Block blockComponent = block.GetComponent<Block>();
        Vector2Int blockSize = blockComponent != null ? blockComponent.Size : Vector2Int.one;

        Vector2 adjustedBlockPos = blockPos;
        if (blockSize.x == 2)
        {
            adjustedBlockPos.x -= totalCellSize.x * 0.5f; 
        }
        if (blockSize.y == 2)
        {
            adjustedBlockPos.y += totalCellSize.y * 0.5f; 
        }

        Vector2 relativePos = adjustedBlockPos - gridOffset;

        int nearestX = Mathf.RoundToInt(relativePos.x / totalCellSize.x);
        int nearestY = Mathf.RoundToInt(-relativePos.y / totalCellSize.y);

        nearestX = Mathf.Clamp(nearestX, 0, gridWidth - blockSize.x);
        nearestY = Mathf.Clamp(nearestY, 0, gridHeight - blockSize.y);

        Vector2 snappedPos = gridOffset + new Vector2(
            nearestX * totalCellSize.x,
            -nearestY * totalCellSize.y
            );

        if (blockSize.x == 2)
        {
            snappedPos.x += totalCellSize.x * 0.5f;
        }
        if (blockSize.y == 2)
        {
            snappedPos.y -= totalCellSize.y * 0.5f;
        }

        //Debug.Log($"=== DETAILED SNAP DEBUG ===");
        //Debug.Log($"Block pos: {blockPos}");
        //Debug.Log($"Adjusted pos: {adjustBlockPos}");
        //Debug.Log($"Total cell size: {totalCellSize}");
        //Debug.Log($"Relative pos: {relativePos}");
        //Debug.Log($"Raw X calc: {relativePos.x / totalCellSize.x}, Raw Y calc: {-relativePos.y / totalCellSize.y}");
        //Debug.Log($"Nearest cell: ({nearestX}, {nearestY})");
        //Debug.Log($"Snapped position: {snappedPos}");

        Vector2Int targetPos = new Vector2Int(nearestX, nearestY);
        isValidMove = IsPositionValid(blockComponent, targetPos);

        if (blockComponent != null && isValidMove)
        {
            blockComponent.UpdateGridPosition(targetPos);

            if (blockSize.x > 1 || blockSize.y > 1)
            {

                //Debug.Log($"MULTI-CELL DEBUG: {block.name}");
                //Debug.Log($"Block size: {blockSize}");
                //Debug.Log($"Block anchor pos: {blockPos}");
                //Debug.Log($"Calculated cell: ({nearestX}, {nearestY})");
                //Debug.Log($"Snapped to: {snappedPos}");
                //Debug.Log($"Should occupy cells: {string.Join(", ", blockComponent.GetOccupiedCells())}");
            }
        }

        Debug.Log($"Block: {block.name}, cell: ({nearestX}, {nearestY}), Valid: {isValidMove}");

        return snappedPos;

       
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

    public void DebugGridInfo()
    {
        if (slotGrid == null) return;

        Vector2 cellSize = slotGrid.cellSize;
        Vector2 spacing = slotGrid.spacing;
        RectOffset padding = slotGrid.padding;

        //Debug.Log($"=== GRID DEBUG INFO ===");
        //Debug.Log($"Cell Size: {cellSize}");
        //Debug.Log($"Spacing: {spacing}");
        //Debug.Log($"Padding: L:{padding.left} R:{padding.right} T:{padding.top} B:{padding.bottom}");
        //Debug.Log($"Grid Rect Position: {slotGrid.transform.position}");
        //Debug.Log($"Grid Local Position: {slotGrid.transform.localPosition}");
        //Debug.Log($"Grid Anchored Position: {slotGrid.GetComponent<RectTransform>().anchoredPosition}");
    }

    public void AnalyzeCorrectBlock(RectTransform correctBlock, int expectedX, int expectedY)
    {
        //Debug.Log($"=== ANALYZING CORRECT BLOCK ===");
        //Debug.Log($"Block anchored Position: {correctBlock.anchoredPosition}");
        //Debug.Log($"Expected grid cell: ({expectedX}, {expectedY})");

        Vector2 cellSize = slotGrid.cellSize;
        Vector2 spacing = slotGrid.spacing;
        //Debug.Log($"Cell Size: {cellSize}, Spacing: {spacing}");

        Vector2 totalCellSize = cellSize + spacing;
        Vector2 myCalculation = new Vector2(
            expectedX * totalCellSize.x,
            -expectedY * totalCellSize.y
            );

        //Debug.Log($"My math for cell ({expectedX}, {expectedY}): {myCalculation}");
        //Debug.Log($"Actual block position: {correctBlock.anchoredPosition}");
        //Debug.Log($"*** DIFFERENCE (This is my offset): {correctBlock.anchoredPosition - myCalculation} ***");
    }

    public bool IsPositionValid(Block movingBlock, Vector2Int targetPosition)
    {
        List<Vector2Int> targetCells = movingBlock.GetOccupiedCells(targetPosition);

        foreach (Vector2Int cell in targetCells)
        {
            if (IsCellOccupiedByOtherBlock(cell, movingBlock))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsCellOccupiedByOtherBlock(Vector2Int cell, Block excludeBlock)
    {
        Block[] allBlocks = FindObjectsOfType<Block>();
        
        foreach (Block block in allBlocks)
        {
            if (block == excludeBlock) continue;

            List<Vector2Int> occupiedCells = block.GetOccupiedCells();
            if (occupiedCells.Contains(cell))
            {
                return true;
            }
        }

        return false;
    }

}
