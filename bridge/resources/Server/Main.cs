using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Database;
using Mysql = Database.Mysql;
namespace Main
{
    public class Main : Script
    {
        static public Main pMain;

        public Mysql pMysql { get; }

        private Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            pMain = this;
            pMysql = Mysql.initialize();
        }
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            PlayersResult res1 = pMysql.select.PlayerByUID(3);
            Console.WriteLine("Gracz po uid, login: {0}", res1.login);
            PlayersResult res2 = pMysql.select.PlayerByLogin("zlodziejdbn");
            Console.WriteLine("Konto po loginie1, pid: {0}", res2.pid);
            PlayersResult res3 = pMysql.select.PlayerByLogin("zlOdziEJdbN");
            Console.WriteLine("Konto po loginie2, pid: {0}", res3.pid);
            PlayersResult res4 = pMysql.select.PlayerByEmail("inny.EMAIL@wp.pl");
            Console.WriteLine("Konto po emailu1, pid: {0}", res4.pid);
            PlayersResult res5 = pMysql.select.PlayerByEmail("inny.email@wp.pl");
            Console.WriteLine("Konto po emailu2, pid: {0}", res5.pid);

        }
    }
}