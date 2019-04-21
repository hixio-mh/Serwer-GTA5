using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using DevOne.Security.Cryptography.BCrypt;
using Database;
using Main;
using Logic.Account;
using Extend.Client;

namespace Managers
{
    public class CAccountManager
    {
        List<uint> listUsedAccounts = new List<uint>();
        public CAccountManager()
        {
            /*string haslo1 = "$2a$10$3lDxW2vW.KK2GtJAr33WxeLJc19cBmSk1mJPY/HBO8WhLE4/USMAa";
            string haslo2 = BCryptHelper.HashPassword("siema", BCryptHelper.GenerateSalt());
            bool poprawne1 = BCryptHelper.CheckPassword("siema", haslo1);
            bool poprawne2 = BCryptHelper.CheckPassword("siema", haslo2);
            bool blabla = haslo1 == haslo2;
            Console.WriteLine("blabla {0} poprawne1 {1} poprawne2 {2}", blabla.ToString(), poprawne1.ToString(), poprawne2.ToString());*/
        }

        public uint GetLastPid()
        {
            return Convert.ToUInt32(Globals.Mysql.GetValue("select max(pid) from accounts limit 1"));
        }
        public void setAccountUsed(uint id, bool used)
        {
            if(used)
            {
                listUsedAccounts.Remove(id);
                listUsedAccounts.Add(id);
            }
            else
            {
                listUsedAccounts.Remove(id);
            }
        }

        public bool isAccountInUse(uint id)
        {
            return listUsedAccounts.Contains(id);
        }

        public bool AccountExists(string login, string email)
        {
            bool loginOccupied = Globals.Mysql.select.PlayerByLogin(login).isResult;
            if (loginOccupied)
                return true;

            bool emailOccupied = Globals.Mysql.select.PlayerByEmail(email).isResult;
            if (emailOccupied)
                return true;

            return false;
        }
        public bool AccountExists(string login)
        {
            bool loginOccupied = Globals.Mysql.select.PlayerByLogin(login).isResult;
            if (loginOccupied)
                return true;

            return false;
        }

        public CAccount Register(string login, string pass, string email)
        {
            if(AccountExists(login,pass))
            {
                return null;
            }
            string bcryptedPass = BCryptHelper.HashPassword(pass, BCryptHelper.GenerateSalt());
            Globals.Mysql.UpdateBlocking("insert into accounts (login,pass,email)values(@p1,@p2,@p3)", login, bcryptedPass, email);

            return new CAccount(GetLastPid());
        }

        public uint CheckCredentials(string loginOrEmail, string pass)
        {
            CPlayersResult resultLogin = Globals.Mysql.select.PlayerByLogin(loginOrEmail);
            if(resultLogin.isResult)
            {
                if(BCryptHelper.CheckPassword(pass, resultLogin.pass))
                {
                    return resultLogin.pid;
                }
            }
            /*
            CPlayersResult resultEmail = Globals.gMysql.select.PlayerByEmail(loginOrEmail);
            if (resultEmail.isResult)
            {
                if (BCryptHelper.CheckPassword(pass, resultEmail.pass))
                {
                    return resultEmail.pid;
                }
            }*/
            return 0;
        }
        public CAccount LogIn(Client player, uint pid)
        {
            if(isAccountInUse(pid))
                return null;

            CAccount account = new CAccount(pid);
            account.SetPlayer(player);
            player.SetAccount(account);
            return new CAccount(pid);
        }

        public CAccount LogIn(Client player, CAccount account)
        {
            account.SetPlayer(player);
            player.SetAccount(account);
            return account;
        }
    }
}
