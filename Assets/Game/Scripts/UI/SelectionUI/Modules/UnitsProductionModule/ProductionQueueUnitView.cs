using System;
using UnityEngine;
using UnityEngine.UI;

public class ProductionQueueUnitView : MonoBehaviour
{
    [SerializeField] private Image previewImage;

    private int _queueId;
    private UnitConfig _config;
    private Action<int> _onCancelCallback;
    
    public void Initialize(int id, UnitConfig config, Action<int> onCancelCallback)
    {
        _queueId = id;
        _config = config;
        previewImage.sprite = _config.Icon;
        _onCancelCallback = onCancelCallback;
    }

    public void OnCancel()
    {
        _onCancelCallback.Invoke(_queueId);
    }
}