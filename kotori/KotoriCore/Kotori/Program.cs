using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Kotori.Http;
using Kotori.Module;
using Kotori.Json;

namespace Kotori
{
    class Program
    {
        static void Main(string[] args)
        {
            // read test data
            string testData = System.IO.File.ReadAllText("test.txt", System.Text.Encoding.UTF8);
            Console.WriteLine("--------------------");
            Console.WriteLine(testData);
            Console.WriteLine("--------------------");
            // json test code
            Application.Json.Class1 jsonTestObj = JsonParser.ParseJsonToObject<Application.Json.Class1>(testData);

            // sql test code
            var sqlTest = new Kotori.Mysql.MysqlConnectWrapper("localhost", "kotori", "chunchun", "kotori");
            sqlTest.ReflectionTest();
            sqlTest.Dispose();

            // http
            Int32 port = 80;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            // test
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            WebModuleResolver resolver = new WebModuleResolver(assembly.GetTypes());

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                // Get a stream object for reading and writing
                Exec(client, resolver);
                // Shutdown and end connection
                client.Close();
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

