#region LICENSE
/**
The MIT License (MIT)

Copyright (c) 2016 Yusuke Kurokawa

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion LICENSE

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
