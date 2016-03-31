using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Mysql
{
    internal class PoolingList<T> where T:class
    {
        private string poolTag;
        private int maxNum;
        private LinkedList<T> activeList;
        private LinkedList<T> releasedList;
        private Func<string,T> generateMethod;
        private Func<T,bool> aliveCheckMethod;

        public PoolingList(string tag,int num, Func<string,T> genMethod,Func<T,bool> aliveMethod)
        {
            this.poolTag = tag;
            this.maxNum = num;
            this.generateMethod = genMethod;
            this.aliveCheckMethod = aliveMethod;
            this.activeList = new LinkedList<T>();
            this.releasedList = new LinkedList<T>();
        }

        public T Allocate()
        {
            int activeNum = this.activeList.Count ;
            int releasedNum = this.releasedList.Count;
            if ( activeNum + releasedNum > this.maxNum)
            {
                return null;
            }
            if (releasedNum > 0 )
            {
                for (int i = 0; i < releasedNum; ++i)
                {
                    var obj = this.releasedList.First.Value;
                    if (this.aliveCheckMethod(obj))
                    {
                        this.activeList.AddLast(obj);
                        return obj;
                    }
                    this.releasedList.RemoveFirst();
                }
            }
            var newObj = this.generateMethod(this.poolTag);
            this.activeList.AddLast(newObj);
            return newObj;
        }

        public void Release( T obj ){
            this.activeList.Remove(obj);
            this.releasedList.AddFirst(obj);
        }
    }
}
