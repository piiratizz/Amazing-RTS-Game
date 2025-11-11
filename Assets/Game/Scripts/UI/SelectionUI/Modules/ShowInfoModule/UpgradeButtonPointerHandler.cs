using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButtonPointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action PointerEnterEvent;
    public Action PointerExitEvent;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent?.Invoke();
    }
}