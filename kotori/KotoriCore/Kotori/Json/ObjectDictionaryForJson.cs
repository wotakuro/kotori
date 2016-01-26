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
