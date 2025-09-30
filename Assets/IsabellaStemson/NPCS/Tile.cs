using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("References")]
    public BlockGame game;
    public Canvas canvas;

    [Header("Movement Settings")]
    public MoveDirection moveDirection;
    public int currentX;
    public int currentY;

    [Header("Tile Properties")]
    public bool isGoalBlock = false;
    public Vector2Int tileSize = Vector2Int.one;

    RectTransform rectTransform;
    private Vector3 originalPosition;
    //public bool isBlocked;

    public enum MoveDirection
    {
        Horizontal,
        Vertical,
        None
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        if (game == null)
            game = FindAnyObjectByType<BlockGame>();
    }

    private void Start()
    {
        SetPosition(currentX, currentY);

        if (moveDirection == MoveDirection.None) return;
        if (tileSize.x > tileSize.y)
            moveDirection = MoveDirection.Horizontal;
        else if (tileSize.y > tileSize.x)
            moveDirection = MoveDirection.Vertical;

        //rectTransform = GetComponent<RectTransform>();
        //SetPosition(currentX, currentY);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (moveDirection == MoveDirection.None) return;
        originalPosition = transform.localPosition;

        foreach (Transform child in transform)
        {
            Vector3 localPos = transform.parent.InverseTransformPoint(child.position) / 100;
            int x = Mathf.RoundToInt(localPos.x);
            int y = Mathf.RoundToInt(localPos.y);
            game.RemoveTile(x, y);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (moveDirection == MoveDirection.None) return;

        Vector2 delta = eventData.delta / canvas.scaleFactor;
        switch (moveDirection)
        {
            case MoveDirection.Horizontal:
                delta.y = 0;
                break;
            case MoveDirection.Vertical:
                delta.x = 0;
                break;
        }

        rectTransform.anchoredPosition += delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (moveDirection == MoveDirection.None) return;

        Vector3 refPos = transform.localPosition / 100;
        int roundX = Mathf.RoundToInt(refPos.x);
        int roundY = Mathf.RoundToInt(refPos.y);

        bool isSafe = true;
        foreach(Transform child in transform)
        {

            Vector3 localPos = transform.parent.InverseTransformPoint(child.position) / 100;
            int x = Mathf.RoundToInt(localPos.x);
            int y = Mathf.RoundToInt(localPos.y);
            
            if (!game.IsSafe(x, y))
            {
                isSafe = false;
                break;
            }
        }

        if (isSafe)
        {
            SetPosition(roundX, roundY);
        }
        else
        {
            ReturnPosition();
        }
    }

    void SetPosition(int x, int y)
    {
        transform.localPosition = new Vector3(x * 100, y * 100);
        currentX = x;
        currentY = y;

        foreach (Transform child in transform)
        {
            Vector3 localPos = transform.parent.InverseTransformPoint(child.position) / 100;
            int tilex = Mathf.RoundToInt(localPos.x);
            int tiley = Mathf.RoundToInt(localPos.y);
            game.SetTile(this, tilex, tiley);
        }
    }

    void ReturnPosition()
    {
        //transform.localPosition = new Vector3(currentX * 100, currentY * 100);
        transform.localPosition = originalPosition;

        foreach (Transform child in transform)
        {
            Vector3 localPos = transform.parent.InverseTransformPoint(child.position) / 100;
            int x = Mathf.RoundToInt(localPos.x);
            int y = Mathf.RoundToInt(localPos.y);
            game.SetTile(this, x, y);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = isGoalBlock ? Color.yellow : Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(tileSize.x * 90, tileSize.y * 90, 10));
    }

}
