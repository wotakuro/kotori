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
using System.Reflection;

using MySql.Data.MySqlClient;

namespace Kotori.Mysql
{
    class MysqlRefection<T>
        where T:class
    {
        /// <summary>
        /// support types.
        /// </summary>
        enum ETypes { 
            TypeNoSupport,
            TypeInt,
            TypeBigInt,
            TypeString,
            TypeDate,
        }
        /// <summary>
        /// object type
        /// </summary>
        private Type createType;


        private ETypes[] importTypes;
        private FieldInfo[] importFieldInfo;


        /// <summary>
        /// constructor
        /// </summary>
        public MysqlRefection()
        {
            this.createType = typeof(T);
        }

        /// <summary>
        /// Read from sql
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> ReadFromMysql(MySqlDataReader reader)
        {
            List<T> list = new List<T>();
            this.SetupInsertInfo(reader);

            while (reader.Read())
            {
                T obj = this.CreateObject();
                this.SetObjectData(obj, reader);
                list.Add( obj );
            }
            reader.Close();

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void SetupInsertInfo( MySqlDataReader reader)
        {
            this.importTypes = new ETypes[reader.FieldCount];
            this.importFieldInfo = new FieldInfo[reader.FieldCount];

            for (int i = 0; i < reader.FieldCount; ++i)
            {
                this.importFieldInfo[i] = this.createType.GetField(reader.GetName(i));
                this.importTypes[i] = this.GetFieldImportType(this.importFieldInfo[i]);
            }
        }


        /// <summary>
        /// get 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private ETypes GetFieldImportType(FieldInfo info)
        {
            if (info == null) {
                return ETypes.TypeNoSupport;
            }
            if (info.FieldType == typeof(int))
            {
                return ETypes.TypeInt;
            }
            else if (info.FieldType == typeof(string))
            {
                return ETypes.TypeString;
            }
            return ETypes.TypeNoSupport;
        }
        
        /// <summary>
        /// set ObjectData
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reader"></param>
        private void SetObjectData( T obj , MySqlDataReader reader )
        {
            for (int i = 0; i < this.importTypes.Length; ++i) {
                switch (this.importTypes[i]) { 
                    case ETypes.TypeNoSupport:
                        break;
                    case ETypes.TypeInt:
                        this.importFieldInfo[i].SetValue(obj, reader.GetInt32(i) );
                        break;
                    case ETypes.TypeBigInt:
                        this.importFieldInfo[i].SetValue(obj, reader.GetInt64(i) );
                        break;
                    case ETypes.TypeString:
                        this.importFieldInfo[i].SetValue(obj, reader.GetString(i) );
                        break;
                    case ETypes.TypeDate:
                        break;
                }
            }
        }

        /// <summary>
        /// Create T.
        /// </summary>
        /// <returns></returns>
        private T CreateObject()
        {
            return Activator.CreateInstance( this.createType ) as T;
        }
    }
}
