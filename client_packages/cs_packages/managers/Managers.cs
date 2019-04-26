using System;
using System.Collections.Generic;
using System.Text;
using Manager;
namespace Manager
{
    public class CManagers
    {
        public CRPCManager rpc;
        public CManagers()
        {
            rpc = new CRPCManager();
        }
    }
}
