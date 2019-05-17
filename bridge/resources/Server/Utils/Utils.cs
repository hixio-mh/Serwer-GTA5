using System;
using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Extend;
using Model.Logs;
using System.Configuration.Assemblies;

namespace Utils
{
    public class CUtils
    {
        private static readonly long startTickCount = DateTime.Now.Ticks;
        public static Random Random = new Random();

        public CUtils()
        {
        }

        public static long GetTickCount(bool precious = false)
        {
            if(precious)
                return (DateTime.Now.Ticks - startTickCount);
            else
                return (DateTime.Now.Ticks - startTickCount) / 10000;
        }

        public static Client GetRandomClient()
        {
            List<Client> players = NAPI.Pools.GetAllPlayers();
            if(players.Count > 0)
            {
                return players.GetRandom();
            }
            return null;
        }
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }

        public static object TryConvertTo<T>(object input)
        {
            object result = null;
            try
            {
                result = Convert.ChangeType(input, typeof(T));
            }
            catch
            {
            }

            return result;
        }

        public static bool TransferNumber(ref int src, ref int dest, int transfer, int destLimit)
        {
            if (transfer <= 0) return false;
            int originalSrc = src;
            int originalDest = dest;
            transfer = Math.Min(src, transfer);
            dest += transfer;
            src -= transfer;
            if(dest > destLimit)
            {
                src += (dest - destLimit);
                dest = destLimit;
            }
            
            return true;
            // z 'a' przenieś do 'b' 'c' itemów gdzie 'd' to limit 'b'
            // true jeśli chociaż 1 liczbe przeniosło
            
        }
    }

    public static class ReflectionHelper
    {
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute(Type classType, Type attributeType)
        {
            return classType.GetMethods().Where(methodInfo => methodInfo.GetCustomAttributes(attributeType, true).Length > 0);
        }

        public static IEnumerable<Type> GetAllSubclassOf(Type parent)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type t in a.GetTypes())
                    if (t.IsSubclassOf(parent)) yield return t;
        }
    }
}
