using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Kotori.Module;
using System.Net;
using Kotori.Http;

namespace Kotori.Task
{
    public class HttpMainTask
    {
        private Thread thread;

        private TcpClient client;
        private WebModuleResolver resolver;
        private bool inExec = false;
        private bool isEnable = true;
        private Action<HttpMainTask> onEndConnection;

        public HttpMainTask(Action<HttpMainTask> onEnd){
            this.thread = new Thread(Run);
            this.thread.Start();
            this.onEndConnection = onEnd;
        }

        public void SetExec( TcpClient cl,WebModuleResolver resolv ){
            this.client = cl;
            this.resolver = resolv;
            inExec = true;
            this.thread.Interrupt();
        }

        public void Run()
        {
            while (isEnable)
            {
                try
                {
                    Thread.Sleep(Timeout.Infinite);
                }catch(ThreadInterruptedException){

                }
                if (inExec)
                {
                    Exec(this.client, this.resolver);
                    this.onEndConnection(this);
                    this.inExec = false;
                }
            }
        }

        /// <summary>
        /// exec httpSession
        /// </summary>
        /// <param name="client"></param>
        /// <param name="resolver"></param>
        private static void Exec(TcpClient client, WebModuleResolver resolver)
        {
            using (NetworkStream stream = client.GetStream())
            {
                HttpRequest request = new HttpRequest(stream);
                HttpResponse response = new HttpResponse();

                IWebModule module = resolver.CreateWebModule(request.path);
                System.Console.WriteLine("access " + request.path);
                if (module != null)
                {
                    module.Exec(request, response);
                }
                else
                {
                    response.status = HttpResponse.HttpStatus.NotFound;
                }
                if (!string.IsNullOrEmpty(request.method))
                {
                    response.SendData(stream);
                }
                stream.Close();
            }
        }
    }
}
