using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Module
{
    public class InitializeResolver
    {
        public static void Initialize(Type[] types)
        {
            new InitializeResolver(types);
        }
        private InitializeResolver(Type[] types)
        {
            foreach(var type in types){
                if (IsImplementsInitialize(type))
                {
                    var obj = CreateWebModule(type);
                    if (obj != null)
                    {
                        obj.OnInitialize();
                    }
                }
            }
        }


        public IProgramInitializeExec CreateWebModule(Type t)
        {
            return Activator.CreateInstance(t) as IProgramInitializeExec;
        }



        /// <summary>
        /// Check the type is Implements IProgramInitializeExec?
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool IsImplementsInitialize(Type t)
        {
            foreach (var inter in t.GetInterfaces())
            {
                if (inter == typeof(Kotori.Module.IProgramInitializeExec))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
