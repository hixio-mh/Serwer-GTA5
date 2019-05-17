using System;
using System.Collections.Generic;
using System.Text;
using Database;
using Main;
using GTANetworkAPI;
using Extend;

namespace Managers
{
    public class CAdminManager : Manager
    {
        List<Client> OnlineAdmins = new List<Client>();


        public CAdminManager()
        {
            UpdateAdmins();
        }

        public void ClearAdmins()
        {
            OnlineAdmins.ForEach(admin => StopDuty(admin));
        }

        public void UpdateAdmins()
        {
            NAPI.Data.SetWorldData("admins", OnlineAdmins);
            ClearAdmins();
            List<Client> OnlineAdminsTemp = NAPI.Data.GetWorldData("admins");
            OnlineAdminsTemp.ForEach(admin => StartDuty(admin));
            OnlineAdminsTemp.Clear();

            NAPI.Data.ResetWorldData("admin");
        }

        public bool HasAccessTo(Client player, string permission)
        {
            long? pid = player.UID();
            if (!pid.HasValue) return false;

            if (!CanStartDuty(player)) return false;

            return true;
        }

        
        public bool CanStartDuty(Client player)
        {
            long? pid = player.UID();
            if (!pid.HasValue) return false;

            return false;// Admins.Exists(admin => admin.pid == pid);
        }

        public bool StartDuty(Client player)
        {
            if (IsOnDuty(player)) return false;

            if (!CanStartDuty(player)) return false;

            long? pid = player.UID();
            if (!pid.HasValue) return false;

            Console.WriteLine("start duty", player.Name);
            OnlineAdmins.Add(player);

            return true;
        }

        public bool StopDuty(Client player)
        {
            if (!IsOnDuty(player)) return false;

            long? pid = player.UID();
            if (!pid.HasValue) return false;

            Console.WriteLine("stop duty", player.Name);
            OnlineAdmins.Remove(player);
            return true;
        }

        public bool IsOnDuty(Client player)
        {
            long? pid = player.UID();
            if (!pid.HasValue) return false;

            return OnlineAdmins.Contains(player);
        }
    }
}
