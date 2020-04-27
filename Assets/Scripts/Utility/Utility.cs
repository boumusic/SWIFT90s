using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utility
{
    public static float Vector2ToAngle(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        angle = angle * Mathf.Rad2Deg;

        return angle;
    }

    /// <summary>
    /// Method used to return a number clamped between a min and max value, based on where a variable interpolates between its min and max value.
    /// </summary>
    /// <param name="minValueReturn">The minimum of the returned value.</param>
    /// <param name="maxValueReturn">The maximum of the returned value.</param>
    /// <param name="minValueToCheck">The minimum of the value we're checking.</param>
    /// <param name="maxValueToCheck">The maximum of the value we're checking.</param>
    /// <param name="valueToCheck">The value we're checking.</param>
    public static float Interpolate(float minValueReturn, float maxValueReturn, float minValueToCheck, float maxValueToCheck, float valueToCheck)
    {
        return Mathf.LerpUnclamped(minValueReturn, maxValueReturn, Mathf.InverseLerp(minValueToCheck, maxValueToCheck, valueToCheck));
    }

    public static float BoolToFloat(bool boolean)
    {
        if (boolean) return 1f;
        else return 0f;
    }

    public static float Derivative(this AnimationCurve self, float time)
    {
        if (self == null) return 0.0f;
        for (int i = 0; i < self.length - 1; i++)
        {
            if (time < self[i].time) continue;
            if (time > self[i + 1].time) continue;
            return Derivative(self[i], self[i + 1], (time - self[i].time) / (self[i + 1].time - self[i].time));
        }
        return 0.0f;
    }

    private static float Derivative(Keyframe from, Keyframe to, float lerp)
    {
        float dt = to.time - from.time;

        float m0 = from.outTangent * dt;
        float m1 = to.inTangent * dt;

        float lerp2 = lerp * lerp;

        float a = 6.0f * lerp2 - 6.0f * lerp;
        float b = 3.0f * lerp2 - 4.0f * lerp + 1.0f;
        float c = 3.0f * lerp2 - 2.0f * lerp;
        float d = -a;

        return a * from.value + b * m0 + c * m1 + d * to.value;
    }

    public static Vector3 SmoothDampAngle (this Vector3 vector, Vector3 to, ref Vector3 velocity, float smoothing, float maxSpeed, float deltaTime)
    {
        Vector3 result;

        result.x = Mathf.SmoothDampAngle(vector.x, to.x, ref velocity.x, smoothing, maxSpeed, deltaTime);
        result.y = Mathf.SmoothDampAngle(vector.y, to.y, ref velocity.y, smoothing, maxSpeed, deltaTime);
        result.z = Mathf.SmoothDampAngle(vector.z, to.z, ref velocity.z, smoothing, maxSpeed, deltaTime);

        return result;
    }

    public static Vector3 Flat(this Vector3 vector)
    {
        Vector3 result;

        result.x = vector.x;
        result.y = 0;
        result.z = vector.z;

        return result;
    }

    public static float ClosestTo(this List<float> collection, float target)
    {

        var closest = float.MaxValue;
        var minDifference = float.MaxValue;
        for (int i = 0; i < collection.Count; i++)
        {
            var difference = Mathf.Abs(collection[i] - target);
            if (minDifference > difference)
            {
                minDifference = difference;
                closest = collection[i];
            }
        }

        return closest;
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }

    public static Vector2 WorldToCanvas(this Canvas canvas, Vector3 world_position, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        var viewport_position = camera.WorldToViewportPoint(world_position);
        var canvas_rect = canvas.GetComponent<RectTransform>();

        return new Vector2((viewport_position.x * canvas_rect.sizeDelta.x) - (canvas_rect.sizeDelta.x * 0.5f),
                           (viewport_position.y * canvas_rect.sizeDelta.y) - (canvas_rect.sizeDelta.y * 0.5f));
    }
}

[System.Serializable]
public struct MinMax
{
    public float min;
    public float max;

    public float Lerp(float t)
    {
        return Mathf.Lerp(min, max, t);
    }
}