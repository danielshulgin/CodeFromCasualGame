using UnityEngine;

namespace Generic
{
    public class HelperFunctions
    {
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
  
        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public static float RangeToRange(float value, float oldMin, float  oldMax, float newMin, float newMax)
        {
            var oldRange = (oldMax - oldMin);
            var newRange = (newMax - newMin);
            return (((value - oldMin) * newRange) / oldRange) + newMin;
        }
        
        public static bool CheckInternetConnection()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}