using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ThinWall : MonoBehaviour
{
    [SerializeField] List<SimpleFracture> fractures;

    public void BreakWall()
    {
        foreach (SimpleFracture fracture in fractures)
            fracture.ExecuteShatter();

        gameObject.SetActive(false);
    }
}