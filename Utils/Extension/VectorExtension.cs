using UnityEngine;

namespace Maniac.Utils.Extension
{
    public static class VectorExtension
    {
        public static Vector2 AddAngle(this Vector2 direction,float addAngle)
        {
            // return new Vector2(Mathf.Cos(addAngle), Mathf.Sin(addAngle)) * direction.magnitude;
            return Quaternion.AngleAxis(addAngle, Vector3.forward) * direction;
        }

        public static Vector2 NormalizedM(this Vector2 vector)
        {
            return vector / Mathf.Sqrt(vector.sqrMagnitude);
        }
        
        public static Vector3 NormalizedM(this Vector3 vector)
        {
            return vector / Mathf.Sqrt(vector.sqrMagnitude);
        }
    }
}