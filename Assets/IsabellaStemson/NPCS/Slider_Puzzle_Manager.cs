using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider_Puzzle_Manager : MonoBehaviour
{

    public int gridWidth = 6;
    public int gridHeight = 6;
    public Block[] blocks; //obsticals
    public Block goalBlock; //player

    public bool[,] grid;

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

    public bool CheckWinCondition()
    {
        return goalBlock.Position.x + goalBlock.Size.x >= gridWidth;
    }

}
