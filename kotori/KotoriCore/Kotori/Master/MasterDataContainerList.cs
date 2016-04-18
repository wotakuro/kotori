using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Master
{
    public class MasterDataContainerList<T>
    where T:class{
        private string queryString;
        private List<T> resultList;

        public MasterDataContainerList() { 
        }

        public virtual void SetResult( List<T> data) {
        }
    }
}
