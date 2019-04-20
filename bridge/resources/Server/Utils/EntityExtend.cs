using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;

namespace ExtendEntity
{
    public static class EntityExtension 
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

    }
}
