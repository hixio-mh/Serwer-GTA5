using Extend.Clients;
using Extend.Entity;
using Extend.Strings;
using GTANetworkAPI;
using Logic.Account;
using Main;
using System;
using Database;
using Newtonsoft.Json;
using Extend.Maths;
using Extend.Clients;

public class CAdmin : Script
{
    CAdmin()
    {
        Console.WriteLine("Admin włączony");
    }

    [Command("restart")]
    public void SayHi(Client player)
    {
        NAPI.Resource.StopResource("Server");
        NAPI.Resource.StartResource("Server");
    }
    [Command("veh")]
    public void veh(Client player, VehicleHash vehicle)
    {
        NAPI.Vehicle.CreateVehicle(vehicle, player.Position.Around(3.0f), 0f, 0, 0);
    }

    [Command("testset")]
    public void testset(Client player, int wartosc)
    {
        player.SetValue("test",wartosc);
        player.SendChatMessage("Ustawiono pomyślnie");
    }

    [Command("testget")]
    public void testget(Client player)
    {
        player.SendChatMessage("twoja wartosc: " + player.GetValue("test").ToString());
    }

    [Command("register")]
    public void register(Client player, string login, string hasło, string email)
    {
        if (player.IsLoggedIn())
        {
            player.SendChatMessage("Jesteś już zalogowany.");
            return;
        }
        if (!login.IsBetween(6, 20))
        {
            player.SendChatMessage("Login powinien mieć od 6 do 20 znaków.");
            return;
        }

        if (!hasło.IsBetween(6, 20))
        {
            player.SendChatMessage("Hasło powinien mieć od 6 do 20 znaków.");
            return;
        }

        if(!email.IsValidEmail())
        {
            player.SendChatMessage("Podany email jest niepoprawny.");
            return;
        }
        if(Globals.Managers.account.AccountExists(login, email))
        {
            player.SendChatMessage("Podany login lub email jest zajęty.");
            return;
        }

        CAccount account = Globals.Managers.account.Register(login, hasło, email);
        Globals.Managers.account.LogIn(player, account);
        player.SendChatMessage("Zarejestrowano i zalogowano pomyślnie, id konta {0}", player.Account().pid);
    }
    [Command("login")]
    public void login(Client player, string login, string hasło)
    {
        if (player.IsLoggedIn())
        {
            player.SendChatMessage("Jesteś już zalogowany.");
            return;
        }
        if (!login.IsBetween(6, 20))
        {
            player.SendChatMessage("Login powinien mieć od 6 do 20 znaków.");
            return;
        }

        if (!hasło.IsBetween(6, 20))
        {
            player.SendChatMessage("Hasło powinien mieć od 6 do 20 znaków.");
            return;
        }

        if(!Globals.Managers.account.AccountExists(login))
        {
            player.SendChatMessage("Konto nie istnieje.");
            return;
        }

        uint pid = Globals.Managers.account.CheckCredentials(login, hasło);
        if(pid == 0)
        {
            player.SendChatMessage("Hasło niepoprawne.");
            return;
        }
        Globals.Managers.account.LogIn(player, pid);
        Globals.Managers.spawn.SpawnPlayer(player, true);
        player.SendChatMessage("Zalogowano pomyślnie, id konta {0}",player.Account().pid);
    }

    [Command("testac")]
    public void testac(Client player)
    {
        if (player.IsLoggedIn())
        {
            player.SendChatMessage(String.Format("testac ZALOGOWANY {0}", player.Account().pid));
        }
        else
        {
            player.SendChatMessage("testac NIEZALOGOWANY");

        }
    }

    [Command("setmoney")]
    public void setmoney(Client player, long amount)
    {
        if (player.IsLoggedIn())
        {
            player.SendChatMessage("Ustaw {0} $", amount);
            player.Account().SetMoney(amount, "komenda /setmoney");
        }
        else
        {
            player.SendChatMessage("musisz być zalogowany");
        }
    }

    [Command("gp")]
    public void gp(Client player)
    {
        Console.WriteLine(player.Position.ToStr());
    }


    [Command("test")]
    public void test(Client player, float distance)
    {
        NAPI.Marker.CreateMarker(1, player.GetPositionAtFront(distance), new Vector3(), new Vector3(), 2, 255, 0,0, false, uint.MaxValue);
    }

    [Command("licgiv")]
    public void licgiv(Client player)
    {
        if (player.IsLoggedIn())
        {
            player.Account().GiveLicense(1);
            player.SendChatMessage("daj licencje 1");
        }
    }
    [Command("liccheck")]
    public void liccheck(Client player)
    {
        if (player.IsLoggedIn())
        {
            bool has = player.Account().HasLicense(1);
            player.SendChatMessage("ma licencje 1? {0}",has);
        }
    }
    [Command("createveh")]
    public void createveh(Client player)
    {
        if (player.IsLoggedIn())
        {
            uint vid = Globals.Managers.vehicle.CreatePrivateVehicle(VehicleHash.Adder, player);
            player.SendChatMessage("nowy pojazd {0}", vid);
        }
    }

    [Command("spawnveh")]
    public void spawnveh(Client player, uint vid)
    {
        Vehicle veh = Globals.Managers.vehicle.SpawnPrivateVehicle(vid, player.GetPositionAtFront(5), new Vector3(0, 0, 0));
        if(veh != null)
        {
            player.SendChatMessage("stworzono pojazd {0} vid {1}", veh, veh.UID());
        }
        else
        {
            player.SendChatMessage("nie udało się stworzyć pojazdu");
        }
    }

    [Command("save")]
    public void save(Client player)
    {
        player.Save();
    }
}