using System;
using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;

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
    }
}
