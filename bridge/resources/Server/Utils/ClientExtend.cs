using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Logic.Account;

namespace Extend.Client
{
    public static class CClientExtension
    {
        private static CAccount account;
        public static bool SetAccount(this GTANetworkAPI.Client player, CAccount playerAccount)
        {
            if(account == null)
            {
                account = playerAccount;
                return true;
            }
            return false;
        }

        public static bool IsLoggedIn(this GTANetworkAPI.Client player)
        {
            return account != null;
        }
        public static CAccount Account(this GTANetworkAPI.Client player)
        {
            return account;
        }

        public static void SendChatMessage(this GTANetworkAPI.Client player, string message, params object[] format)
        {
            player.SendChatMessage(string.Format(message, format));
        }

    }
}
