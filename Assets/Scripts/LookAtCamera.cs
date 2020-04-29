using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform cam;

    private void Update()
    {
        if (cam == null)
            return;

        Vector3 pos = cam.position;
        Vector3 dir = pos - transform.position;
        transform.forward = dir.normalized;
    }
}
