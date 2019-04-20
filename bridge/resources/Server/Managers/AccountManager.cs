using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using DevOne.Security.Cryptography.BCrypt;

namespace Managers
{
    public class CAccountManager
    {
        public CAccountManager()
        {
            Console.WriteLine("AccountManager init {0}",BCryptHelper.HashPassword("siema",BCryptHelper.GenerateSalt()));
        }
    }
}
