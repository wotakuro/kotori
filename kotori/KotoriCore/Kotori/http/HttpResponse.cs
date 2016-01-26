using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Kotori.Http
{
    /// <summary>
    /// http response data
    /// </summary>
    class HttpResponse
    {
        /*----------------- const ----------------*/
        /// <summary>
        /// defalut status
        /// </summary>
        public enum HttpStatus
        {
            Continue                = 100,
            SwitchingProtocols      = 101,
            Processing              = 102,
            OK                      = 200,
            Created                 = 201,
            Accepted                = 202,
            NonAuthoritative        = 203,
            NoContent               = 204,
            ResetContent            = 205,
            PatialContent           = 206,
            MultiStatus             = 207,
            MultipleChoices         = 300,
            MovedPermanently        = 301,
            Found                   = 302,
            SeeOther                = 303,
            NotModified             = 304,
            UseProxy                = 305,
            TemporaryRedirect       = 307,
            PermanentRedirect       = 308,
            BadRequest              = 400,
            Unauthorized            = 401,
            PaymentRequired         = 402,
            Forbidden               = 403,
            NotFound                = 404,
            MethodNotAllowed        = 405,
            NotAcceptable           = 406,
            ProxyAuthenication      = 407,
            RequestTimeout          = 408,
            Conflict                = 409,
            Gone                    = 410,
            LengthRequired          = 411,
            PreconditionFailed      = 412,
            RequestEntityTooLarge   = 413,
            RequestURITooLong       = 414,
            UnSupportedMediaType    = 415,
            RequestRangeNot         = 416,
            ExpectationFailed       = 417,
            InternalServerError     = 500,
            NotImplemented          = 501,
            BadGateWay              = 502,
            ServiceUnavailable      = 503,
            GatewayTimeout          = 504,
            HttpVersionNotSupported = 505,
            BandwidthLimitExceeded  = 506,
        };

        /// <summary>
        /// Write body buffer
        /// </summary>
        private const int BODY_WRITE_BUFFER = 256;

        /// <summary>
        /// CR LF Code for http header
        /// </summary>
        private static readonly byte[] CR_LF_CODE = new byte[] { 0x0D, 0x0A };
        /*----------------- vars ----------------*/
        /// <summary>
        /// body data
        /// </summary>
        private List<byte> bodyData;

        /*--------------- properties --------------------*/
        /// <summary>
        /// header
        /// </summary>
        public Dictionary<string, string> header
        {
            private set;
            get;
        }

        /// <summary>
        /// http status
        /// </summary>
        public HttpStatus status
        {
            set;
            get;
        }

        /*---------------- functions ---------------------*/

        /// <summary>
        /// constructer
        /// </summary>
        public HttpResponse()
        {
            this.bodyData = new List<byte>();
            this.header = new Dictionary<string, string>();
            this.status = HttpStatus.OK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Write( byte []data ) 
        {
            this.Write(data, 0, data.Length);
        }

        /// <summary>
        /// write to data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        public void Write(byte[] data, int index, int size) 
        { 
            int edPoint = index + size;
            for (int i = index; i < edPoint; ++i) {
                this.bodyData.Add(data[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void SendData(NetworkStream stream)
        {
            this.ResolveHeaders();
            this.WriteHeader(stream);
            this.WriteData(stream);
        }

        /// <summary>
        ///  write header stream
        /// </summary>
        /// <param name="stream"></param>
        private void WriteHeader(NetworkStream stream)
        { 
            StringBuilder sb = new StringBuilder(64);
            // first line
            sb.Append("HTTP/1.1 ").Append( (int)this.status);
            sb.Append( GetHttpStatusString(this.status) );
            byte[] writeBin = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            stream.Write(writeBin, 0, writeBin.Length);
            stream.Write(CR_LF_CODE, 0, CR_LF_CODE.Length);
            // header
            foreach (var hData in this.header)
            {
                sb.Clear();
                sb.Append(hData.Key).Append(": ").Append(hData.Value);
                writeBin = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                stream.Write(writeBin, 0, writeBin.Length);
                stream.Write(CR_LF_CODE, 0, CR_LF_CODE.Length);
            }
            stream.Write(CR_LF_CODE, 0, CR_LF_CODE.Length);
        }

        /// <summary>
        /// write data to stream
        /// </summary>
        /// <param name="stream"></param>
        private void WriteData( NetworkStream stream)
        {
            byte[] tmpData = new byte[BODY_WRITE_BUFFER];
            int leftSize = this.bodyData.Count;
            int index = 0;
            while (leftSize > 0) { 
                int size = Math.Min(BODY_WRITE_BUFFER , leftSize);
                for (int i = 0; i < size; ++i) {
                    tmpData[i] = this.bodyData[index];
                    ++index;
                }
                stream.Write( tmpData , 0, size);
                leftSize -= size;
            }
        }

        /// <summary>
        /// Resolve http Header
        /// </summary>
        private void ResolveHeaders()
        {
            DateTime dt = DateTime.Now;
            this.header["Content-Length"] = this.bodyData.Count.ToString();
            this.header["Date"] = dt.ToUniversalTime().ToString("r");
            this.header["Connection"] = "close";
            this.header["Server"] = "Kotori/0.1";
        }

        /// <summary>
        /// Http status code
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private string GetHttpStatusString(HttpStatus state) {
            switch (state) { 
                case HttpStatus.Continue:
                    return "Continue";
                case HttpStatus.SwitchingProtocols:
                    return "Switching Protocols";
                case HttpStatus.Processing:
                    return "Processing";
                // 200
                case HttpStatus.OK:
                    return "OK";
                case HttpStatus.Created:
                    return "Created";
                case HttpStatus.Accepted:
                    return "Accepted";
                case HttpStatus.NonAuthoritative:
                    return "Non-Authoritative Information";
                case HttpStatus.NoContent:
                    return "No Content";
                case HttpStatus.ResetContent:
                    return "Reset Content";
                case HttpStatus.PatialContent:
                    return "Patial Content";
                case HttpStatus.MultiStatus:
                    return "Multi-Status";
                // 300
                case HttpStatus.MultipleChoices:
                    return "Multiple Choices";
                case HttpStatus.MovedPermanently:
                    return "Moved Permanently";
                case HttpStatus.Found:
                    return "Found";
                case HttpStatus.SeeOther:
                    return "See Other";
                case HttpStatus.NotModified:
                    return "Not Modified";
                case HttpStatus.UseProxy:
                    return "Use Proxy";
                case HttpStatus.TemporaryRedirect:
                    return "Temporary Redirect";
                case HttpStatus.PermanentRedirect:
                    return "Permanent Redirect";
                // 400
                case HttpStatus.BadRequest:
                    return "Bad Request";
                case HttpStatus.Unauthorized:
                    return "Unauthorized";
                case HttpStatus.PaymentRequired:
                    return "Payment Required";
                case HttpStatus.Forbidden:
                    return "Forbidden";
                case HttpStatus.NotFound:
                    return "Not Found";
                case HttpStatus.MethodNotAllowed:
                    return "Method Not Allowed";
                case HttpStatus.NotAcceptable:
                    return "Not Acceptable";
                case HttpStatus.ProxyAuthenication:
                    return "Proxy Authenication Required";
                case HttpStatus.RequestTimeout:
                    return "Request Timeout";
                case HttpStatus.Conflict:
                    return "Conflict";
                case HttpStatus.Gone:
                    return "Gone";
                case HttpStatus.LengthRequired:
                    return "Length Required";
                case HttpStatus.PreconditionFailed:
                    return "Precondition Failed";
                case HttpStatus.RequestEntityTooLarge:
                    return "Request Entity Too Large";
                case HttpStatus.RequestURITooLong:
                    return "Request URI Too Long";
                case HttpStatus.UnSupportedMediaType:
                    return "UnSupported Media Type";
                case HttpStatus.RequestRangeNot:
                    return "Request Range Not Satisfiable";
                case HttpStatus.ExpectationFailed:
                    return "Expectation Failed";
                // 500
                case HttpStatus.InternalServerError:
                    return "Internal Server Error";
                case HttpStatus.NotImplemented:
                    return "Not Implemented";
                case HttpStatus.BadGateWay:
                    return "Bad GateWay";
                case HttpStatus.ServiceUnavailable:
                    return "Service Unavailable";
                case HttpStatus.GatewayTimeout:
                    return "Gateway Timeout";
                case HttpStatus.HttpVersionNotSupported:
                    return "Http Version Not Supported";
                case HttpStatus.BandwidthLimitExceeded:
                    return "Bandwidth Limit Exceeded";

            }
            return "OK";
        }
    }
}
