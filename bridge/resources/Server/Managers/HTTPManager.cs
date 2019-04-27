using System;
using System.Net;
using System.Text;
using System.Threading;
using Extend;

namespace Managers
{
    public class CHTTPManager
    {
        private string[] listenedAddresses;
        private bool isWorked;
        private HttpListener listener;

        public CHTTPManager()
        {
            listenedAddresses = new string[] { "http://localhost:3000/" };
            isWorked = false;
            RunServer();
        }

        private void HandleRequest(HttpListenerContext context)
        {
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
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    Console.WriteLine("request url {0}, method {1}", request.RawUrl, request.HttpMethod);

                    string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                    response.ContentLength64 = buffer.Length;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    System.IO.Stream output = response.OutputStream;
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