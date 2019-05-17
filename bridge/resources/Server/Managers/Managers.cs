using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Utils;
using Main;

namespace Managers
{
    public class Manager
    {

    }

    public class CManagers
    {
        public CAccountManager account;
        public CVehicleManager vehicle;
        public CSpawnManager spawn;
        public CRPCManager rpc;
        public CHTTPManager http;
        public CMapManager map;
        public CAdminManager admin;
        public CItemManager item;
        public CPerformanceManager performance;
        public CManagers()
        {
            Console.WriteLine("amangers {0}", CUtils.GetTickCount());
            performance = new CPerformanceManager();
            account = new CAccountManager();
            vehicle = new CVehicleManager();
            spawn = new CSpawnManager();
            rpc = new CRPCManager();
            http = new CHTTPManager();
            //map = new CMapManager();
            admin = new CAdminManager();
            item = new CItemManager();
        }
    }
}
