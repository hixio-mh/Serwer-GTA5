using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Database;
using Managers;
using Utils;

namespace Main
{
    public static class Globals
    {
        static public CMain Main;
        static public CMysql Mysql;
        static public CManagers Managers;
        static public CUtils Utils;
    }

    public class CMain : Script
    {
        private CMain()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            Globals.Main = this;
            Globals.Mysql = CMysql.initialize();
            Globals.Managers = new CManagers();
            Globals.Utils = new CUtils();
        }

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            CPlayersResult res1 = Globals.Mysql.select.PlayerByUID(3);
            Console.WriteLine("Gracz po uid, login: {0}", res1.login);
            CPlayersResult res2 = Globals.Mysql.select.PlayerByLogin("zlodziejdbn");
            Console.WriteLine("Konto po loginie1, pid: {0}", res2.pid);
            CPlayersResult res3 = Globals.Mysql.select.PlayerByLogin("zlOdziEJdbN");
            Console.WriteLine("Konto po loginie2, pid: {0}", res3.pid);
            CPlayersResult res4 = Globals.Mysql.select.PlayerByEmail("inny.EMAIL@wp.pl");
            Console.WriteLine("Konto po emailu1, pid: {0}", res4.pid);
            CPlayersResult res5 = Globals.Mysql.select.PlayerByEmail("inny.email@wp.pl");
            Console.WriteLine("Konto po emailu2, pid: {0}", res5.pid);

        }

        [ServerEvent(Event.Update)]
        public void OnUpdate()
        {
        }
    }
}