using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kotori.Module;

namespace Application.module
{
    public class InitilizeExec : IProgramInitializeExec
    {
        public void OnInitialize() {
            System.Console.WriteLine("onInitialized");
        }
    }
}
