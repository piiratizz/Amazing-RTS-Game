using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField] private RawImage backgroundImage1;
    [SerializeField] private RawImage backgroundImage2;

    [SerializeField] private Vector3 scaleTo;
    [SerializeField] private float swapTime;
    [SerializeField] private float colorChangeTime;
    
    private Texture2D[] _backgroundTextures;

    private Vector3 _backgroundScale;
    private RawImage _activeBackgroundImage;
    
    private void Start()
    {
        _backgroundTextures = Resources.LoadAll<Texture2D>("Backgrounds");

        if(_backgroundTextures == null) return;

        _activeBackgroundImage = backgroundImage1;
        _activeBackgroundImage.gameObject.SetActive(true);
        _backgroundScale = _activeBackgroundImage.rectTransform.localScale;
        _activeBackgroundImage.texture = _backgroundTextures[Random.Range(0, _backgroundTextures.Length)];
        
        StartCoroutine(StartAnimationCycle());
    }

    private IEnumerator StartAnimationCycle()
    {
        while (true)
        {
            _activeBackgroundImage.rectTransform.DOScale(scaleTo, swapTime);
            yield return new WaitForSeconds(swapTime);
            SwapImage();
        }
    }

    private void SwapImage()
    {
        
        _activeBackgroundImage.rectTransform.localScale = _backgroundScale;
        _activeBackgroundImage.gameObject.SetActive(false);
        
        if (_activeBackgroundImage == backgroundImage2)
        {
            _activeBackgroundImage = backgroundImage1;
        }
        else if (_activeBackgroundImage == backgroundImage1)
        {
            _activeBackgroundImage = backgroundImage2;
        }
        
        _activeBackgroundImage.texture = _backgroundTextures[Random.Range(0, _backgroundTextures.Length-1)];
        _backgroundScale = _activeBackgroundImage.rectTransform.localScale;
        _activeBackgroundImage.gameObject.SetActive(true);
    }
}
