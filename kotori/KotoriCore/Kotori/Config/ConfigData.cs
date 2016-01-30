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
        public string accessName = "";
        public string host = "";
        public string user = "";
        public string password = "";
        public string database = "";
        public int port = 3306;
    }
}
