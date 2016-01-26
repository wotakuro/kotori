using System;
using System.Collections.Generic;
using Kotori.Http;

namespace Kotori.Module
{
    /// <summary>
    /// a interface of web module class.
    /// all module class should implements this interface.
    /// </summary>
    interface IWebModule
    {
        /// <summary>
        /// Exect module
        /// </summary>
        /// <param name="request">request data</param>
        /// <param name="response">response data</param>
        void Exec(HttpRequest request, HttpResponse response);
    }
}
