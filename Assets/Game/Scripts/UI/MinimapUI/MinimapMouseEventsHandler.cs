using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapMouseEventsHandler : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private RawImage minimapImage;
    
    public Action<Vector2> OnMapClicked;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransform rectTransform = minimapImage.rectTransform;
        
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
        {
            Vector2 normalized = new Vector2(
                Mathf.InverseLerp(rectTransform.rect.xMin, rectTransform.rect.xMax, localPoint.x),
                Mathf.InverseLerp(rectTransform.rect.yMin, rectTransform.rect.yMax, localPoint.y)
            );

            OnMapClicked?.Invoke(normalized);
        }
    }
}