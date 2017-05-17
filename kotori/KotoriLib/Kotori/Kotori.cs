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
using System.Net;
using System.Net.Sockets;
using Kotori.Http;
using Kotori.Module;
using Kotori.Json;
using Kotori.Mysql;
using Kotori.Task;

namespace Kotori
{
    public class KotoriCore
    {
        public static void MainExecute(Config.ConfigData configData , System.Reflection.Assembly assembly)
        {
            // connections
            SqlConnectionPool.Instance.Initialize(configData.database.connections);

            // http server open
            Int32 port = configData.listenPort;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            // resolveModule
            Type[] assemblyTypes = assembly.GetTypes();
            WebModuleResolver resolver = new WebModuleResolver(assemblyTypes);

            // Application Initilize routine
            InitializeResolver.Initialize(assemblyTypes);

            /// http task Thread
            HttpMainTaskManager.Instance.Initialize(configData.webThread);

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                var task = HttpMainTaskManager.Instance.GetWaitTask();
                if (task != null)
                {
                    task.SetExec(client, resolver);
                }
                else
                {
                    // Shutdown and end connection
                    client.Close();
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

