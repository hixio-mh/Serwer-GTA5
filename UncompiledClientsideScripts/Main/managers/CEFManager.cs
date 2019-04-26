using System;
using System.Collections.Generic;
using System.Text;
using RAGE;
using CEF = RAGE.Ui.HtmlWindow;

namespace Manager
{
    public class CCEFManager
    {
        CEF browser;
        public CCEFManager()
        {
            browser = new CEF("package://html/index.html");

            browser.Active = true;
        }
    }
}
