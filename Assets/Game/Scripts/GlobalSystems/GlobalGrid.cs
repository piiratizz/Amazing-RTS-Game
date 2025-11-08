using System;
using UnityEngine;

public class GlobalGrid : MonoBehaviour
{
    [SerializeField] private Grid grid;

    public Vector3 WorldToCell(Vector3 position) => grid.WorldToCell(position);
}