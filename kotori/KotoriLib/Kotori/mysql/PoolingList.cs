#region LICENSE
/**
The MIT License (MIT)

Copyright (c) 2016 Yusuke Kurokawa

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion LICENSE

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
