using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform cam;

    private void Update()
    {
        if(!cam)cam = Camera.main.transform;

        Vector3 pos = cam.position;
        Vector3 dir = pos - transform.position;
        if(dir!= Vector3.zero)
            transform.forward = -dir.normalized;
    }
}
