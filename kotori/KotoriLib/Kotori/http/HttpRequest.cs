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
using System.Net.Sockets;

namespace Kotori.Http
{
    /// <summary>
    /// Http RequestData
    /// </summary>
    public class HttpRequest
    {
        /*-------------- constant vars -----------------*/
        /// <summary>
        /// CR LF Code for http header
        /// </summary>
        private static readonly byte[] CR_LF_CODE = new byte[] { 0x0D, 0x0A };

        public const string GET_METHOD = "GET";
        public const string POST_METHOD = "POST";
        /*-------------- vars -----------------*/

        /// <summary>
        /// Request Body Data
        /// </summary>
        private List<byte> bodyData;

        /// <summary>
        /// Buffer size for read from stream.
        /// </summary>
        private const int BUFFER_SIZE = 256;

        /// <summary>
        /// HTTP header buffer Size
        /// </summary>
        private const int HEADER_BUILDER_SIZE = 256;

        /// <summary>
        /// Http Request Body Size.(first alocate memory)
        /// </summary>
        private const int BODY_BUFFER_SIZE = 512;

        /*------------ properties -------*/
        /// <summary>
        /// Http Headers
        /// </summary>
        public Dictionary<string, string> headers
        {
            private set;
            get;
        }
        /// <summary>
        /// Http Method
        /// </summary>
        public string method
        {
            private set;
            get;
        }

        /// <summary>
        /// request URI
        /// </summary>
        public string uri
        {
            private set;
            get;
        }

        /// <summary>
        /// path 
        /// </summary>
        public string path
        {
            private set;
            get;
        }

        /// <summary>
        /// getで取得してきたパラメーター
        /// </summary>
        public Dictionary<string, string> getParam
        {
            private set;
            get;
        }

        /// <summary>
        /// postで取得してきたパラメーター
        /// </summary>
        public Dictionary<string, string> postParam
        {
            private set;
            get;
        }

        /// <summary>
        /// Postの生データ取得
        /// </summary>
        public List<byte> postRowData {
            get
            {
                return bodyData;
            }
        }
        /*------------ functions -------*/
        /// <summary>
        /// construdter
        /// </summary>
        /// <param name="stream"></param>
        public HttpRequest(NetworkStream stream)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            int readSize = 0;

            this.bodyData = new List<byte>(BODY_BUFFER_SIZE);
            this.headers = new Dictionary<string, string>();


            StringBuilder sb = new StringBuilder(HEADER_BUILDER_SIZE);
            int tmpIdx = 0;
            int readBodySize = 0;
            bool isFirstLine = true;
            bool isHeaderOver = false;
            // http request Header
            while ((readSize = stream.Read(buffer, 0, buffer.Length)) != 0){
                tmpIdx = 0;
                while (tmpIdx < readSize)
                {
                    int oldIdx = tmpIdx;
                    tmpIdx = this.GetNextBufferIndex(buffer, tmpIdx, readSize);
                    sb.Append(System.Text.Encoding.UTF8.GetString(buffer, oldIdx, tmpIdx - oldIdx));
                    if (tmpIdx != readSize)
                    {
                        if (sb.Length == 0) {
                            tmpIdx += 2;
                            isHeaderOver = true;
                            readBodySize = readSize - tmpIdx;
                            this.AddBody(buffer, tmpIdx, readBodySize);
                            break;
                        }
                        if (isFirstLine)
                        {
                            this.ExecFirstLine(sb.ToString());
                            isFirstLine = false;
                        }
                        else {
                            this.AddHeaderLine(sb.ToString());
                        }
                        sb.Length = 0;
                        tmpIdx += 2;
                    }
                }
                if (isHeaderOver)
                {
                    break;
                }
            }
            // http request Body
            if (readSize == buffer.Length && this.method == POST_METHOD )
            {
                int ContentLength = int.Parse( this.headers["Content-Length"] );
                if (readBodySize < ContentLength)
                {
                    while ((readSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        this.AddBody(buffer, 0, readSize);
                        readBodySize += readSize;
                        if (readBodySize >= ContentLength)
                        {
                            break;
                        }
                    }
                }
            }
            
            if( bodyData !=null && bodyData.Count > 0 ){
                UrlParamParse postParse = new UrlParamParse(this.bodyData);
                this.postParam = postParse.parameters;
            }
        }


        /// <summary>
        /// Execute first request firstLine;
        /// </summary>
        /// <param name="str"></param>
        private void ExecFirstLine(string str)
        {
            string[] datas = str.Split(' ');
            if (datas != null && datas.Length <= 2) {
                return;
            }
            this.method = datas[0];
            this.uri = datas[1];

            UrlParamParse parser = new UrlParamParse(this.uri);
            this.path = parser.path;
            this.getParam = parser.parameters;
        }

        /// <summary>
        /// Add Body Byte Data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="idx"></param>
        /// <param name="size"></param>
        private void AddBody(byte[] data, int idx, int size) {
            if (data == null)
            {
                return;
            }
            for (int i = idx; i < idx + size; ++i) {
                this.bodyData.Add(data[i]);
            }
        }

        /// <summary>
        /// Add Header line to dictionary
        /// </summary>
        /// <param name="str"></param>
        private void AddHeaderLine(string str) 
        {
            int separator = str.IndexOf(':');
            if (separator <= 0  ) {
                return;
            }
            string key = str.Substring(0, separator);
            string value = null;
            if (str.Length - 1 > separator) {
                value = str.Substring(separator + 1).TrimStart();
            }
            this.headers[key] = value;
        }

        /// <summary>
        /// returns index that should next read
        /// </summary>
        /// <param name="data">search from this</param>
        /// <param name="readSize">read size</param>
        /// <returns></returns>
        private int GetNextBufferIndex(byte[] data, int start, int readSize)
        {
            for (int i = start; i < readSize - 1; ++i){
                if (data[i] == CR_LF_CODE[0] && data[i+1] == CR_LF_CODE[1])
                {
                    return i;
                }
            }
            return readSize;
        }
    }
}
