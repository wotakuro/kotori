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
    class Program
    {
        static void Main(string[] args)
        {
            // read test data
            string configText = System.IO.File.ReadAllText( Config.ConfigData.FILEPATH, System.Text.Encoding.UTF8);
            Console.WriteLine(configText);
            Console.WriteLine("--------------------");
            // read json config
            Config.ConfigData configData = JsonParser.ParseJsonToObject<Config.ConfigData>(configText);
            // connections
            SqlConnectionPool.Instance.Initialize(configData.database.connections);

            Console.WriteLine(configData.database.connections[0].host);

            // sql test code
            var sqlTest = SqlConnectionPool.Instance.AllocConnection("master");
            sqlTest.ReflectionTest();
            SqlConnectionPool.Instance.ReleaseConnection(sqlTest);

            // http
            Int32 port = configData.listenPort;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            // resolveModule
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            WebModuleResolver resolver = new WebModuleResolver(assembly.GetTypes());


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

