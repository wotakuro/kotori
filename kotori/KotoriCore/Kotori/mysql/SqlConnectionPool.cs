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
            return null;
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
