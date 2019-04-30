using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using Database;
using GTANetworkAPI;
using Main;

namespace Extend
{
    public static class CList
    {
        public static T GetRandom<T>(this IList<T> obj)
        {
            return obj[CUtils.Random.Next(obj.Count)];
        }

        public static void ClearAndRemoveEntities<T>(this List<T> list) where T: Entity
        {
            foreach(T entity in list)
            {
                if (entity == null) continue;

                if (typeof(T) == typeof(Vehicle))
                {
                    Globals.Managers.vehicle.Destroy((Vehicle)(object)entity, true);
                }
                else
                    entity.Delete();
            }
            list.Clear();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = CUtils.Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}