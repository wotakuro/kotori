using System;
using System.Text;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

namespace Kotori.Mysql
{
    public class MysqlConnectWrapper:IDisposable
    {

        public string Tag { private set; get; }

        /// <summary>
        /// connection
        /// </summary>
        private MySqlConnection connection;

        /*-------- functions ------------*/
        /// <summary>
        /// mysql Connection 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="host"></param>
        /// <param name="userId"></param>
        /// <param name="passwd"></param>
        /// <param name="scheme"></param>
        /// <param name="port"></param>
        public MysqlConnectWrapper(string tag,string host,string userId,string passwd,string scheme,int port=3306)
        {
            this.Tag = tag;
            StringBuilder sb = new StringBuilder(256);

            sb.Append("server=").Append(host).Append(";");
            sb.Append("userid=").Append(userId).Append(";");
            if (!string.IsNullOrEmpty(passwd))
            {
                sb.Append("password=").Append(passwd).Append(";"); ;
            }
            sb.Append("database=").Append(scheme).Append(";");
            this.connection = new MySqlConnection(sb.ToString());
            this.connection.Open();
        }
        #region TEST_CODE
        public void Test() 
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM kotori.test where id< @i0" , this.connection );
            cmd.Parameters.AddWithValue("@i0", 2);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                System.Console.WriteLine("----- colomns -----");
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    System.Console.WriteLine("" + i + "::" + reader.GetName(i));
                }
                System.Console.WriteLine("----- result -----");
                while (reader.Read())
                {
                    System.Console.WriteLine("" + reader.GetInt32(0) + " :: " + reader.GetString(1));
                }
                reader.Close();
            }
        }

        public void ReflectionTest() {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM kotori.test where id< @i0", this.connection);
            cmd.Parameters.AddWithValue("@i0", 3);

            var list = this.SelectQuery<Application.Sql.Table.Test >(cmd);
            foreach (var item in list) {
                Console.WriteLine("-----------------");
                Console.WriteLine(" id=" + item.id + "// name=" + item.name);
                if (item.val != null)
                {
                    for (int i = 0; i < item.val.Length; ++i)
                    {
                        Console.WriteLine("  val[" + i + "] " + item.val[i]);
                    }
                }
                else {
                    Console.WriteLine("  val is null ");
                }
            }
        }

        #endregion


        /// <summary>
        /// Select query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public List<T> SelectQuery<T>( MySqlCommand cmd )
            where T:class
        {
            List<T> list = null;
            var reflectionResolver = new MysqlRefection<T>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                list = reflectionResolver.ReadFromMysql(reader);
            }
            return list;
        }


        /// <summary>
        /// Dispose Function.
        /// When realese this object will be called
        /// </summary>
        public void Dispose()
        {
            if (this.connection != null) {
                this.connection.Clone();
                this.connection = null;
            }
        }

    }
}
