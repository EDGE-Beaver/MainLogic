using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    private float moveSpeed = 2f;
    private float moveDistance = 10f;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(originalPosition.x, originalPosition.y + y, originalPosition.z);
    }
}
