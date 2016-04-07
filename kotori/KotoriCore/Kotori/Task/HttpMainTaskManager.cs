using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Task
{
    public class HttpMainTaskManager
    {
        private LinkedList<HttpMainTask> waitHttpTaskList;
        
        private static HttpMainTaskManager s_this = new HttpMainTaskManager();

        /// <summary>
        /// get singleton instance
        /// </summary>
        public static HttpMainTaskManager Instance
        {
            get
            {
                return s_this;
            }
        }

        /// <summary>
        /// initialize
        /// </summary>
        /// <param name="maxThread">max thread num</param>
        public void Initialize(int maxThread)
        {
            waitHttpTaskList = new LinkedList<HttpMainTask>();
            for (int i = 0; i < maxThread; ++i)
            {
                waitHttpTaskList.AddLast(new HttpMainTask(this.BackToWaitTask));
            }
        }

        /// <summary>
        /// get wait task
        /// </summary>
        /// <returns>wait task</returns>
        public HttpMainTask GetWaitTask()
        {
            lock(this){
                if (this.waitHttpTaskList.First == null)
                {
                    return null;
                }
                return this.waitHttpTaskList.First.Value;
            }
        }

        /// <summary>
        /// when task is
        /// </summary>
        /// <param name="task"></param>
        private void BackToWaitTask(HttpMainTask task)
        {
            lock (this)
            {
                this.waitHttpTaskList.AddLast(task);
            }
        }
    }
}
