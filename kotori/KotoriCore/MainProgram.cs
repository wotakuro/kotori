using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MainProgram
{
    public static void Main(string[] args)
    {
        var asm = System.Reflection.Assembly.GetExecutingAssembly();
        Kotori.KotoriCore.MainExecute(asm);
    }
}
