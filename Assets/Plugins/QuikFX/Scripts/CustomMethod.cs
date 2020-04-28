using UnityEngine;

namespace Pataya.Utility
{
    public static class CustomMethod
    {
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
    }
}

