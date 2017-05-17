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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Config
{
    /// <summary>
    /// configure for kotori engine.
    /// </summary>
    class ConfigData
    {
        /// <summary>
        /// config file path
        /// </summary>
        public const string FILEPATH = "config/config.json";

        /// <summary>
        /// tcp port which listen for kotori
        /// </summary>
        public int listenPort = 8080;
        /// <summary>
        /// thread number to use web working
        /// </summary>
        public int webThread = 1;
        /// <summary>
        /// thread number to use background job
        /// </summary>
        public int bgWorkerThread = 1;
        /// <summary>
        /// database configure
        /// </summary>
        public DataBaseConfig database = null;
    }

    /// <summary>
    /// database configure for kotori
    /// </summary>
    class DataBaseConfig{
        /// <summary>
        /// database type( now support only mysql)
        /// </summary>
        public string type = "mysql";
        /// <summary>
        /// connection setting
        /// </summary>
        public DataBaseConnectionConfig[] connections = null;
    }

    /// <summary>
    /// database connection setting
    /// </summary>
    class DataBaseConnectionConfig
    {
        /// <summary>
        /// Access name for appliction
        /// </summary>
        public string accessName = "";
        /// <summary>
        /// connection pool num 
        /// </summary>
        public int poolNum = 1;

        /// <summary>
        /// mysql host
        /// </summary>
        public string host = "";
        /// <summary>
        /// mysql user
        /// </summary>
        public string user = "";
        /// <summary>
        /// mysql password
        /// </summary>
        public string password = "";
        /// <summary>
        /// mysql database select
        /// </summary>
        public string database = "";
        /// <summary>
        ///  mysql port number
        /// </summary>
        public int port = 3306;
    }
}
