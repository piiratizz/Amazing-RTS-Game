using System;
using UnityEngine;

public class UnitAnimationsEventsHandler : MonoBehaviour
{
    public Action OnHitEvent;

    public void InvokeHitEvent()
    {
        OnHitEvent?.Invoke();
    }
}