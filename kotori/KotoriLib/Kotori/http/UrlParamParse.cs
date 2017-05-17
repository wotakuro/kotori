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

namespace Kotori.Http
{
    /// <summary>
    /// Parse url parameter
    /// </summary>
    public class UrlParamParse
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
