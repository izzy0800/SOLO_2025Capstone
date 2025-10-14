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
    //public static Slider_Puzzle_Manager Instance;

    public RectTransform puzzleBoardRect;
    public GridLayoutGroup slotGrid;

    public int gridWidth = 6;
    public int gridHeight = 6;

    public Vector2 cellSize;
    public bool[,] grid;

    public Block goalBlock;
    public Vector2Int exitCell;

    public void UpdateGrid() { }

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    Instance = this;
    //}

    private void Start()
    {
        SetPuzzle();
    }

    public void SetPuzzle()
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
        Debug.Log("Puzzle init");
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
        Vector2 gridOffset = new Vector2(-342.30f, 426.40f);  //DONT FUCKING TOUCH

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

        bool withinBounds = (nearestX >= 0 && nearestX <= gridWidth - blockSize.x &&
                            nearestY >= 0 && nearestY <= gridHeight - blockSize.y);

        if (!withinBounds)
        {
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

        Vector2Int targetGridPos = new Vector2Int(nearestX, nearestY);
        bool gridValid = IsPositionValidGrid(blockComponent, targetGridPos);

        Canvas canvas = GetComponentInParent<Canvas>();
        Vector3 worldPos = canvas.transform.TransformPoint(snappedPos);
        bool physicsValid = true; 

        // physicsValid = IsPositionValidPhysics(blockComponent, worldPos);

        isValidMove = withinBounds && gridValid;

        if (isValidMove)
        {
            Debug.Log($"{block.name} - Grid: {gridValid}, Final: Valid");
        }
        else
        {
            Debug.Log($"{block.name} - Grid: {gridValid}, Final: Invalid");
        }

        if (blockComponent != null && isValidMove)
        {
            blockComponent.UpdateGridPosition(targetGridPos);
        }

        return isValidMove ? snappedPos : blockPos;
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

        Vector2 checkSize = colliderSize * 0.9f;

        Collider2D[] overlapping = Physics2D.OverlapBoxAll(targetWorldPosition, checkSize, 0f);
        blockCollider.enabled = true;

        foreach (Collider2D other in overlapping)
        {
            Block otherBlock = other.GetComponent<Block>();
            if (otherBlock != null)
            {
                if (otherBlock.gameObject.name == "Exit")
                    continue;

                Vector2 distance = new Vector2(
                    Mathf.Abs(targetWorldPosition.x - other.transform.position.x),
                    Mathf.Abs(targetWorldPosition.y - other.transform.position.y)
                );

                Vector2 minDistance = new Vector2(
                    (colliderSize.x + other.bounds.size.x) / 2f - slotGrid.spacing.x * 0.5f,
                    (colliderSize.y + other.bounds.size.y) / 2f - slotGrid.spacing.y * 0.5f
                );

                bool isHorizontalMover = movingBlock.MovesHorizontally;
                bool isVerticalMover = movingBlock.MovesVertically;

                bool collision = false;

                if (isHorizontalMover)
                {
                    if (distance.x < minDistance.x && distance.y < minDistance.y + slotGrid.spacing.y)
                    {
                        collision = true;
                        Debug.Log($"H-COLLISION: {movingBlock.name} → {otherBlock.name}");
                        Debug.Log($"  X-distance: {distance.x:F1} < {minDistance.x:F1}");
                        Debug.Log($"  Y-distance: {distance.y:F1} < {minDistance.y + slotGrid.spacing.y:F1}");
                    }
                }
                else if (isVerticalMover)
                {
                    if (distance.y < minDistance.y && distance.x < minDistance.x + slotGrid.spacing.x)
                    {
                        collision = true;
                        Debug.Log($"V-COLLISION: {movingBlock.name} → {otherBlock.name}");
                        Debug.Log($"  X-distance: {distance.x:F1} < {minDistance.x + slotGrid.spacing.x:F1}");
                        Debug.Log($"  Y-distance: {distance.y:F1} < {minDistance.y:F1}");
                    }
                }
                else
                {
                    if (distance.x < minDistance.x && distance.y < minDistance.y)
                    {
                        collision = true;
                        Debug.Log($"S-COLLISION: {movingBlock.name} → {otherBlock.name}");
                        Debug.Log($"  X-distance: {distance.x:F1} < {minDistance.x:F1}");
                        Debug.Log($"  Y-distance: {distance.y:F1} < {minDistance.y:F1}");
                    }
                }

                if (collision)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private float CalculateAxisOverlap(float pos1, float size1, float pos2, float size2)
    {
        float min1 = pos1 - size1 / 2f;
        float max1 = pos1 + size1 / 2f;
        float min2 = pos2 - size2 / 2f;
        float max2 = pos2 + size2 / 2f;

        float overlapStart = Mathf.Max(min1, min2);
        float overlapEnd = Mathf.Min(max1, max2);

        return Mathf.Max(0, overlapEnd - overlapStart);
    }

    public bool IsPositionValidGrid(Block movingBlock, Vector2Int targetGridPos)
    {
        List<Vector2Int> targetCells = movingBlock.GetOccupiedCells(targetGridPos);

        Block[] allBlocks = FindObjectsOfType<Block>();
        foreach (Block otherBlock in allBlocks)
        {
            if (otherBlock == movingBlock || otherBlock.gameObject.name == "Exit")
                continue;

            List<Vector2Int> otherCells = otherBlock.GetOccupiedCells();

            foreach (Vector2Int targetCell in targetCells)
            {
                if (otherCells.Contains(targetCell))
                {
                    Debug.Log($"GRID COLLISION: {movingBlock.name} cell {targetCell} occupied by {otherBlock.name}");
                    return false;
                }
            }
        }

        return true;
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

                //Debug.Log($"Fixed {block.name} collider size to: {collider.size} (RectTransform size: {rectTransform.sizeDelta})");
            }
            else
            {
                //Debug.LogError($"Missing components on {block.name}: Collider={collider != null}, RectTransform={rectTransform != null}");
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
                if (controller != null)
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
            Debug.Log($"Initialized {block.name} at grid position {block.Position}");
        }
    }
}
