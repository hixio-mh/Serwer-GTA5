using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;

namespace Managers
{
    public class CManagers
    {
        public CAccountManager account;
        public CManagers()
        {
            account = new CAccountManager();
        }
    }
}
