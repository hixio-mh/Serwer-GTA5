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
        public static void SetAccount(this GTANetworkAPI.Client player, CAccount playerAccount)
        {
            player.SetData("extension", playerAccount);
        }

        public static CAccount GetExtension(this GTANetworkAPI.Client player)
        {
            if (!player.HasData("extension"))
            {
                return null;
            }
            return player.GetData("extension");
        }

        public static bool IsLoggedIn(this GTANetworkAPI.Client player)
        {
            return player.Account() != null;
        }
        public static CAccount Account(this GTANetworkAPI.Client player)
        {
            return player.GetExtension();
        }

        public static void SendChatMessage(this GTANetworkAPI.Client player, string message, params object[] format)
        {
            player.SendChatMessage(string.Format(message, format));
        }

    }
}
