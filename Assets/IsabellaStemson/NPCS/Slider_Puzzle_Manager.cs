using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

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

        InitializeBlockPositions();
        FixColliderSizes();
        DebugSpecificBlock("C block");
        VerifyGridCalculation(new Vector2(70.50f, -257.30f), "C block");

        DebugAllBlockPositions();

        UpdateGrid();
    }

    void Update()
    {
        ValidateColliderSizes();
    }

    private void DebugSpecificBlock(string blockName)
    {
        GameObject blockObj = GameObject.Find(blockName);
        if (blockObj != null)
        {
            Block block = blockObj.GetComponent<Block>();
            RectTransform rect = blockObj.GetComponent<RectTransform>();

            //Debug.Log($"=== DEBUG {blockName} ===");
            //Debug.Log($"Visual position: {rect.anchoredPosition}");
            //Debug.Log($"Logical position: {block.Position}");
            //Debug.Log($"Size: {block.Size}");
            //Debug.Log($"Occupied cells: [{string.Join(", ", block.GetOccupiedCells())}]");
        }
    }

    private void VerifyGridCalculation(Vector2 visualPos, string blockName)
    {
        Vector2 gridOffset = new Vector2(-342.30f, 426.40f);
        Vector2 cellSize = slotGrid.cellSize;
        Vector2 spacing = slotGrid.spacing;
        Vector2 totalCellSize = cellSize + spacing;

        Vector2 relativePos = visualPos - gridOffset;
        int calculatedX = Mathf.RoundToInt(relativePos.x / totalCellSize.x);
        int calculatedY = Mathf.RoundToInt(-relativePos.y / totalCellSize.y);

        //Debug.Log($"=== GRID CALCULATION VERIFICATION for {blockName} ===");
        //Debug.Log($"Visual pos: {visualPos}");
        //Debug.Log($"Grid offset: {gridOffset}");
        //Debug.Log($"Relative pos: {relativePos}");
        //Debug.Log($"Total cell size: {totalCellSize}");
        //Debug.Log($"Calculated grid pos: ({calculatedX}, {calculatedY})");
    }

    public void DebugAllBlockPositions()
    {
        //Debug.Log("=== ALL BLOCK POSITIONS ===");
        Block[] allBlocks = FindObjectsOfType<Block>();


        foreach (Block block in allBlocks)
        {
            RectTransform rect = block.GetComponent<RectTransform>();
            Vector2 visualPos = rect.anchoredPosition;

            //Debug.Log($"{block.name}: Visual({visualPos.x:F1}, {visualPos.y:F1}) " +
            //    $"Logical{block.Position} Size{block.Size} " +
            //    $"Occupies[{string.Join(",", block.GetOccupiedCells())}]");
        }
    }

    public Vector2 GetNearestCellPositionWithCollision(RectTransform block, out bool isValidMove)
    {
        Vector2 cellSize = slotGrid.cellSize;
        Vector2 spacing = slotGrid.spacing;
        Vector2 totalCellSize = cellSize + spacing;

        Vector2 blockPos = block.anchoredPosition;
        Vector2 gridOffset = new Vector2(-342.30f, 426.40f);

        Block blockComponent = block.GetComponent<Block>();
        Vector2Int blockSize = blockComponent != null ? blockComponent.Size : Vector2Int.one;

        Vector2 adjustedBlockPos = blockPos;
        if (blockSize.x == 2)
        {
            adjustedBlockPos.x -= totalCellSize.x * 0.5f;
        }
        else if (blockSize.x == 3)
        {
            adjustedBlockPos.x -= totalCellSize.x * 1.0f; 
        }

        if (blockSize.y == 2)
        {
            adjustedBlockPos.y += totalCellSize.y * 0.5f;
        }
        else if (blockSize.y == 3)
        {
            adjustedBlockPos.y += totalCellSize.y * 1.0f; 
        }

        Vector2 relativePos = adjustedBlockPos - gridOffset;

        int nearestX = Mathf.RoundToInt(relativePos.x / totalCellSize.x);
        int nearestY = Mathf.RoundToInt(-relativePos.y / totalCellSize.y);

        nearestX = Mathf.Clamp(nearestX, 0, gridWidth - blockSize.x);
        nearestY = Mathf.Clamp(nearestY, 0, gridHeight - blockSize.y);

        if (nearestX < 0 || nearestX > gridWidth - blockSize.x ||
            nearestY < 0 || nearestY > gridHeight - blockSize.y)
        {
            //Debug.LogError($"BOUNDS ERROR: {block.name} trying to go to ({nearestX}, {nearestY}) but grid limits are ({gridWidth - blockSize.x}, {gridHeight - blockSize.y})");
            isValidMove = false;
            return blockPos; 
        }

        Vector2 snappedPos = gridOffset + new Vector2(
            nearestX * totalCellSize.x,
            -nearestY * totalCellSize.y
        );

        if (blockSize.x == 2)
        {
            snappedPos.x += totalCellSize.x * 0.5f;
        }
        else if (blockSize.x == 3)
        {
            snappedPos.x += totalCellSize.x * 1.0f;
        }

        if (blockSize.y == 2)
        {
            snappedPos.y -= totalCellSize.y * 0.5f;
        }
        else if (blockSize.y == 3)
        {
            snappedPos.y -= totalCellSize.y * 1.0f;
        }

        bool withinBounds = (nearestX >= 0 && nearestX <= gridWidth - blockSize.x &&
                            nearestY >= 0 && nearestY <= gridHeight - blockSize.y);

        //Debug.Log($"BOUNDS DEBUG {block.name}: nearestX={nearestX}, nearestY={nearestY}, " +
        //          $"blockSize={blockSize}, gridWidth={gridWidth}, gridHeight={gridHeight}");
        //Debug.Log($"X bounds: {nearestX} >= 0 && {nearestX} <= {gridWidth - blockSize.x} = {nearestX >= 0 && nearestX <= gridWidth - blockSize.x}");
        //Debug.Log($"Y bounds: {nearestY} >= 0 && {nearestY} <= {gridHeight - blockSize.y} = {nearestY >= 0 && nearestY <= gridHeight - blockSize.y}");

        Canvas canvas = GetComponentInParent<Canvas>();
        Vector3 worldPos = canvas.transform.TransformPoint(snappedPos);

        bool noPhysicsCollision = IsPositionValidPhysics(blockComponent, worldPos);

        isValidMove = withinBounds && noPhysicsCollision;

        if (blockComponent != null && isValidMove)
        {
            blockComponent.UpdateGridPosition(new Vector2Int(nearestX, nearestY));
        }

        //Debug.Log($"Block {block.name}: Bounds OK: {withinBounds}, Physics OK: {noPhysicsCollision}, Final: {isValidMove}");

        return snappedPos;
    }

    public bool IsPositionValidPhysics(Block movingBlock, Vector2 targetWorldPosition)
    {
        BoxCollider2D blockCollider = movingBlock.GetComponent<BoxCollider2D>();
        if (blockCollider == null)
        {
            Debug.Log($"No collider found on {movingBlock.name} - assuming valid move");
            return true;
        }

        blockCollider.enabled = false;
        Vector2 colliderSize = blockCollider.size;

        Collider2D[] overlapping = Physics2D.OverlapBoxAll(targetWorldPosition, colliderSize, 0f);
        blockCollider.enabled = true;

        foreach (Collider2D other in overlapping)
        {
            Block otherBlock = other.GetComponent<Block>();
            if (otherBlock != null)
            {

                if (otherBlock.gameObject.name == "Exit")
                    continue;

                float overlapArea = CalculateOverlapArea(targetWorldPosition, colliderSize, other.transform.position, other.bounds.size);

                float threshold;
                Vector2Int blockSize = movingBlock.GetBlockSize();
                int totalCells = blockSize.x * blockSize.y;

                if (totalCells >= 3) 
                {
                    threshold = 6000f; 
                }
                else 
                {
                    threshold = 3500f; 
                }

                if (overlapArea > threshold)
                {
                    Debug.Log($"PHYSICS COLLISION: {movingBlock.name} would overlap with {otherBlock.name} (area: {overlapArea} > {threshold})");
                    return false;
                }
            }
        }
        return true;
    }

    private float CalculateOverlapArea(Vector2 pos1, Vector2 size1, Vector2 pos2, Vector2 size2)
    {
        float left1 = pos1.x - size1.x / 2f;
        float right1 = pos1.x + size1.x / 2f;
        float bottom1 = pos1.y - size1.y / 2f;
        float top1 = pos1.y + size1.y / 2f;

        float left2 = pos2.x - size2.x / 2f;
        float right2 = pos2.x + size2.x / 2f;
        float bottom2 = pos2.y - size2.y / 2f;
        float top2 = pos2.y + size2.y / 2f;

        float overlapWidth = Mathf.Max(0, Mathf.Min(right1, right2) - Mathf.Max(left1, left2));
        float overlapHeight = Mathf.Max(0, Mathf.Min(top1, top2) - Mathf.Max(bottom1, bottom2));

        return overlapWidth * overlapHeight;
    }

    public void FixColliderSizes()
    {
        Block[] allBlocks = FindObjectsOfType<Block>();

        foreach (Block block in allBlocks)
        {
            RectTransform rectTransform = block.GetComponent<RectTransform>();
            BoxCollider2D collider = block.GetComponent<BoxCollider2D>();

            if (collider != null && rectTransform != null)
            {
                collider.size = rectTransform.sizeDelta;

                Debug.Log($"Fixed {block.name} collider size to: {collider.size} (RectTransform size: {rectTransform.sizeDelta})");
            }
            else
            {
                Debug.LogError($"Missing components on {block.name}: Collider={collider != null}, RectTransform={rectTransform != null}");
            }
        }
    }

    public void ValidateColliderSizes()
    {
        Block[] allBlocks = FindObjectsOfType<Block>();

        foreach (Block block in allBlocks)
        {
            RectTransform rectTransform = block.GetComponent<RectTransform>();
            BoxCollider2D collider = block.GetComponent<BoxCollider2D>();

            if (collider != null && rectTransform != null)
            {
                Vector2 expectedSize = rectTransform.sizeDelta;
                Vector2 currentSize = collider.size;

                if (Vector2.Distance(expectedSize, currentSize) > 0.1f)
                {
                    Debug.LogWarning($"Collider size mismatch on {block.name}: Expected {expectedSize}, Got {currentSize}. Fixing...");
                    collider.size = expectedSize;
                }
            }
        }
    }

    public bool CheckWinCondition()
    {
        foreach (Vector2Int cell in goalBlock.GetOccupiedCells())
        {
            if (cell == exitCell)
            {
                Debug.Log("Puzzle Solved");

                MiniGameController controller = FindObjectOfType<MiniGameController>();
                if(controller != null)
                {
                    controller.OnMiniGamerWin();
                }

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
    
    private void InitializeBlockPositions()
    {
        Block[] allBlocks = FindObjectsOfType<Block>();
        
        foreach (Block block in allBlocks)
        {
            RectTransform blockRect = block.GetComponent<RectTransform>();
            Vector2 blockPos = blockRect.anchoredPosition;
            Vector2 gridOffset = new Vector2(-342.30f, 426.40f);
            Vector2 cellSize = slotGrid.cellSize;
            Vector2 spacing = slotGrid.spacing;
            Vector2 totalCellSize = cellSize + spacing;

            Vector2 adjustedBlockPos = blockPos;
            if (block.Size.x == 2)
            {
                adjustedBlockPos.x -= totalCellSize.x * 0.5f;
            }
            if (block.Size.y == 2)
            {
                adjustedBlockPos.y += totalCellSize.y * 0.5f;
            }

            Vector2 relativePos = adjustedBlockPos - gridOffset;
            int gridX = Mathf.RoundToInt(relativePos.x / totalCellSize.x);
            int gridY = Mathf.RoundToInt(-relativePos.y / totalCellSize.y);

            block.Position = new Vector2Int(gridX, gridY);
            //Debug.Log($"Initialized {block.name} at grid position {block.Position}");
        }
    }

}
