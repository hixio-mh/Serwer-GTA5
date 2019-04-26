using System;
using RAGE;
using Player = RAGE.Elements.Player;
using Manager;

namespace Main
{
    static class Globals
    {
        static public CManagers managers;
        static public Player localPlayer = Player.LocalPlayer;
    }
    public class CMain : Events.Script
    {
        public CMain()
        {
            Globals.managers = new CManagers();

            Chat.Output("Załadowano pliki klienta ok");
        }
    }
}
