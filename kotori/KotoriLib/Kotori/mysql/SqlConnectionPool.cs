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
using Kotori.Config;

namespace Kotori.Mysql
{
    public class SqlConnectionPool
    {
        private static SqlConnectionPool s_this = new SqlConnectionPool();
        private Dictionary<string, PoolingList<MysqlConnectWrapper>> poolingDictionary;
        private Dictionary<string, DataBaseConnectionConfig> configDictionary;

        public static SqlConnectionPool Instance
        {
            get
            {
                return s_this;
            }
        }

        internal void Initialize(DataBaseConnectionConfig[] settingArray)
        {
            if (settingArray == null)
            {
                return;
            }
            this.InitializeConfig(settingArray);
            this.InitilizePoolingList(this.configDictionary);
        }


        public MysqlConnectWrapper AllocConnection(string name)
        {
            if (!this.poolingDictionary.ContainsKey(name))
            {
                return null;
            }
            var poolList = this.poolingDictionary[name];
            if (poolList == null)
            {
                return null;
            }
            return poolList.Allocate();
        }
        public void ReleaseConnection(MysqlConnectWrapper obj)
        {
            var name = obj.Tag;
            if (!this.poolingDictionary.ContainsKey(name))
            {
                return ;
            }
            var poolList = this.poolingDictionary[name];
            if (poolList == null)
            {
                return ;
            }
            poolList.Release(obj);
        }

        private PoolingList<MysqlConnectWrapper> CreatePoolingListObject(DataBaseConnectionConfig config)
        {
            PoolingList<MysqlConnectWrapper> poolList =
                new PoolingList<MysqlConnectWrapper>( 
                    config.accessName,
                    config.poolNum,
                    this.GenerateConnection,
                    this.IsAliveConnection);
            return poolList;
        }

        private bool IsAliveConnection(MysqlConnectWrapper obj)
        {
            return true;
        }
        private MysqlConnectWrapper GenerateConnection(string tag)
        {
            if (!this.configDictionary.ContainsKey(tag))
            {
                return null;
            }
            var config = this.configDictionary[tag];
            if (config == null)
            {
                return null;
            }
            return new MysqlConnectWrapper(tag, config.host, config.user, config.password, config.database, config.port);
        }

        private void InitializeConfig(DataBaseConnectionConfig[] settingArray)
        {
            this.configDictionary = new Dictionary<string, DataBaseConnectionConfig>();
            foreach (var setting in settingArray)
            {
                if (setting == null)
                {
                    continue;
                }
                if (!this.configDictionary.ContainsKey(setting.accessName))
                {
                    this.configDictionary.Add(setting.accessName, setting);
                }
            }
        }

        private void InitilizePoolingList(Dictionary<string, DataBaseConnectionConfig> config) {
            this.poolingDictionary = new Dictionary<string, PoolingList<MysqlConnectWrapper>>();
            foreach (var kvs in config)
            {
                if (!this.poolingDictionary.ContainsKey(kvs.Key))
                {
                    this.poolingDictionary.Add(kvs.Key, CreatePoolingListObject(kvs.Value));
                }
            }
        }

    }
}
