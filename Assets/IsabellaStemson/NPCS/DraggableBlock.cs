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

        Vector2 snapped = Slider_Puzzle_Manager.Instance.GetNearestCellPosition(rectTransform);
        rectTransform.anchoredPosition = snapped;
        Slider_Puzzle_Manager.Instance.UpdateGrid();
        Slider_Puzzle_Manager.Instance.CheckWinCondition();
    }

}
