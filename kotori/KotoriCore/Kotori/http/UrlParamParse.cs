using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Http
{
    /// <summary>
    /// Parse url parameter
    /// </summary>
    class UrlParamParse
    {
        /*--------- constants ---------------*/
        /// <summary>
        /// buffer size
        /// </summary>
        private const int BUFFER_SIZE = 256;
        /*--------- vars ---------------*/

        /*--------- properties ---------------*/
        /// <summary>
        /// file path only( for GET method)
        /// </summary>
        public string path
        {
            private set;
            get;
        }


        /// <summary>
        /// parameters
        /// </summary>
        public Dictionary<string, string> parameters
        {
            private set;
            get;
        }


        /*--------- funcions ---------------*/
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="uri"></param>
        public UrlParamParse(string uri)
        {
            int questionIdx = uri.IndexOf('?');
            if (questionIdx < 0)
            {
                this.path = uri;
                return;
            }
            this.path = uri.Substring(0, questionIdx);
            this.ParseParams(uri, questionIdx + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="idx"></param>
        private void ParseParams(string str, int idx)
        {
            this.parameters = new Dictionary<string, string>();
            StringBuilder sbKey = new StringBuilder(16);
            StringBuilder sbVal = new StringBuilder(32);
            int length = str.Length;
            while (idx < length)
            {
                // get key
                for (; idx < length; ++idx)
                {
                    char ch = str[idx];
                    if (ch == '=')
                    {
                        break;
                    }
                    sbKey.Append(ch);
                }
                ++idx;
                // get value
                for (; idx < length; ++idx)
                {
                    char ch = str[idx];
                    if (ch == '&')
                    {
                        break;
                    }
                    sbVal.Append(ch);
                }
                if (!parameters.ContainsKey(sbKey.ToString()))
                {
                    parameters.Add(sbKey.ToString(), sbVal.ToString());
                    sbKey.Clear();
                    sbVal.Clear();
                }
                ++idx;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="data"></param>
        public UrlParamParse(List<byte> data)
        {
            StringBuilder sb = new StringBuilder(BUFFER_SIZE);
            byte[] buffer = new byte[BUFFER_SIZE];
            int loopNum = (data.Count + BUFFER_SIZE - 1) / BUFFER_SIZE;
            int leftLength = data.Count;
            int index = 0;
            for (int i = 0; i < loopNum; ++i)
            {
                data.CopyTo(index, buffer, 0, Math.Min(leftLength, BUFFER_SIZE) );
                string tmpStr = System.Text.Encoding.UTF8.GetString(buffer);
                index += BUFFER_SIZE;
                leftLength -= BUFFER_SIZE;
                sb.Append(tmpStr);
            }
            //System.Console.WriteLine(sb.ToString());
            this.ParseParams(sb.ToString(), 0);
        }
    }
}
