using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildMenuItemView : MonoBehaviour, IDisposable
{
    [SerializeField] private Image previewImage;
    [SerializeField] private Button button;

    public void Initialize(Sprite preview, UnityAction onClickCallback)
    {
        previewImage.sprite = preview;
        button.onClick.AddListener(onClickCallback);
    }
    
    public void Dispose()
    {
        button.onClick.RemoveAllListeners();
    }
}