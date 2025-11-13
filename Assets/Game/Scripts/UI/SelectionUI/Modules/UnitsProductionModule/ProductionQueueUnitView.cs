using System;
using UnityEngine;
using UnityEngine.UI;

public class ProductionQueueUnitView : MonoBehaviour
{
    [SerializeField] private Image previewImage;

    private int _queueId;
    private Action<int> _onCancelCallback;
    
    public void Initialize(int id, Sprite icon, Action<int> onCancelCallback)
    {
        _queueId = id;
        previewImage.sprite = icon;
        _onCancelCallback = onCancelCallback;
    }

    public void OnCancel()
    {
        _onCancelCallback.Invoke(_queueId);
    }
}