using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Net;
using System.Linq;
using Managers;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Main;

namespace Systems
{
    using WebPage = Tuple<string, Func<HttpListenerContext, NameValueCollection, string>>;

    public class CWebAccount
    {
        public string name { get; private set; }

    }
    public class CWebPanel
    {
        private Dictionary<char, CWebAccount> webAccounts = new Dictionary<char, CWebAccount>();
        private readonly Dictionary<string, WebPage> pages;

        private WebPage Page(string link, Func<HttpListenerContext, NameValueCollection, string> func) => new WebPage(link, func);

        public CWebPanel()
        {
            pages = new Dictionary<string, WebPage>
            {
                ["index"] = Page("Strona główna", Index),
                ["players"] = Page("Gracze", Players),
                ["performance"] = Page("Wydajność", Performance),
            };
        }

        public void UpdateAccounts()
        {
            webAccounts.Clear();

        }
        private string GetPagesList()
        {
            List<string> pagesStr = new List<string>(capacity: pages.Count);
            foreach(KeyValuePair <string, WebPage> pair in pages)
            {
                pagesStr.Add(String.Format("<a href='admin?page={0}'>{1}</a>", pair.Key, pair.Value.Item1));
            }
            return string.Concat(pagesStr);
        }

        private string Index(HttpListenerContext context, NameValueCollection query)
        {
            return "index";
        }
        private string Players(HttpListenerContext context, NameValueCollection query)
        {
            return "gracze";
        }
        private string Performance(HttpListenerContext context, NameValueCollection query)
        {
            StringBuilder result = new StringBuilder();
            result.Append("Użycie procesora:<br>");
            CPerformanceManager performance = Globals.Managers.performance;
            performance.sCPU.ToList().ForEach((CPerformanceManager.CCPU cpuUsage) =>
            {
                result.Append(string.Format("{0}: {1}<br>", cpuUsage.Ticks, cpuUsage.Usage));
            });

            result.Append("<hr>Użycie ramu:<br>");
            performance.sRAM.ToList().ForEach((CPerformanceManager.CRAM ramUsage) =>
            {
                result.Append(string.Format("{0}: {1} MB<br>", ramUsage.Ticks, ramUsage.Usage));
            });
            return result.ToString();
        }

        private bool HasPermissions(HttpListenerContext context, NameValueCollection query)
        {
            Cookie cookie = context.Request.Cookies["key"];

            return false;
        }

        public string OnEnter(HttpListenerContext context, NameValueCollection query)
        {
            string page = query.Get("page")??"index";

            WebPage func;
            if (!pages.TryGetValue(page, out func))
                func = pages["index"];

            string pageList = GetPagesList();
            string contentString = func.Item2(context, query);
            return $@"
<!doctype html>
<html lang=""en"">
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=no"">

    <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css"" integrity=""sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm"" crossorigin=""anonymous"">

    <link rel=""stylesheet"" type=""text/css"" href=""https://cdn.datatables.net/1.10.19/css/jquery.dataTables.css"" >

    <script type=""text/javascript"" charset = ""utf8"" src = ""https://cdn.datatables.net/1.10.19/js/jquery.dataTables.js"" ></script>

    <title>WebPanel</title>
  </head>
  <body>
    <style>
      body, html{{
        height:100vh;
      }}

      .container{{
        height:100vh;
      }}

      .container > .row{{
        height:100vh;
      }}

      .leftMenu{{
        background-color: black;
        padding-left:1%;
        height:100%;
      }}

    .leftMenu > a{{
      display: block;
      width:100%;
      color:white;
      font: bold 1em ""Trebuchet MS"", Arial, sans-serif;
      font-size:20px;
      --text-decoration: none;
    }}

    .rightMenu{{
    }}

    </style>
    <div class=""container"">
        <div class=""row"">
            <div class=""col-md-2 first leftMenu"">
              {pageList}
            </div>
            <div class=""col-md second rightMenu"">
              {contentString}
            </div>
        </div>
      </div>
    </div>

    <script src=""https://code.jquery.com/jquery-3.2.1.slim.min.js"" integrity=""sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN"" crossorigin=""anonymous""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js"" integrity=""sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q"" crossorigin=""anonymous""></script>
    <script src=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js"" integrity=""sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl"" crossorigin=""anonymous""></script>
  </body>
</html>
";
        }
    }
}
