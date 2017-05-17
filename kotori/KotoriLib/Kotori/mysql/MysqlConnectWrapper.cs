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
        /*
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
        }*/

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
