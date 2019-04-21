using System;
using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Main;
using Managers;
using Database;
using Logger;

namespace Logic.Account
{
    public class CAccount
    {
        public readonly uint pid;
        public readonly string login;
        public readonly string email;
        public long money;
        public uint xp;

        public Client player;

        public enum ESave
        {
            ALL,
            MONEY,
        }
        public void SetPlayer(Client player)
        {
            this.player = player;
        }

        public CAccount(uint pid)
        {
            CPlayersResult result = Globals.Mysql.select.PlayerByUID(pid);

            this.pid = result.pid;
            login = result.login;
            email = result.email;
            money = result.money;
            xp = result.xp;

            Globals.Managers.account.setAccountUsed(pid, true);
        }
        ~CAccount()
        {
            Globals.Managers.account.setAccountUsed(pid, false);
        }

        public void GiveMoney(uint amount, string description)
        {
            CLogger.LogMoney(pid, money, money + amount, description);
            money += amount;
            Save(ESave.MONEY);
        }
        public void TakeMoney(uint amount, string description)
        {
            CLogger.LogMoney(pid, money, money - amount, description);
            money -= amount;
            Save(ESave.MONEY);
        }
        public void SetMoney(uint amount, string description)
        {
            CLogger.LogMoney(pid, money, amount, description);
            money = amount;
            Save(ESave.MONEY);
        }

        public bool Save(ESave save = ESave.ALL)
        {
            switch(save)
            {
                case ESave.ALL:
                    //Globals.gMysql.Update(""); @Todo

                    return true;
                case ESave.MONEY:
                    Globals.Mysql.Update("update accounts set money = @p1 where pid = @p2 limit 1", money, pid);
                    return true;
            }
            return false;
        }
    }
}
