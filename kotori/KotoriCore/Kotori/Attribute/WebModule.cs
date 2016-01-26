using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    class WebModule : System.Attribute
    {
        private string moduleName;
        public WebModule(string name)
        {
            this.moduleName = name;
        }

        public string GetModuleName()
        {
            return this.moduleName;
        }
    }
}
