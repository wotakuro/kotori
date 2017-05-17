using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kotori.Http;
using Kotori.Attribute;

namespace Application.Module
{
    /// <summary>
    ///  テスト用のモジュール
    /// </summary>
    [WebModule("/test")]
    class TestModule : Kotori.Module.IWebModule
    {
        /// <summary>
        /// テスト用のモジュール
        /// </summary>
        public TestModule()
        {
        }
        
        /// <summary>
        /// 実行処理を行います
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public virtual void Exec( HttpRequest request , HttpResponse response)
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append("this is a test page");

            response.Write(System.Text.Encoding.UTF8.GetBytes(sb.ToString()));
        }
    }
}
