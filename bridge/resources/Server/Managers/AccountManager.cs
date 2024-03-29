﻿using System;
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
using Extend;

namespace Managers
{
    public class CAccountManager : Manager
    {
        List<long> listUsedAccounts = new List<long>();

        public CAccountManager()
        {
            UpdateLevels();
        }

        public void UpdateLevels()
        {
            //levelsRows.Clear();
            //Globals.Mysql.GetTableRows(ref levelsRows);
        }

        public int GetLevelFromXP(uint xp)
        {
            /*foreach (CLevelRow levelRow in levelsRows)
            {
                if (xp < levelRow.xp) return (levelRow.level - 1);
            }*/

            return 1;
        }

        public uint GetLastPid()
        {
            return 0;//Convert.ToUInt32(Globals.Mysql.GetValue("select max(pid) from accounts limit 1"));
        }
        public void setAccountUsed(long id, bool used)
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
            bool loginOccupied = false;// Globals.Mysql.select.PlayerByLogin(login).isResult;
            if (loginOccupied)
                return true;

            bool emailOccupied = false;// Globals.Mysql.select.PlayerByEmail(email).isResult;
            if (emailOccupied)
                return true;

            return false;
        }
        public bool AccountExists(string login)
        {
            bool loginOccupied = false;// Globals.Mysql.select.PlayerByLogin(login).isResult;
            if (loginOccupied)
                return true;

            return false;
        }

        /*public CAccount Register(string login, string pass, string email)
        {
            if(AccountExists(login,pass))
            {
                return null;
            }
            string bcryptedPass = BCryptHelper.HashPassword(pass, BCryptHelper.GenerateSalt());
            Globals.Mysql.UpdateBlocking("insert into accounts (login,pass,email)values(@p1,@p2,@p3)", login, bcryptedPass, email);

            return new CAccount(GetLastPid());
        }*/

        public uint CheckCredentials(string loginOrEmail, string pass)
        {
            /*CAccountsRow resultLogin = Globals.Mysql.select.PlayerByLogin(loginOrEmail);
            if(resultLogin.isResult)
            {
                if(BCryptHelper.CheckPassword(pass, resultLogin.pass))
                {
                    return resultLogin.pid;
                }
            }*/
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
        /*public CAccount LogIn(Client player, uint pid)
        {
            if(isAccountInUse(pid))
                return null;

            CAccount account = new CAccount(pid);
            account.SetPlayer(player);
            player.SetAccount(account);
            return account;
        }

        public CAccount LogIn(Client player, CAccount account)
        {
            account.SetPlayer(player);
            player.SetAccount(account);
            return account;
        }*/
    }
}
