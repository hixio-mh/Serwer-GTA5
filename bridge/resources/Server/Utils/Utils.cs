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

namespace Utils
{
    public class CUtils
    {

        private readonly long startTickCount;
        public CUtils()
        {
            startTickCount = DateTime.Now.Ticks;
        }
        public long GetTickCount()
        {
            return (DateTime.Now.Ticks - startTickCount)/10000;
        }

        public static Random Random = new Random();

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }
    }
}
