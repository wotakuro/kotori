using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotori.Module
{
    class WebModuleResolver
    {
        /// <summary>
        /// module dictionary
        /// </summary>
        Dictionary<string, Type> moduleDictionary;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="types">all types in this application</param>
        public WebModuleResolver(Type[] types) 
        {
            this.moduleDictionary = new Dictionary<string, Type>();
            foreach (var t in types)
            {
                bool isWebModule = this.IsImplementsWebModule( t );
                if (isWebModule) {
                    string moduleName = GetModuleName(t);
                    if (moduleName != null) {
                        this.moduleDictionary.Add(moduleName, t);
                    }
                }
            }
        }

        /// <summary>
        /// create web object from uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public IWebModule CreateWebModule(string uri)
        {
            if (uri == null || !this.moduleDictionary.ContainsKey(uri))
            {
                return null;
            }
            Type t = this.moduleDictionary[uri];
            return Activator.CreateInstance(t) as IWebModule;
        }

        /// <summary>
        /// Check the type is Implements IWebModule?
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool IsImplementsWebModule(Type t)
        {
            foreach (var inter in t.GetInterfaces())
            {
                if (inter == typeof(Kotori.Module.IWebModule))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Module name from typeObject ( attribute ).
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private string GetModuleName(Type t) {
            var attributeList = t.GetCustomAttributes(typeof(Kotori.Attribute.WebModule), false);
            foreach (var attr in attributeList)
            {
                Kotori.Attribute.WebModule attrWebModule = attr as Kotori.Attribute.WebModule;
                if (attrWebModule != null)
                {
                    System.Console.WriteLine(" web module attribute " + attrWebModule.GetModuleName());
                    return attrWebModule.GetModuleName();
                }
            }
            return null;
        }
    }
}
