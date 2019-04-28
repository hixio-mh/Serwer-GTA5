using System;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Logic.Account;
using Main;
using Managers;

namespace Extend
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

        public static Vector3 GetLastPosition(this GTANetworkAPI.Client player)
        {
            if(player.Account() == null)
            {
                return null;
            }
            return player.Account().GetLastPosition();
        }

        public static bool Save(this GTANetworkAPI.Client player)
        {
            if(player.Account() == null)
            {
                return false;
            }
            return player.Account().Save();
        }
        public static CAccount Account(this GTANetworkAPI.Client player)
        {
            return player.GetExtension();
        }

        public static void SendChatMessage(this GTANetworkAPI.Client player, string message, params object[] format)
        {
            player.SendChatMessage(string.Format(message, format));
        }
        
        public static void CleanUp(this GTANetworkAPI.Client player)
        {
            Globals.Systems.publicVehicles.ClearPlayerPublicVehicle(player);
            Globals.Managers.admin.StopDuty(player);

            if (player.Account() == null) return;

            player.Account().CleanUp();
        }
        static public string RPCs(CRPCManager.ERPCs eRPC)
        {
            return eRPC.ToString();
        }

        public static void TriggerClient(this GTANetworkAPI.Client player, CRPCManager.ERPCs rpc, params object[] args)
        {
            NAPI.ClientEvent.TriggerClientEvent(player, rpc.ToString(), args);
        }
        
        public static bool IsOnDuty(this GTANetworkAPI.Client player)
        {
            return Globals.Managers.admin.IsOnDuty(player);
        }

    }
}
