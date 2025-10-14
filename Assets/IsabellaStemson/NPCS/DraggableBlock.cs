using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPos;

    private Slider_Puzzle_Manager manager;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        manager = GetComponentInParent<Slider_Puzzle_Manager>();
    }

    

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Block blockComponent = GetComponent<Block>();
        Vector2 delta = eventData.delta / canvas.scaleFactor;

        if (blockComponent != null)
        {
            if (blockComponent.MovesHorizontally)
            {
                delta.y = 0;
            }
            else if (blockComponent.MovesVertically)
            {
                delta.x = 0;
            }
        }

        rectTransform.anchoredPosition += delta;

        manager.goalBlock.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Vector2 originalAnchoredPos = originalPos;
        bool isValidMove;
        Vector2 snapped = manager.GetNearestCellPositionWithCollision(rectTransform, out isValidMove);

        if (isValidMove)
        {
            rectTransform.anchoredPosition = snapped;
            Debug.Log("Valid move applied");

            manager.goalBlock.GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            rectTransform.anchoredPosition = originalPos;
            Debug.Log("Invalid move - returned to original position");
        }

        //Block blockComponent = GetComponent<Block>();
        //if (blockComponent != null)
        //{
        //    Vector2 relativePos = snapped - new Vector2(-342.30f, 426.40f);
        //    Vector2 totalCellSize = new Vector2(137.5f, 171.1f); //wrong maybe?

        //    int targetX = Mathf.RoundToInt(relativePos.x / totalCellSize.x);
        //    int targetY = Mathf.RoundToInt(-relativePos.y / totalCellSize.y);
        //    Vector2Int targetGridPos = new Vector2Int(targetX, targetY);

        //    if (Slider_Puzzle_Manager.Instance.IsPositionValid(blockComponent, targetGridPos))
        //    {
        //        rectTransform.anchoredPosition = snapped;
        //        blockComponent.UpdateGridPosition(targetGridPos);
        //        Debug.Log($"{blockComponent.name} move to valid position {targetGridPos}");
        //    }
        //    else
        //    {
        //        rectTransform.anchoredPosition = originalPos;
        //        Debug.Log($"{blockComponent.name} blocked by collision, staying at {blockComponent.Position}");
        //    }
        //}
        //else
        //{
        //    rectTransform.anchoredPosition = snapped;
        //}

        manager.UpdateGrid();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("IM A BLOCK");
        //MAKE SURE THAT THE CANVAS 'BLOCKS RAYCASTS' IS TICKED FOR SOME REASON
    }
}
