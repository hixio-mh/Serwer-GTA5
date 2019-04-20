using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using ExtendEntity;

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
}