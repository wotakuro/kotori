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
using System.Reflection;

namespace Kotori.Json
{
    /// <summary>
    /// Object Field Dicionary for Json parse using reflection.
    /// </summary>
    class ObjectDictionaryForJson
    {
        /// <summary>
        /// the type list for json parser.
        /// </summary>
        public enum EType
        {
            NotSupported,
            Integer,
            BigInteger,
            String,
            Date,
            Object,
            IntegerArray,
            BigIntegerArray,
            StringArray,
            DateArray,
            ObjectArray,
        }
        /// <summary>
        /// Dictionary of Data
        /// </summary>
        private Dictionary<string, EType> typeDictionary = null;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="t">Type of Data</param>
        public ObjectDictionaryForJson(Type t)
        {
            if (t == null)
            {
                return;
            }
            FieldInfo[] fields = t.GetFields( );
            if (fields == null) {
                return;
            }
            this.typeDictionary = new Dictionary<string, EType>( fields.Length );

            foreach (FieldInfo field in fields) {
                this.typeDictionary.Add(field.Name, this.GetFieldInfoType(field));
            }
        }

        public void DebugPrint()
        {
            Console.WriteLine("json debug start ");
            foreach (var kvp in this.typeDictionary) {
                Console.WriteLine(kvp.Key.ToString() + "::" + kvp.Value.ToString());
            }
            Console.WriteLine("json debug end ");
        }

        /// <summary>
        /// Get Type for json parser.
        /// </summary>
        /// <param name="name">var name</param>
        /// <returns>type for json parser</returns>
        public EType GetTypeFromVarName(string name)
        {
            if (this.typeDictionary == null || !this.typeDictionary.ContainsKey(name)) {
                return EType.NotSupported;
            }
            return this.typeDictionary[name];
        }

        /// <summary>
        /// Get type for json parser from fieldInfo.
        /// </summary>
        /// <param name="field"> field Info from reflection </param>
        /// <returns> type for json parser </returns>
        private EType GetFieldInfoType( FieldInfo field )
        {
            Type t = field.FieldType;
            if (t == typeof(int)) {
                return EType.Integer;
            }
            else if (t == typeof(long)) {
                return EType.BigInteger;
            }
            else if (t == typeof(string))
            {
                return EType.String;
            }
            else if (t == typeof(DateTime))
            {
                return EType.Date;
            }
            else if (t == typeof(int[]) )
            {
                return EType.IntegerArray;
            }
            else if (t == typeof(long[]))
            {
                return EType.BigIntegerArray;
            }
            else if (t == typeof(DateTime[]))
            {
                return EType.DateArray;
            }
            else if (t == typeof(string[]))
            {
                return EType.StringArray;
            }
            else if (t.IsArray)
            {
                return EType.ObjectArray;
            }
            else if (t.IsClass)
            {
                return EType.Object;
            }

            return EType.NotSupported;
        }

    }
}
