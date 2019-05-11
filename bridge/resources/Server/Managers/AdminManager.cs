using System;
using System.Collections.Generic;
using System.Text;
using Database;
using Main;
using GTANetworkAPI;
using Extend;

namespace Managers
{
    public class CAdminManager
    {
        List<CAdminRow> admins = new List<CAdminRow>();
        public List<CAdminRow> Admins { get => admins; private set => admins = value; }

        List<CAdminRankRow> AdminsRanks = new List<CAdminRankRow>();
        List<CAdminCommandRow> AdminsCommands = new List<CAdminCommandRow>();
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
            Admins.Clear();
            AdminsRanks.Clear();
            AdminsCommands.Clear();
            Globals.Mysql.GetTableRows(ref admins);
            Globals.Mysql.GetTableRows(ref AdminsRanks);
            Globals.Mysql.GetTableRows(ref AdminsCommands);

            List<Client> OnlineAdminsTemp = NAPI.Data.GetWorldData("admins");
            OnlineAdminsTemp.ForEach(admin => StartDuty(admin));
            OnlineAdminsTemp.Clear();

            NAPI.Data.ResetWorldData("admin");
        }

        public bool HasAccessToCommand(Client player, string command)
        {
            uint? pid = player.UID();
            if (pid == null) return false;

            if (!CanStartDuty(player)) return false;

            byte? playerRank = GetRank(player);
            if (playerRank == null) return false;

            byte? commandLevel = GetCommandLevel(command);
            if (commandLevel == null) return false;

            byte? playerLevel = GetRankLevel((byte)playerRank);
            if (playerLevel == null) return false;

            if (commandLevel > playerLevel) return false;
            return true;
        }

        public byte? GetCommandLevel(string command)
        {
            CAdminCommandRow commandRow = AdminsCommands.Find(cmd => cmd.command == command);
            if (commandRow == null) return null;

            return commandRow.level;
        }
        
        public byte? GetRank(Client player)
        {
            uint? pid = player.UID();
            if (pid == null) return null;

            if (!CanStartDuty(player)) return null;

            return Admins.Find(admin => admin.pid == pid).rank;
        }
        
        public byte? GetRankLevel(byte rank)
        {
            CAdminRankRow adminRankRow = AdminsRanks.Find(r => r.rank == rank);
            if (adminRankRow == null) return null;
            return adminRankRow.level;
        }
        
        public bool CanStartDuty(Client player)
        {
            uint? pid = player.UID();
            if (pid == null) return false;

            return Admins.Exists(admin => admin.pid == pid);
        }

        public bool StartDuty(Client player)
        {
            if (IsOnDuty(player)) return false;

            if (!CanStartDuty(player)) return false;

            uint? pid = player.UID();
            if (pid == null) return false;

            Console.WriteLine("start duty", player.Name);
            OnlineAdmins.Add(player);

            return true;
        }

        public bool StopDuty(Client player)
        {
            if (!IsOnDuty(player)) return false;

            uint? pid = player.UID();
            if (pid == null) return false;

            Console.WriteLine("stop duty", player.Name);
            OnlineAdmins.Remove(player);
            return true;
        }

        public bool IsOnDuty(Client player)
        {
            uint? pid = player.UID();
            if (pid == null) return false;

            return OnlineAdmins.Contains(player);
        }
    }
}
