using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPos;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Vector2 originalAnchoredPos = originalPos;
        bool isValidMove;
        Vector2 snapped = Slider_Puzzle_Manager.Instance.GetNearestCellPositionWithCollision(rectTransform, out isValidMove);

        if (isValidMove)
        {
            rectTransform.anchoredPosition = snapped;
            Debug.Log("Valid move applied");
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

        Slider_Puzzle_Manager.Instance.UpdateGrid();
        Slider_Puzzle_Manager.Instance.CheckWinCondition();
    }

}
