using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Extend;

namespace Extend
{
    public static class CEntityExtension 
    {
        public static bool SetValue(this GTANetworkAPI.Entity entity, string strKey, object value)
        {
            entity.SetSharedData(strKey, value);
            return true;
        }
        public static object GetValue(this GTANetworkAPI.Entity entity, string strKey)
        {
            if(entity.HasSharedData(strKey))
            {
                return entity.GetSharedData(strKey);
            }
            return null;
        }

        public static uint? UID(this GTANetworkAPI.Entity entity)
        {
            if(entity.HasSharedData("UID"))
            {
                return System.Convert.ToUInt32(entity.GetSharedData("UID"));
            }
            return null;
        }
        public static bool AssignUID(this GTANetworkAPI.Entity entity, uint uid)
        {
            if(entity.HasSharedData("UID"))
            {
                return false;
            }
            entity.SetSharedData("UID", uid);
            return true;
        }

        public static Vector3 GetPositionAtFront(this GTANetworkAPI.Entity entity, float distance)
        {
            return entity.Position.GetPointFromDistanceRotation(distance, -entity.Rotation.Z);
        }
    }
}
