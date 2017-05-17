using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kotori.Attribute.Sql;

namespace Application.Sql.Table
{
    [MasterData("Select * from test" , typeof(Kotori.Master.MasterDataContainerList<Test>))]
    class Test
    {
        public readonly int id = 0;
        public readonly string name = null;

        public readonly int val_00 = 0 ;
        public readonly int val_01 = 0;
        public readonly int val_02 = 0;
        public readonly int val_03 = 0;

        private int[] valArray = null;
        public int[] val{
            get
            {
                if (valArray == null)
                {
                    valArray = new int[] { val_00, val_01, val_02, val_03 };
                }
                return valArray;
            }
        }
    }
}
