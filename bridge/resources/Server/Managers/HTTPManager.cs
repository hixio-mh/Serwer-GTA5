using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Linq.Expressions;
using GTANetworkAPI;
using Extend;
using Main;
using System.Collections.Specialized;

namespace Managers
{
    public class CHTTPManager : Manager
    {
        private string[] listenedAddresses;
        private bool isWorked;
        private HttpListener listener;
        private object responseObject;
        public bool custom = false;

        private Dictionary<string, Func<HttpListenerContext, NameValueCollection, object>> dEndPoints = new Dictionary<string, Func<HttpListenerContext, NameValueCollection, object>>
        {
            ["/players"] = (HttpListenerContext context, NameValueCollection query) =>
            {
                List<string> players = new List<string>();
                foreach(Client client in NAPI.Pools.GetAllPlayers())
                {
                    players.Add(client.Name);
                }
                return players.ToArray();
            },
            ["/test2"] = (HttpListenerContext context, NameValueCollection query) =>
            {
                return new string[] { "a", "test2", "c" };
            },
            ["/test3"] = (HttpListenerContext context, NameValueCollection query) =>
            {
                return context.User;
            },
            ["/admin"] = (HttpListenerContext context, NameValueCollection query) =>
            {
                Globals.Managers.http.custom = true;
                context.Response.ContentType = "text/html";
                return Globals.Systems.webPanel.OnEnter(context, query);
            },
        };

        public CHTTPManager()
        {
#if true
            listenedAddresses = new string[] { "http://localhost:3000/" };
            isWorked = false;
            RunServer();
#endif
        }

        private bool HandleRequest(HttpListenerContext context, bool custom = false)
        {
            if (context.Request.HttpMethod == "GET")
            {
                Func<HttpListenerContext, NameValueCollection, object> endPoint = default;
                string endPointName = context.Request.RawUrl.Split("?")[0];
                if (dEndPoints.TryGetValue(endPointName, out endPoint))
                {
                    responseObject = endPoint(context, context.Request.QueryString);
                    return true;
                }
            }

            return false;
        }

        private void work()
        {
            listener = new HttpListener();
            foreach (var prefix in listenedAddresses)
                listener.Prefixes.Add(prefix);

            listener.Start();

            while (isWorked)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();

                    byte[] buffer;
                    if (HandleRequest(context))
                    {
                        if(custom)
                            buffer = Encoding.UTF8.GetBytes(responseObject.ToString());
                        else
                            buffer = Encoding.UTF8.GetBytes(responseObject.Serialize());

                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes("");
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }

                    if(!custom)
                        context.Response.ContentType = "application/json";

                    context.Response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = context.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    output.Close();
                }
                catch (Exception)
                {

                }
            }
            stop();
        }

        public void stop()
        {
            isWorked = false;
            listener.Stop();
        }


        public void RunServer()
        {
            if (isWorked)
                throw new Exception("server alredy started");

            isWorked = true;

            Timer t = new Timer((thread) =>
            {
                work();
            });
            t.Change(1, Timeout.Infinite);
            Thread.Sleep(10);
        }

    }
}