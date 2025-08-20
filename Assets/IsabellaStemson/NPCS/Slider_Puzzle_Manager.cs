using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider_Puzzle_Manager : MonoBehaviour
{

    public static Slider_Puzzle_Manager Instance;

    public Vector2 cellSize = new Vector2(100, 100);
    public Vector2 gridOrigin = Vector2.zero;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

    public GameObject puzzlePanel;
    public int gridWidth = 6;
    public int gridHeight = 6;
    public Block[] blocks; //obsticals
    public Block goalBlock; //player

    public bool[,] grid;

    //this is the Exit location of the grid (the goal of the mini game)
    public Vector2Int exitCell;

    // Start is called before the first frame update
    private void Start()
    { 
        grid = new bool[gridWidth, gridHeight];
        UpdateGrid();
    }

    // Update is called once per frame
    public void UpdateGrid()
    {
        grid = new bool[gridWidth, gridHeight];

        foreach (Block b in blocks)
        {
            foreach (Vector2Int cell in b.GetOccupiedCells())
            {
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

}
