using System;
using RAGE;
using Player = RAGE.Elements.Player;
using Manager;
using Systems;
using Utils;

namespace Main
{
    static class Globals
    {
        static public CManagers Managers;
        static public CSystems Systems;
        static public Player localPlayer = Player.LocalPlayer;
        static public CUtils Utils;
    }
    public class CMain : Events.Script
    {
        public CMain()
        {
            Globals.Managers = new CManagers();
            Globals.Systems = new CSystems();
            Globals.Utils = new CUtils();

            Chat.Output("Załadowano pliki klienta ok");
        }
    }
}
