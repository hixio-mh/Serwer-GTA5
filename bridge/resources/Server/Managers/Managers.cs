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
        public CVehicleManager vehicle;
        public CSpawnManager spawn;
        public CManagers()
        {
            account = new CAccountManager();
            vehicle = new CVehicleManager();
            spawn = new CSpawnManager();
        }
    }
}
