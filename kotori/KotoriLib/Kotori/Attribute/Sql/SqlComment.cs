using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Attribute.Sql
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class SqlComment : System.Attribute
    {
        private string comment;

        public SqlComment(string str) {
            this.comment = str;
        }
        public string GetComment() {
            return this.comment;
        }
    }
}
