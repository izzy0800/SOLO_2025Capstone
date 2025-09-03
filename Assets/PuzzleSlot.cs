using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    public int id;
    public Block occupiedBlock;

    public void PlaceBlock(Block b)
    {
        occupiedBlock = b;
    }

    public void RemoveBlock(Block b)
    {
        occupiedBlock = null;
    }
}
