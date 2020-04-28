using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    private static GameCamera instance;
    public static GameCamera Instance { get { if (!instance) instance = FindObjectOfType<GameCamera>(); return instance; } }

    [Header("Position")]
    [SerializeField] private float posSmoothness = 0.2f;
    private Vector3 currentVel;

    [SerializeField] private float minZ = 7;
    [SerializeField] private float maxZ = 30f;
    [SerializeField] private float maxPlayerDistance = 30f;

    private List<CameraFocusPoint> focusPoints = new List<CameraFocusPoint>();

    public void RegisterFocusPoint(CameraFocusPoint point)
    {
        focusPoints.Add(point);
    }

    public void UnregisterFocusPoint(CameraFocusPoint point)
    {
        focusPoints.Remove(point);
    }

    private void Update()
    {
        Position();
    }

    private void Position()
    {
        Vector3 target = AveragePos();
        float z = Utility.Interpolate(minZ, maxZ, 0, maxPlayerDistance, GetGreatestDistance());
        target = new Vector3(target.x, target.y, -z);
        Vector3 pos = Vector3.SmoothDamp(transform.position, target, ref currentVel, posSmoothness);

        transform.position = pos;
    }

    public Vector3 AveragePos()
    {
        if (focusPoints.Count > 0)
        {
            Vector3 pos = Vector3.zero;
            for (int i = 0; i < focusPoints.Count; i++)
            {
                pos += focusPoints[i].transform.position;
            }

            return pos / focusPoints.Count;
        }

        else
        {
            return Vector3.zero;
        }
    }

    public float GetGreatestDistance()
    {
        float greatest = 0f;
        for (int x = 0; x < focusPoints.Count; x++)
        {
            for (int y = 0; y < focusPoints.Count; y++)
            {
                float distance = Vector3.Distance(focusPoints[x].transform.position, focusPoints[y].transform.position);
                if (distance > greatest)
                {
                    greatest = distance;
                }
            }
            
        }

        return greatest;
    }
}
