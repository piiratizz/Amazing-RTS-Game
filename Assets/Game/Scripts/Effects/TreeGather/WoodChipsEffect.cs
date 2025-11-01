using System;
using System.Collections;
using NTC.Pool;
using UnityEngine;

public class WoodChipsEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;

    private void OnEnable()
    {
        particleSystem.Play();
        StartCoroutine(CheckIfFinished());
    }
    
    private IEnumerator CheckIfFinished()
    {
        yield return new WaitWhile(particleSystem.IsAlive);

        NightPool.Despawn(gameObject);
    }
}