using System.Collections.Generic;
using UnityEngine;

public class BlockGame : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] int width;
    [SerializeField] int height;

    public List<GridTile> grid = new List<GridTile>();

    [Header("Win Condition")]
    [SerializeField] Tile goalBlock;
    [SerializeField] Vector2 exitPosition = new Vector2Int(5, 2);

    private MiniGameController miniGameController;

    private void Start()
    {
        miniGameController = FindObjectOfType<MiniGameController>();
        if (grid.Count == 0)
        {
            CreateGrid();
        }
    }

    [ContextMenu("Create Grid")]
    public void CreateGrid()
    {
        grid.Clear();
        for(int x = 0; x < height; x++)
        {
            for(int y = 0; y < width; y++)
            {
                GridTile newTile = new GridTile()
                {
                    x = x,
                    y = y,
                };

                grid.Add(newTile);
            }
        }
    }

    public void SetTile(Tile tile, int x, int y) 
    { 
        foreach(GridTile gTile in grid)
        {
            if(gTile.x == x && gTile.y == y)
            {
                gTile.tile = tile;
            }
        }
        if(tile == goalBlock)
        {
            CheckWinCondition();
        }
    }

    public void RemoveTile(int x, int y)
    {
        foreach (GridTile gTile in grid)
        {
            if (gTile.x == x && gTile.y == y)
            {
                gTile.tile = null;
            }
        }
    }

    public bool IsSafe(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return false;

        foreach (GridTile gTile in grid)
        {
            if (gTile.x == x && gTile.y == y)
            {
                if(gTile.tile == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }

    void CheckWinCondition()
    {
        if (goalBlock == null) return;
        foreach (Transform child in goalBlock.transform)
        {
            Vector3 localPos = goalBlock.transform.parent.InverseTransformPoint(child.position) / 100;
            int x = Mathf.RoundToInt(localPos.x);
            int y = Mathf.RoundToInt(localPos.y);

            if (x == exitPosition.x && y == exitPosition.y)
            {
                Debug.Log("Puzzle Solved!");
                OnPuzzleWin();
                return;
            }
        }
    }

    void OnPuzzleWin()
    {
        if (miniGameController != null)
        {
            miniGameController.OnMiniGamerWin();
        }
    }

    private void OnDrawGizmos()
    {
        if (grid == null || grid.Count == 0) return;

        Gizmos.color = Color.gray;
        for (int x = 0; x <= width; x++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(x * 100, 0, 0),
                transform.position + new Vector3(x * 100, height * 100, 0));
        }
        for (int y = 0; y <= height; y++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(0, y * 100, 0),
                transform.position + new Vector3(width * 100, y * 100, 0));
        }

        Gizmos.color = Color.green;
        Vector3 exitWorldPos = transform.position + new Vector3(exitPosition.x * 100 + 50, 0);
        Gizmos.DrawWireCube(exitWorldPos, new Vector3(80, 80, 10));

    }

}

[System.Serializable]
public class GridTile
{
    public int y;
    public int x;
    public Tile tile;
}
