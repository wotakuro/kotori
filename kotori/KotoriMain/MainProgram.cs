using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Kotori.Json;

namespace Kotori
{
    class MainProgram
    {
        public static void Main(string[] args)
        {

            // read test data
            string configText = System.IO.File.ReadAllText(Config.ConfigData.FILEPATH, System.Text.Encoding.UTF8);
            // read json config
            Config.ConfigData configData = JsonParser.ParseJsonToObject<Config.ConfigData>(configText);

            // todo load from json config
            var asm = Assembly.LoadFrom(@"dll\ApplicationSample.dll");
            Kotori.KotoriCore.MainExecute(configData,asm);
        }

    }
}