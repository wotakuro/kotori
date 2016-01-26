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
    [WebModule("/test2")]
    class TestModule2 : TestModule
    {


        /// <summary>
        /// 実行処理を行います
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public override void Exec(HttpRequest request, HttpResponse response)
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append("this is a test2 page");
            response.Write(System.Text.Encoding.UTF8.GetBytes(sb.ToString()));
        }
    }
}
