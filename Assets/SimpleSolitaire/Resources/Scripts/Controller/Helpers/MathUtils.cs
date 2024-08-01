using UnityEngine;

namespace SimpleSolitaire
{
    public static class MathUtils
    {
        public static Vector3 EvaluateBezierValue(Vector3 point1, Vector3 point2, Vector3 point3, float time)
        {
            time = Mathf.Clamp01(time);
            return Mathf.Pow(1 - time, 2) * point1 + 2 * (1 - time) * time * point2 + Mathf.Pow(time, 2) * point3;
        }
        
        public static float EvaluateQuadraticValue(float increment, float time)
        {
            time = Mathf.Clamp01(time);
            return 1f + increment * (4 * time * (1 - time));
        }

        public static float GetRandomBetween(float min, float max)
        {
            return Random.Range(min, max);
        }
    }
}