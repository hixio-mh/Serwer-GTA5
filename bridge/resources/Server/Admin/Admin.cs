using Extend.Client;
using Extend.Entity;
using Extend.String;
using GTANetworkAPI;
using Logic.Account;
using Main;
using System;

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
        player.SendChatMessage(String.Format("rejestrowano i zalogowano {0} {1}", player.IsLoggedIn().ToString(), player.Account().pid));
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
        player.SendChatMessage(String.Format("rejestrowano i zalogowano {0} {1}", player.IsLoggedIn().ToString(), player.Account().pid));
    }

    [Command("testac")]
    public void testac(Client player)
    {
        if(player.IsLoggedIn())
        {
            player.SendChatMessage(String.Format("testac ZALOGOWANY {0}", player.Account().pid));

        }
        else
        {
            player.SendChatMessage("testac NIEZALOGOWANY");

        }
    }

}