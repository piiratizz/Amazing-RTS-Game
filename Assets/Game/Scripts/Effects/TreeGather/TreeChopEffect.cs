using DG.Tweening;
using NaughtyAttributes;
using NTC.Pool;
using UnityEngine;

public class TreeChopEffect : MonoBehaviour
{
    [SerializeField] private WoodChipsEffect woodChipsEffect;
    
    [Header("Chop Shake")]
    [SerializeField] private Transform visual;
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeStrength = 0.3f;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float randomness = 90f;
    
    [Header("Chop Shake")]
    [SerializeField] private Vector3 fallDirection = new Vector3(0, 0, 1);
    [SerializeField][MinMaxSlider(0f, 360f)] private Vector2 fallAngle;
    [SerializeField] private float fallDuration = 1.5f;
    [SerializeField] private float dropAmount = 0.3f;
    
    private Tween _currentTween;
    private bool _isFalling;
    
    public void DoChop(AnimationHitArgs args)
    {
        NightPool.Spawn(
            woodChipsEffect,
            args.Position,
            Quaternion.LookRotation(args.Author.transform.position - args.Position));
        
        _currentTween?.Kill();
        
        _currentTween = visual
            .DOShakeRotation(shakeDuration, shakeStrength * Vector3.one, vibrato, randomness, false)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => visual.localRotation = Quaternion.identity);
    }
    
    public void StartFalling()
    {
        if (_isFalling) return;
        _isFalling = true;
        
        var seq = DOTween.Sequence();
        
        var angle = Random.Range(fallAngle.x, fallAngle.y);
        
        seq.Append(transform.DOLocalRotate(fallDirection * angle, fallDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutCubic));
        
        seq.Join(transform.DOMoveY(transform.position.y - dropAmount, fallDuration * 0.5f)
            .SetEase(Ease.OutQuad));

        seq.Play();
    }
}