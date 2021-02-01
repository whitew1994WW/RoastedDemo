using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f;

    [Range(0, 1)] [SerializeField] float movementFactor;

    Vector3 startingPosition;
    float startingOffset;

    void Start()
    {
        startingPosition = transform.position;
        startingOffset = movementFactor * period;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }

        float sineFactor = ((Time.time + startingOffset) / period);
        float rawSinWave = Mathf.Sin(sineFactor * 2 * Mathf.PI);

        movementFactor = (rawSinWave + 1) / 2f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + transform.right * offset.x +
            transform.up * offset.y + transform.forward * offset.z;
    }
}