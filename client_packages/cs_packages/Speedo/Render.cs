using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using RAGE;

namespace Main.Speedo
{
    class CRender : Events.Script
    {
        public static int ResX = 0;
        public static int ResY = 0;
        public CRender()
        {
            RAGE.Game.Graphics.GetActiveScreenResolution(ref ResX, ref ResY);
            Events.Tick += OnUpdate;
        }

        private void OnUpdate(List<Events.TickNametagData> nametags)
        {
            if(RAGE.Game.Ped.IsPedInAnyVehicle(RAGE.Game.Player.GetPlayerPed(), false))
            {
                RAGE.Game.UIText.Draw($"ResX: {ResX}", new Point(ResX - 50, 50), 0.8f, Color.Red, RAGE.Game.Font.ChaletComprimeCologne, false);
                RAGE.Game.UIText.Draw($"ResY: {ResY}", new Point(ResX - 50, 90), 0.8f, Color.Red, RAGE.Game.Font.ChaletComprimeCologne, false);
            }
        }
    }
}
