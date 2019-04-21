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
using Data.Account;

namespace Logic.Account
{
    public class CLicense
    {
        public byte id;
        public DateTime suspended;
        public string reason;
        public bool isSuspended()
        {
            if (suspended > DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
    public class CAccount
    {
        public readonly uint pid;
        public readonly string login;
        public readonly string email;
        public long money;
        public uint xp;

        public Client player;
        private bool licensesUpdatedFromDB = false;
        public List<CLicense> licenses = new List<CLicense>();
        
        public void SetPlayer(Client player)
        {
            this.player = player;
        }

        public void UpdateLicensesFromDB(bool force = false)
        {
            if (force || !licensesUpdatedFromDB)
            {
                licensesUpdatedFromDB = true;
                licenses.Clear();
                List<CAccountLicenseResult> dbResult = Globals.Mysql.select.GetAccountLicenses(pid);
                foreach(CAccountLicenseResult result in dbResult)
                {
                    licenses.Add(new CLicense {
                        id = result.lid,
                        suspended = result.suspended,
                        reason = result.suspendedreason,
                    });
                }
            }
        }

        public CLicense GetLicense(byte lid)
        {
            if(HasLicense(lid))
            {
                CLicense license = licenses.Find(delegate (CLicense i) { return i.id == lid; });
                if (license == null)
                {
                    return null;
                }
                return license;
            }
            return null;
        }
        public bool HasLicense(byte lid, bool checkIfIsSuspended = false)
        {
            UpdateLicensesFromDB();
            CLicense license = licenses.Find(i => i.id == lid);
            Console.WriteLine("HasLicense {0} {1}",lid, license);
            if(license == null)
            {
                return false;
            }
            
            if(checkIfIsSuspended)
            {
                if(license.isSuspended())
                {
                    return false;
                }
            }
            return true;
        }

        public bool GiveLicense(byte lid)
        {
            if(HasLicense(lid))
            {
                return false;
            }
            Globals.Mysql.UpdateBlocking("insert into accounts_licenses (pid,lid)values(@p1,@p2)", pid, lid);
            UpdateLicensesFromDB(true);
            return true;
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

        public void GiveMoney(long amount, string description)
        {
            CLogger.LogMoney(pid, money, money + amount, description);
            money += amount;
            Save(CAccountData.ESave.MONEY);
        }
        public void TakeMoney(long amount, string description)
        {
            CLogger.LogMoney(pid, money, money - amount, description);
            money -= amount;
            Save(CAccountData.ESave.MONEY);
        }
        public void SetMoney(long amount, string description)
        {
            CLogger.LogMoney(pid, money, amount, description);
            money = amount;
            Save(CAccountData.ESave.MONEY);
        }

        public bool Save(CAccountData.ESave save = CAccountData.ESave.ALL)
        {
            switch(save)
            {
                case CAccountData.ESave.ALL:
                    //Globals.gMysql.Update(""); @Todo

                    return true;
                case CAccountData.ESave.MONEY:
                    Globals.Mysql.Update("update accounts set money = @p1 where pid = @p2 limit 1", money, pid);
                    return true;
            }
            return false;
        }
    }
}
