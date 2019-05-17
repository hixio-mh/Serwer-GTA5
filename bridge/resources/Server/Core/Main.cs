using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Database;
using Managers;
using Utils;
using Vehicle = GTANetworkAPI.Vehicle;
using Data.Vehicle;
using Logic.Inventory;
using Extend;
using Systems;
using Newtonsoft.Json.Linq;
using Mongo;
using Model.Logs;
using Interfaces;
using Models;

namespace Main
{
    //using MySql.Data.MySqlClient;
    public static class Globals
    {
        static public CMain Main;
        //static public CMysql Mysql;
        static public CManagers Managers;
        static public CUtils Utils;
        static public CSystems Systems;
        static public Config Config;
        static public CMongoDB Mongo;
    }

    public class CMain : Script
    {
        private CMain()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            Globals.Main = this;
            Globals.Utils = new CUtils();
            //Globals.Mysql = CMysql.initialize();
            CMongoDB.Initialize();

            Globals.Managers = new CManagers();
            VehicleData.InitiliazeDefault();
            Globals.Systems = new CSystems();
            Globals.Config = new Config();
            Globals.Mongo.OnMongoReady();
            NAPI.Server.SetCommandErrorMessage("Komenda nie istnieje");

            //Console.Clear();
            Console.WriteLine("Serwer został uruchomiony");
            Test();
        }

        public void Test()
        {

            //IAccount account = new Account();
            //new LogsMoney { AccountId = 5 }.Log();

            /*
            CInventory inv1 = new CInventory(3,2);

            CInventory inv2 = new CInventory(2,2);
            CItem a = inv1.GiveItem(1);
            inv1.MoveItem(a, inv2, 0, 1);
            CItem b = inv1.GiveItem(1);
            CItem c = inv1.GiveItem(1);
            ((CItemTest)c).A = 69;

            CItemIdCard idCard = (CItemIdCard)inv1.GiveItem(3);
            idCard.owner = "asper";

            CItemIdCard innaKarta = (CItemIdCard)inv1.GiveItem(3);
            innaKarta.cardId = 123456;
            inv1.SyncItem(innaKarta, null);
            //Console.WriteLine(inv1.ToJSON());
            inv1.TakeItem(b);*/



            /*int a = 5;
            int b = 10;
            int c = 50;
            int d = 20;
            CUtils.TransferNumber(ref a, ref b, c, d);
            Console.WriteLine("ab {0} {1}",a, b);*/

            //JObject conf = (JObject)Globals.Config["test4"];
            //Console.WriteLine("Config {0} {1}", Globals.Config["test5"], conf["A"]);
            //CSpawnRow spawn1 = Globals.Managers.spawn.GetNearest(new Vector3(20, 0, 0));
            //CSpawnRow spawn2 = Globals.Managers.spawn.GetNearest(new Vector3(200, 0, 0));
            //Console.WriteLine("Najbliższy: {0} {1}", spawn1.Serialize(), spawn2.Serialize());

            //Vehicle vehA = Globals.Managers.vehicle.Create(EVehicleType.UNKNOWN, VehicleHash.Adder, new Vector3(-414.72, 1127.23, 325.90), new Vector3(0,0,0));
            //Vehicle vehB = Globals.Managers.vehicle.Create(EVehicleType.SALON, VehicleHash.Adder, new Vector3(-404.72, 1127.23, 325.90), new Vector3(0,0,0));
            //Console.WriteLine("vehicle type {0} {1}", vehA.IsType(EVehicleType.SALON), vehB.IsType(EVehicleType.SALON));

            /*using (MySqlDataReader reader = Globals.Mysql.RawGet("select pid,login,pass,email,money,xp from accounts where pid = 5"))
            {
                Database.CAccountsRow row = new Database.CAccountsRow();
                Globals.Mysql.select.ReadRow(reader, ref row);
                Console.WriteLine("{0}",JsonConvert.SerializeObject(row));
            }*/
        }

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            /*CPlayersResult res1 = Globals.Mysql.select.PlayerByUID(3);
            Console.WriteLine("Gracz po uid, login: {0}", res1.login);
            CPlayersResult res2 = Globals.Mysql.select.PlayerByLogin("zlodziejdbn");
            Console.WriteLine("Konto po loginie1, pid: {0}", res2.pid);
            CPlayersResult res3 = Globals.Mysql.select.PlayerByLogin("zlOdziEJdbN");
            Console.WriteLine("Konto po loginie2, pid: {0}", res3.pid);
            CPlayersResult res4 = Globals.Mysql.select.PlayerByEmail("inny.EMAIL@wp.pl");
            Console.WriteLine("Konto po emailu1, pid: {0}", res4.pid);
            CPlayersResult res5 = Globals.Mysql.select.PlayerByEmail("inny.email@wp.pl");
            Console.WriteLine("Konto po emailu2, pid: {0}", res5.pid);*/

        }

        [ServerEvent(Event.Update)]
        public void OnUpdate()
        {
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(Client player, Vehicle vehicle, sbyte seatID)
        {
            Globals.Managers.vehicle.OnPlayerEnterVehicle(player, vehicle, seatID);
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Client player, Client killer, uint reason)
        {
            //Globals.Managers.spawn.SpawnPlayer(player);
        }

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Client player)
        {
            NAPI.Entity.SetEntityTransparency(player, 0);
            //NAPI.Entity.SetEntityInvincible(player, true);
            NAPI.Entity.SetEntityPosition(player, new Vector3(9999, 9999, 9999));
            // NAPI.Entity.SetEntityPositionFrozen(player, true);
            //player.FreezePosition = true;
            //Globals.Managers.spawn.SpawnPlayer(player, true);
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client player, DisconnectionType type, string reason)
        {
            player.CleanUp();
            player.Save();
        }
    }
}