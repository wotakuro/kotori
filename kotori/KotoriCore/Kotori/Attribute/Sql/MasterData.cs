using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Attribute.Sql
{

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class MasterData : System.Attribute
    {
        private string selectSql;
        public MasterData(string sql)
        {
            this.selectSql = sql;
        }
        public string GetCacheSql()
        {
            return this.selectSql;
        }
    }
}
