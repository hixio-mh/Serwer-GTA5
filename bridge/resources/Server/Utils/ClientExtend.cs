using System;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Models;
using Main;
using Managers;

namespace Extend
{
    public static class CClientExtension
    {
        public static void SetAccount(this GTANetworkAPI.Client player, Account playerAccount)
        {
            player.SetData("extension", playerAccount);
        }

        public static Account GetExtension(this GTANetworkAPI.Client player)
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
            return player.Account().LastPosition;
        }

        public static bool Save(this GTANetworkAPI.Client player)
        {
            if(player.Account() == null)
            {
                return false;
            }
            return player.Account().Save();
        }
        public static Account Account(this GTANetworkAPI.Client player)
        {
            return player.GetExtension();
        }

        public static void SendChatMessage(this GTANetworkAPI.Client player, string message, params object[] format)
        {
            player.SendChatMessage(string.Format(message, format));
        }
        
        public static void CleanUp(this GTANetworkAPI.Client player)
        {
            //Globals.Systems.publicVehicles.ClearPlayerPublicVehicle(player);
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
        
        public static bool GiveItem(this GTANetworkAPI.Client player, CItem item, byte? x = null, byte? y = null)
        {
            if (player.Account() == null) return false;

            return player.Account().Inventory.GiveItem(item, x, y);
        }

        public static CItem GiveItem(this GTANetworkAPI.Client player, uint item, byte? x = null, byte? y = null)
        {
            if (player.Account() == null) return null;

            return player.Account().Inventory.GiveItem(item, x, y);
        }

        public static int GetFreeSlots(this GTANetworkAPI.Client player)
        {
            if (player.Account() == null) return 0;

            return player.Account().Inventory.FreeSlots;
        }
        
        public static CItem GetItemBySlot(this GTANetworkAPI.Client player, byte x, byte y)
        {
            if (player.Account() == null) return null;

            return player.Account().Inventory.GetItemBySlot(x,y);
        }

    }
}
