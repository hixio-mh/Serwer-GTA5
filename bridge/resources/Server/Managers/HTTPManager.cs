using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Linq.Expressions;
using Extend;
using System.Collections.Specialized;

namespace Managers
{
    public class CHTTPManager
    {
        private string[] listenedAddresses;
        private bool isWorked;
        private HttpListener listener;
        private object responseObject;

        private Dictionary<string, Func<HttpListenerContext, NameValueCollection, object>> dEndPoints = new Dictionary<string, Func<HttpListenerContext, NameValueCollection, object>>
        {
            ["/test"] = (HttpListenerContext context, NameValueCollection query) =>
            {
                string a = query["a"];
                string b = query["b"];
                return new string[] { "a = ", a, b, "test", "c" };
            },
            ["/test2"] = (HttpListenerContext context, NameValueCollection query) =>
            {
                return new string[] { "a", "test2", "c" };
            },
            ["/test3"] = (HttpListenerContext context, NameValueCollection query) =>
            {
                return context.User;
            },
        };

        public CHTTPManager()
        {
            listenedAddresses = new string[] { "http://localhost:3000/" };
            isWorked = false;
            RunServer();
        }

        private bool HandleRequest(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "GET") return false;

            Func<HttpListenerContext, NameValueCollection, object> endPoint = default;
            string endPointName = context.Request.RawUrl.Split("?")[0];
            if (dEndPoints.TryGetValue(endPointName, out endPoint))
            {
                responseObject = endPoint(context, context.Request.QueryString);
                return true;
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
                        buffer = Encoding.UTF8.GetBytes(responseObject.Serialize());
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes("");
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }

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