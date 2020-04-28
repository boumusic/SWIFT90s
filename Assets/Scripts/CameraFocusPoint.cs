using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusPoint : MonoBehaviour
{
    private void OnEnable()
    {
        GameCamera.Instance.RegisterFocusPoint(this);
    }

    private void OnDisable()
    {
        if (GameCamera.Instance != null)
            GameCamera.Instance.UnregisterFocusPoint(this);
    }
}
