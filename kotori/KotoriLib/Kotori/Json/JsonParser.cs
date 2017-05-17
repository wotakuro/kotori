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

#define JSON_DEBUG


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace Kotori.Json
{
    /// <summary>
    /// Json parser class.
    /// </summary>
    public class JsonParser
    {
        /// <summary>
        /// Object start simbol,
        /// </summary>
        private const char OBJECT_START = '{';
        /// <summary>
        /// Object end simbol
        /// </summary>
        private const char OBJECT_END = '}';
        /// <summary>
        /// Array start simbol.
        /// </summary>
        private const char ARRAY_START = '[';
        /// <summary>
        /// Array end simbol.
        /// </summary>
        private const char ARRAY_END = ']';
        /// <summary>
        /// string separate simbol
        /// </summary>
        private const char STRING_SEPARATOR = '"';
        /// <summary>
        /// object separate simbol
        /// </summary>
        private const char OBJECT_SEPARATOR = ':';
        /// <summary>
        /// comma
        /// </summary>
        private const char COMMNA = ',';
        /// <summary>
        /// back slash (\)
        /// </summary>
        private const char STRING_BACKSLASH = '\\';


        /// <summary>
        /// read Index
        /// </summary>
        private int currentIdx = 0;
        /// <summary>
        /// current string
        /// </summary>
        private char[] currentStr;
        /// <summary>
        /// StringBuilder Buffer.
        /// </summary>
        private StringBuilder strBuilder;

        /// <summary>
        /// dictionary data for json to object.
        /// </summary>
        private Dictionary<Type, ObjectDictionaryForJson> objectMemberDictionary;


        /// <summary>
        ///  debug print
        /// </summary>
        /// <param name="str"></param>
        private void DebugPrint(string str) {
            System.Console.WriteLine(str);
        }

        private void DebugPrint(string str, int pidx, int idx) {
            System.Console.Write(str+"  "+ pidx+"->"+idx);
            System.Console.Write("   ");
            for (int i = pidx; i < idx; ++i)
            {
                System.Console.Write(this.currentStr[i]);
            }
            System.Console.WriteLine();
        }

        /// <summary>
        /// parse json data
        /// </summary>
        /// <typeparam name="T">class data</typeparam>
        /// <param name="jsonStr">json string data</param>\
        /// <returns></returns>
        public static T ParseJsonToObject<T>(string jsonStr) where T : class
        {
            char []chArr = jsonStr.ToCharArray();

            JsonParser parser = new JsonParser(chArr);
            return parser.ReadObject<T>();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="chArr">string array</param>
        private JsonParser(char[] chArr)
        {
            this.currentStr = chArr;
            this.currentIdx = 0;
            this.strBuilder = new StringBuilder();
            this.objectMemberDictionary = new Dictionary<Type, ObjectDictionaryForJson>();
        }

        /// <summary>
        /// read as object
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <returns>object that created</returns>
        private T ReadObject<T>() where T: class
        {
            Type t = typeof(T);
            return this.ReadObject(t) as T;
        }

        /// <summary>
        /// Read object
        /// </summary>
        /// <param name="t">type of object</param>
        /// <returns>object that created </returns>
        private Object ReadObject(Type t)
        {
            Object tmp = null;
            this.SkipEmptyString();
            if (this.currentStr[this.currentIdx] != OBJECT_START)
            {
                return null;
            }
            ObjectDictionaryForJson typeDict = this.GetDictionaryForJson(t);
            ++this.currentIdx;
            tmp = Activator.CreateInstance(t) ;
            while (true)
            {
                string varName = this.ReadObjectVarName();
                this.SkipEmptyString();
                if (this.currentStr[this.currentIdx] != OBJECT_SEPARATOR)
                {
                    break;
                }
                ++this.currentIdx;
                this.SkipEmptyString();
                ObjectDictionaryForJson.EType varType = typeDict.GetTypeFromVarName(varName);

                DebugPrint(varName + "::" + varType);
                switch (varType)
                {
                    case ObjectDictionaryForJson.EType.NotSupported:
                        this.SkipVarValue();
                        break;
                    case ObjectDictionaryForJson.EType.Integer:
                        t.GetField(varName).SetValue(tmp, this.ReadIntValue());
                        break;
                    case ObjectDictionaryForJson.EType.BigInteger:
                        t.GetField(varName).SetValue(tmp, this.ReadLongValue());
                        break;
                    case ObjectDictionaryForJson.EType.String:
                        t.GetField(varName).SetValue(tmp, this.ReadStringValue());
                        break;
                    case ObjectDictionaryForJson.EType.Date:
                        break;
                    case ObjectDictionaryForJson.EType.Object:
                        t.GetField(varName).SetValue(tmp, this.ReadObject(t.GetField(varName).FieldType));
                        break;
                    case ObjectDictionaryForJson.EType.IntegerArray:
                        t.GetField(varName).SetValue(tmp, this.ReadIntArray());
                        break;
                    case ObjectDictionaryForJson.EType.BigIntegerArray:
                        t.GetField(varName).SetValue(tmp, this.ReadLongArray());
                        break;
                    case ObjectDictionaryForJson.EType.StringArray:
                        t.GetField(varName).SetValue(tmp, this.ReadStringArray());
                        break;
                    case ObjectDictionaryForJson.EType.DateArray:
                        break;
                    case ObjectDictionaryForJson.EType.ObjectArray:
                        Type arrayType = t.GetField(varName).FieldType.GetElementType();
                        t.GetField(varName).SetValue(tmp, this.ReadObjectArray(arrayType)  );
                        break;
                }
                this.SkipEmptyString();
                if (this.currentStr[this.currentIdx] == COMMNA)
                {
                    ++this.currentIdx;
                }
                else if (this.currentStr[this.currentIdx] == OBJECT_END)
                {
                    ++this.currentIdx;
                    break;
                }
                else
                {
                    break;
                }
            }
            return tmp;
        }

        /// <summary>
        /// read object as a object array.
        /// </summary>
        /// <returns>read value</returns>
        private Object[] ReadObjectArray(Type t)
        {
            if (this.currentStr[currentIdx] != ARRAY_START)
            {
                return null;
            }
            Object[] result =             
                Activator.CreateInstance( t.MakeArrayType() , new object[]{ this.CountArrayNum(this.currentIdx)} ) as Object[];

            ++this.currentIdx;
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = this.ReadObject(t);
                this.SkipEmptyString();
                if (this.currentStr[this.currentIdx] != COMMNA)
                {
                    break;
                }
                ++this.currentIdx;
            }
            if (this.currentStr[currentIdx] == ARRAY_END)
            {
                ++currentIdx;
            }
            this.SkipEmptyString();
            return result;
        }

        /// <summary>
        /// dictionary data for json
        /// </summary>
        /// <param name="t">type</param>
        /// <returns>dictionary</returns>
        private ObjectDictionaryForJson GetDictionaryForJson(Type t)
        {
            if (this.objectMemberDictionary.ContainsKey(t))
            {
                return this.objectMemberDictionary[t];
            }
            ObjectDictionaryForJson dict = new ObjectDictionaryForJson(t);
            this.objectMemberDictionary.Add(t, dict);
            return dict;
        }

        /// <summary>
        /// read object name
        /// </summary>
        /// <returns>object value</returns>
        private string ReadObjectVarName()
        {
            this.strBuilder.Length = 0;
            // skip empy strings
            this.SkipEmptyString();
            // check double quate
            if (this.currentStr[this.currentIdx] == '"')
            {
                ++this.currentIdx;
            }
            for (; this.currentIdx < this.currentStr.Length; ++this.currentIdx) 
            {
                if (this.currentStr[this.currentIdx] == ':' || this.currentStr[this.currentIdx] == ':' ||
                    this.currentStr[this.currentIdx] == '"' ||
                    this.currentStr[this.currentIdx] == ' ' || this.currentStr[this.currentIdx] == '\t')
                {
                    break;
                }
                this.strBuilder.Append(currentStr[this.currentIdx]);
            }
            // check double quate
            if (this.currentStr[this.currentIdx] == '"')
            {
                ++this.currentIdx;
            }
            this.SkipEmptyString();
            return this.strBuilder.ToString();
        }

        /// <summary>
        /// read value as Int
        /// </summary>
        /// <returns>read value</returns>
        private int ReadIntValue()
        {
            int val= 0;
            this.SkipEmptyString();
            this.strBuilder.Length = 0;
            if (this.currentStr[currentIdx] == '-')
            {
                this.strBuilder.Append(this.currentStr[this.currentIdx]);
                ++this.currentIdx;
            }
            else if (this.currentStr[currentIdx] == '+')
            {
                this.strBuilder.Append(this.currentStr[this.currentIdx]);
                ++this.currentIdx;
            }
            this.SkipEmptyString();
            for (; ('0' <= this.currentStr[this.currentIdx]) && (this.currentStr[this.currentIdx] <= '9') || this.currentStr[this.currentIdx] == '.';
                ++this.currentIdx)
            {
                this.strBuilder.Append(this.currentStr[this.currentIdx]);
            }
            try
            {
                val = int.Parse(this.strBuilder.ToString());
            }
            catch (Exception e) { 
            }
            return val;
        }

        /// <summary>
        /// read value as Int array
        /// </summary>
        /// <returns>read value</returns>
        private int[] ReadIntArray()
        {
            if (this.currentStr[currentIdx] != ARRAY_START)
            {
                return null;
            }
            int[] result = new int[this.CountArrayNum(this.currentIdx) ];
            ++ this.currentIdx;
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = ReadIntValue();
                this.SkipEmptyString();
                if (this.currentStr[this.currentIdx] != COMMNA)
                {
                    break;
                }
                ++this.currentIdx;
            }
            if (this.currentStr[currentIdx] == ARRAY_END)
            {
                ++currentIdx;
            }
            this.SkipEmptyString();
            return result;
        }

        /// <summary>
        /// read value as Long
        /// </summary>
        /// <returns>read value</returns>
        private long ReadLongValue()
        {
            long val = 0;
            this.SkipEmptyString();
            this.strBuilder.Length = 0;
            if (this.currentStr[currentIdx] == '-')
            {
                this.strBuilder.Append(this.currentStr[this.currentIdx]);
                ++this.currentIdx;
            }else if (this.currentStr[currentIdx] == '+')
            {
                this.strBuilder.Append(this.currentStr[this.currentIdx]);
                ++this.currentIdx;
            }
            this.SkipEmptyString();
            for (; ('0' <= this.currentStr[this.currentIdx]) && (this.currentStr[this.currentIdx] <= '9') || this.currentStr[this.currentIdx] == '.' ;
                ++this.currentIdx)
            {
                this.strBuilder.Append(currentStr[currentIdx]);
            }
            try
            {
                val = long.Parse(this.strBuilder.ToString());
            }
            catch (Exception e)
            {
            }
            return val;
        }

        /// <summary>
        /// read long array
        /// </summary>
        /// <returns>read value</returns>
        private long[] ReadLongArray()
        {
            if (this.currentStr[currentIdx] != ARRAY_START)
            {
                return null;
            }
            long[] result = new long[this.CountArrayNum(this.currentIdx)];
            ++this.currentIdx;
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = ReadLongValue();
                this.SkipEmptyString();
                if (this.currentStr[this.currentIdx] != COMMNA)
                {
                    break;
                }
                ++this.currentIdx;
            }
            if (this.currentStr[currentIdx] == ARRAY_END)
            {
                ++currentIdx;
            }
            this.SkipEmptyString();
            return result;
        }

        /// <summary>
        /// read value as a string
        /// </summary>
        /// <returns>read value</returns>
        private string ReadStringValue(){
            this.SkipEmptyString();
            this.strBuilder.Length = 0;
            if (this.currentStr[this.currentIdx] != STRING_SEPARATOR)
            {
                return strBuilder.ToString();
            }
            ++this.currentIdx;
            for (; this.currentIdx < this.currentStr.Length; ++this.currentIdx)
            {
                if (this.currentStr[this.currentIdx] == STRING_SEPARATOR)
                {
                    ++this.currentIdx;
                    break;
                }
                else if (this.currentStr[this.currentIdx] == STRING_BACKSLASH)
                {
                    ++this.currentIdx;
                    if (this.currentStr[this.currentIdx] == 'n')
                    {
                        this.strBuilder.Append( '\n' );
                    }
                    else if (this.currentStr[this.currentIdx] == 't')
                    {
                        this.strBuilder.Append( '\t' );
                    }else{
                        this.strBuilder.Append(this.currentStr[this.currentIdx]);
                    }
                    continue;
                }
                else{
                    this.strBuilder.Append(this.currentStr[this.currentIdx]);
                }
            }
            return this.strBuilder.ToString();
        }

        /// <summary>
        /// read string array
        /// </summary>
        /// <returns>read value</returns>
        private string[] ReadStringArray()
        {
            if (this.currentStr[currentIdx] != ARRAY_START)
            {
                return null;
            }
            string[] result = new string[this.CountArrayNum(this.currentIdx)];
            ++this.currentIdx;
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = this.ReadStringValue();
                this.SkipEmptyString();
                if (this.currentStr[this.currentIdx] != COMMNA)
                {
                    break;
                }
                ++this.currentIdx;
            }
            if (this.currentStr[currentIdx] == ARRAY_END)
            {
                ++currentIdx;
            }
            this.SkipEmptyString();
            return result;
        }


        /// <summary>
        /// skip empty string
        /// </summary>
        private void SkipEmptyString() 
        {
            this.currentIdx = this.GetNextNonEmptyIdx(this.currentIdx);
        }
        /// <summary>
        /// Get next non empty index
        /// </summary>
        /// <param name="idx">Index of string</param>
        /// <returns>not empty character Index</returns>
        private int GetNextNonEmptyIdx(int idx)
        {
            for (; idx < this.currentStr.Length; ++idx)
            {
                if (currentStr[idx] != ' ' && this.currentStr[idx] != '\t' &&
                    this.currentStr[idx] != '\n' && this.currentStr[idx] != '\r')
                {
                    break;
                }
            }
            return idx;
        }


        /// <summary>
        /// skip var value 
        /// </summary>
        private void SkipVarValue()
        {
            this.currentIdx = this.GetVarValueEndIndex(this.currentIdx);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int GetVarValueEndIndex(int idx)
        {
#if JSON_DEBUG
            int prevIdx = idx;
#endif
            idx = this.GetNextNonEmptyIdx(idx);
            if (this.currentStr[idx] == ARRAY_START)
            {
                idx = this.GetArrayValueEndIndex(idx);
            }
            else if (this.currentStr[idx] == OBJECT_START)
            {
                idx = this.GetObjectValueEndIndex(idx);
            }
            else if (this.currentStr[idx] == STRING_SEPARATOR)
            {
                idx = this.GetStringValueEndIndex(idx);
            }
            else
            {
                idx = this.GetNumberEndIndex(idx);
            }
#if JSON_DEBUG
            DebugPrint("GetVarValueEndIndex " , prevIdx , idx );
#endif
            return idx;
        }

        /// <summary>
        /// Get End index of array
        /// </summary>
        /// <param name="idx">start index</param>
        /// <returns>end index</returns>
        private int GetArrayValueEndIndex(int idx)
        {
            if (this.currentStr[idx] != ARRAY_START)
            {
                return idx;
            }
            ++idx;
            idx = this.GetNextNonEmptyIdx(idx);
            if (this.currentStr[idx] == ARRAY_END)
            {
                return idx + 1;
            }
            for (; idx < this.currentStr.Length; )
            {
                idx = this.GetVarValueEndIndex(idx);
                idx = this.GetNextNonEmptyIdx(idx);
                if (this.currentStr[idx] == COMMNA)
                {
                    ++idx;
                }
                else if (this.currentStr[idx] == ARRAY_END)
                {
                    ++idx;
                    break;
                }
                idx = this.GetNextNonEmptyIdx(idx);
            }
            return idx;
        }

        /// <summary>
        /// Get End index of object( TODO not yet)
        /// </summary>
        /// <param name="idx">start index</param>
        /// <returns>end index</returns>
        private int GetObjectValueEndIndex(int idx)
        {
            if (this.currentStr[idx] != OBJECT_START)
            {
                return idx;
            }
            ++idx;
            idx = this.GetNextNonEmptyIdx(idx);
            if (this.currentStr[idx] == OBJECT_END)
            {
                return idx + 1;
            }
            // object 
            for (; idx < this.currentStr.Length; ++idx)
            {
                // object name
                idx = this.GetNextNonEmptyIdx(idx);
                idx = GetObjectVarNameEndIndex(idx);
                idx = this.GetNextNonEmptyIdx(idx);
                // semicoron
                if (this.currentStr[idx] == ':' ) 
                {
                    ++idx;
                }
                idx = this.GetNextNonEmptyIdx(idx);
                // object value
                idx = this.GetVarValueEndIndex(idx);
                idx = this.GetNextNonEmptyIdx(idx);

                if (this.currentStr[idx] == OBJECT_END)
                {
                    ++idx;
                    break;
                }
            }
            return idx;
        }

        /// <summary>
        /// Get End index of object var name
        /// </summary>
        /// <param name="idx">start index</param>
        /// <returns>end index of object</returns>
        private int GetObjectVarNameEndIndex( int idx)
        {
            if ( this.currentStr[idx] == '"')
            {
                ++idx;
            }
            for (; idx < this.currentStr.Length; ++idx)
            {
                if (this.currentStr[idx] == ':' || this.currentStr[idx] == ':' ||
                    this.currentStr[idx] == '"' ||
                    this.currentStr[idx] == ' ' || this.currentStr[idx] == '\t')
                {
                    break;
                }
            }
            if (this.currentStr[idx] == '"')
            {
                ++idx;
            }
            return idx;
        }

        /// <summary>
        /// Get String Value index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int GetStringValueEndIndex(int idx)
        {
            if (this.currentStr[idx] != STRING_SEPARATOR) {
                return idx;
            }
            ++idx;
            for (; idx < this.currentStr.Length; ++idx) {
                if (this.currentStr[idx] == STRING_BACKSLASH)
                {
                    ++idx;
                    continue;
                }
                if (this.currentStr[idx] == STRING_SEPARATOR)
                {
                    ++idx;
                    break;
                }
            }
            return idx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int GetNumberEndIndex(int idx)
        {
            if (this.currentStr[idx] == '-')
            {
                ++idx;
            }else if (this.currentStr[idx] == '+')
            {
                this.strBuilder.Append(this.currentStr[idx]);
                ++idx;
            }
            idx = this.GetNextNonEmptyIdx(idx);
            for (; ('0' <= this.currentStr[idx]) && (this.currentStr[idx] <= '9') || this.currentStr[idx] == '.';
                ++idx)
            {
            }
            return idx;
        }


        /// <summary>
        /// Counting array num for allocation
        /// </summary>
        /// <param name="idx">index of string</param>
        /// <returns>array count num</returns>
        private int CountArrayNum( int idx)
        {
            if (this.currentStr[idx] != ARRAY_START ) {
                return 0;
            }
            int cnt = 0;
            int indentLevel = 1;
            ++idx;
            // if there's no letter return 0
            idx = this.GetNextNonEmptyIdx(idx);
            if ( this.currentStr[idx] == ARRAY_END) {
                return 0;
            }
            for (; idx < this.currentStr.Length; ++idx)
            {
                char currentChar = this.currentStr[idx];
                if (currentChar == ARRAY_START || currentChar == OBJECT_START) {
                    ++indentLevel;
                }
                else if (currentChar == ARRAY_END || currentChar == OBJECT_END) {
                    --indentLevel;
                    if (indentLevel <= 0) {
                        break;
                    }
                }
                else if (currentChar == STRING_SEPARATOR)
                {
                    idx = this.GetStringValueEndIndex(idx);
                }
                else if (indentLevel == 1 && currentChar == COMMNA) {
                    ++cnt;
                }
            }
            return cnt + 1;
        }

    }
}
