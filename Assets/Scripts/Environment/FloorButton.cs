using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : MonoBehaviour
{
    [SerializeField] private GameObject door;

    private List<Collider> touchingColliders = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !touchingColliders.Contains(other))
        {
            Debug.Log("touching collider");
            touchingColliders.Add(other);
            door.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && touchingColliders.Contains(other))
        {
            touchingColliders.Remove(other);
        }

        if (touchingColliders.Count == 0)
            door.SetActive(true);
    }
}