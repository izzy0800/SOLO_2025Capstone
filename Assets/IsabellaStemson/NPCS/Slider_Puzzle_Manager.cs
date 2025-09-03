using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider_Puzzle_Manager : MonoBehaviour
{

    public static Slider_Puzzle_Manager Instance;

    public RectTransform puzzleBoardRect;

    public GameObject puzzleBoardSlots;
    public PuzzleSlot[] puzzleSlots;

    public Vector2 cellSize;
    public Vector2 gridOrigin;

    public GameObject puzzlePanel;
    public int gridWidth = 6;
    public int gridHeight = 6;

    public Block[] blocks; //obsticals
    public Block goalBlock; //player

    public bool[,] grid;
    public Canvas canvas;

    //this is the Exit location of the grid (the goal of the mini game)
    public Vector2Int exitCell;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        puzzleSlots = puzzleBoardSlots.GetComponentsInChildren<PuzzleSlot>();
        for (int i = 0; i < puzzleSlots.Length; i++)
        {
            puzzleSlots[i].id = i;
        }

    }

    // Start is called before the first frame update
    private void Start()
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas not assigned or found in parent!");
            return;
        }

        //grabbing canvas height and width
        RectTransform boardRect = puzzleBoardRect; 
        float boardWidth = boardRect.rect.width;
        float boardHeight = boardRect.rect.height;

        cellSize = new Vector2(boardWidth / gridWidth, boardHeight / gridHeight);
        gridOrigin = new Vector2(-boardWidth / 2f, -boardHeight / 2f);

        grid = new bool[gridWidth, gridHeight];
        UpdateGrid();
    }

    public Vector2 GetNearestCellPosition(RectTransform block)
    {
        Vector2 size = block.sizeDelta;
        int widthCells = Mathf.RoundToInt(size.x / cellSize.x);
        int heightCells = Mathf.RoundToInt(size.y / cellSize.y);

        Vector2 localPos = block.anchoredPosition;

        int nearestX = Mathf.RoundToInt((localPos.x - gridOrigin.x) / cellSize.x);
        int nearestY = Mathf.RoundToInt((localPos.y - gridOrigin.y) / cellSize.y);

        nearestX = Mathf.Clamp(nearestX, 0, gridWidth - widthCells);
        nearestY = Mathf.Clamp(nearestY, 0, gridHeight - heightCells);

        //check for collisions with other blocks
        for (int checkY = nearestY; checkY < nearestY + heightCells; checkY++)
        {
            
            for (int checkX = nearestX; checkX < nearestX + widthCells; checkX++)
            {
                
                if (grid[checkX, checkY])
                {
                    return block.anchoredPosition;
                }
            }
        }

        float x = nearestX * cellSize.x + gridOrigin.x;
        float y = nearestY * cellSize.y + gridOrigin.y;

        return new Vector2(x, y);

    }

    // Update is called once per frame
    public void UpdateGrid()
    {
        grid = new bool[gridWidth, gridHeight];

        foreach (Block b in blocks)
        {
            foreach (Vector2Int cell in b.GetOccupiedCells())
            {
                if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
                {
                    Debug.LogError($"Block {b.name} has out-of-bounds cell: {cell}");
                    continue;
                }
                
                grid[cell.x, cell.y] = true;
            }
        }
    }

    public void SetBlockOccupied(Block b, bool occupied)
    {
        foreach (Vector2Int cell in b.GetOccupiedCells())
        {
            if (cell.x >= 0 && cell.x < gridWidth && cell.y >= 0 && cell.y < gridHeight)
            {
                grid[cell.x, cell.y] = occupied;
            }
        }
    }

    public bool CheckWinCondition()
    {
        List<Vector2Int> goalCells = goalBlock.GetOccupiedCells();

        foreach (Vector2Int cell in goalCells)
        {
            if (cell == exitCell)
            {
                Debug.Log("Puzzle Solved!");
                puzzlePanel.SetActive(false);
                Time.timeScale = 1f;
                return true;
            }
        }

        return false;
    }

    public void OnDrawGizmos()
    {
        if (gridWidth <= 0 || gridHeight <= 0 || puzzleBoardRect == null) return;
        Gizmos.color = Color.green;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 pos = puzzleBoardRect.position + new Vector3(
                    gridOrigin.x + x * cellSize.x / 2f,
                    gridOrigin.y + y * cellSize.y / 2f, 0);

                Vector3 size = new Vector3(cellSize.x, cellSize.y, 0);
                Gizmos.DrawWireCube(pos, size);
            }
        }
    }

}
