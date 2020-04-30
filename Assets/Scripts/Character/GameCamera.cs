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
    [SerializeField] private float yOffset = 1f;

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
        target = new Vector3(target.x, target.y + yOffset, -z);
        Vector3 pos = Vector3.SmoothDamp(transform.position, target, ref currentVel, posSmoothness);

        transform.position = pos;
    }

    public Vector3 AveragePos()
    {
        Vector3 topRight = GetTopRightPoint();
        Vector3 bottomLeft = GetBottomLeftPoint();
        return (topRight + bottomLeft) / 2;
    }

    public Vector3 GetTopRightPoint()
    {
        if(focusPoints.Count > 0)
        {
            int topRightest = 0;
            for (int i = 0; i < focusPoints.Count; i++)
            {
                if (focusPoints[i].transform.position.x >= focusPoints[topRightest].transform.position.x)
                {
                        topRightest = i;
                    if (focusPoints[i].transform.position.y >= focusPoints[topRightest].transform.position.y)
                    {
                    }
                }
            }

            return focusPoints[topRightest].transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 GetBottomLeftPoint()
    {
        if (focusPoints.Count > 0)
        {
            int bottomLeftest = 0;
            for (int i = 0; i < focusPoints.Count; i++)
            {
                if (focusPoints[i].transform.position.x <= focusPoints[bottomLeftest].transform.position.x)
                {
                        bottomLeftest = i;
                    if (focusPoints[i].transform.position.y <= focusPoints[bottomLeftest].transform.position.y)
                    {
                    }
                }
            }
            return focusPoints[bottomLeftest].transform.position;
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
