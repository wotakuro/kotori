using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Attribute.Sql
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class PrimaryKey: System.Attribute
    {
        private int order = 0;
        public PrimaryKey()
        {
            this.order = 0;
        }
        public PrimaryKey(int o)
        {
            this.order = o;
        }
        public int GetOrder()
        {
            return this.order;
        }
    }
}
