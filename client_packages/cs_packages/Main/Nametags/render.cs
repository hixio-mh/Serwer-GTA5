using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using RAGE;
using RAGE.Elements;
using CEF = RAGE.Ui.HtmlWindow;

namespace Main.Nametag
{
    class CRender : Events.Script
    {
        public int tick = 0;
        public static int ScreenX = 0;
        public static int ScreenY = 0;

        public static Vector3 Position = new Vector3(-425.5f, 1123.3f, 325.9f);
        public CRender()
        {
            Nametags.Enabled = false;
            RAGE.Game.Graphics.GetActiveScreenResolution(ref ScreenX, ref ScreenY);

            CEF CurrentWindow = new CEF("https://www.youtube.com/");
            CurrentWindow.Active = false;
            RAGE.Ui.Cursor.Visible = false;
            Events.Tick += OnUpdate;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
        }

        private void OnUpdate(List<Events.TickNametagData> nametags)
        {
            tick++;

            float scX = 0;
            float scY = 0;

            RAGE.Game.Ui.GetScreenCoordFromWorldCoord(Position.X, Position.Y, Position.Z, ref scX, ref scY);
            RAGE.NUI.UIResText.Draw("Testing label", (int)(scX * ScreenX), (int)(scY * ScreenY), RAGE.Game.Font.ChaletComprimeCologne, 0.3f, Color.White, RAGE.NUI.UIResText.Alignment.Centered, true, true, 500);

            if (tick > 60)
                HeartBeat();
        }
        public void HeartBeat()
        {
            tick = 0;
        }
    }
}
