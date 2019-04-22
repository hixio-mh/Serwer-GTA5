using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace Extend.Maths
{
    public static class CMathExtension
    {
        public static System.Single ToRadian(this System.Single number)
        {
            return (float)(System.Math.PI * number / 180.0);
        }
        public static System.Single ToDegree(this System.Single number)
        {
            return (float)(number * (180 / System.Math.PI));
        }

        public static Vector3 GetPointFromDistanceRotation(this Vector3 vector, float dist, float angle)
        {
            double r = (90.0f - angle).ToRadian();
            double x = Math.Cos(r) * dist;
            double y = Math.Sin(r) * dist;
            return new Vector3(vector.X + x, vector.Y + y, vector.Z);
        }
        public static string ToStr(this Vector3 vector)
        {
            return String.Format("{0:F2},{1:F2},{2:F2}", vector.X, vector.Y, vector.Z);
        }
    }
}
