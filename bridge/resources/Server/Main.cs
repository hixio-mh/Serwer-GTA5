using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Database;
using Mysql = Database.CMysql;
using Managers;

namespace Main
{
    public class CMain : Script
    {
        static public CMain pMain;

        public CMysql pMysql { get; }
        public CManagers pManagers { get; }

        private CMain()
        {
            pManagers = new CManagers();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            pMain = this;
            pMysql = Mysql.initialize();
        }
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            CPlayersResult res1 = pMysql.select.PlayerByUID(3);
            Console.WriteLine("Gracz po uid, login: {0}", res1.login);
            CPlayersResult res2 = pMysql.select.PlayerByLogin("zlodziejdbn");
            Console.WriteLine("Konto po loginie1, pid: {0}", res2.pid);
            CPlayersResult res3 = pMysql.select.PlayerByLogin("zlOdziEJdbN");
            Console.WriteLine("Konto po loginie2, pid: {0}", res3.pid);
            CPlayersResult res4 = pMysql.select.PlayerByEmail("inny.EMAIL@wp.pl");
            Console.WriteLine("Konto po emailu1, pid: {0}", res4.pid);
            CPlayersResult res5 = pMysql.select.PlayerByEmail("inny.email@wp.pl");
            Console.WriteLine("Konto po emailu2, pid: {0}", res5.pid);

        }
    }
}