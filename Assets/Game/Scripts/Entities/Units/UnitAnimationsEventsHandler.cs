using System;
using UnityEngine;

public class UnitAnimationsEventsHandler : MonoBehaviour
{
    [SerializeField] private Transform hitPoint;
    [SerializeField] private Entity author;
    
    public Action<AnimationHitArgs> OnHitEvent;
    public Action OnProjectileLauncherEvent;
    
    public void InvokeHitEvent()
    {
        AnimationHitArgs args =  new AnimationHitArgs
        {
            Position = hitPoint == null ? Vector3.zero : hitPoint.position,
            Author = author,
        };
        
        OnHitEvent?.Invoke(args);
    }

    public void InvokeProjectileLaunchEvent()
    {
        OnProjectileLauncherEvent?.Invoke();
    }
}

public struct AnimationHitArgs
{
    public Vector3 Position;
    public Entity Author;
}