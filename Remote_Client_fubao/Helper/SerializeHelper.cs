/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Helper
* 类名称：SerializeHelper
* 创建时间：2018/11/26 
* 创建人：zhangbaoj
* 创建说明：序列化帮助类
*****************************************************************************************************/
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Remote_Client_fubao.Helper
{
    public class SerializeHelper
    {
        /// <summary>
        ///     将C#数据实体转化为xml数据
        /// </summary>
        /// <param name="obj">要转化的数据实体</param>
        /// <returns>xml格式字符串</returns>
        public static string XmlSerialize<T>(T obj)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            stream.Position = 0;
            var sr = new StreamReader(stream);
            var resultStr = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return resultStr;
        }

        /// <summary>
        ///     将xml数据转化为C#数据实体
        /// </summary>
        /// <param name="json">符合xml格式的字符串</param>
        /// <returns>T类型的对象</returns>
        public static T XmlDeserialize<T>(string xml)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml.ToCharArray()));
            var obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
        /// <summary>
        ///     newton.json序列化,日志参数专用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        ///     newton.json反序列化,日志参数专用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        ///     将C#数据实体转化为JSON数据
        /// </summary>
        /// <param name="obj">要转化的数据实体</param>
        /// <returns>JSON格式字符串</returns>
        public static string JsonSerialize(object obj)
        {
            var ds = new DataContractJsonSerializer(obj.GetType());
            using (var ms = new MemoryStream())
            {
                ds.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        ///     将JSON数据转化为C#数据实体
        /// </summary>
        /// <param name="json">符合JSON格式的字符串</param>
        /// <returns>T类型的对象</returns>
        public static T JsonDeserialize<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                ms.Position = 0;
                var obj = (T)serializer.ReadObject(ms);
                return obj;
            }
        }

        /// <summary>
        ///     二进制序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ByteSerialize(object obj)
        {
            using (var m = new MemoryStream())
            {
                var bin = new BinaryFormatter();
                bin.Serialize(m, obj);
                return m.ToArray();
            }
        }

        /// <summary>
        ///     二进制反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T ByteDeserialize<T>(byte[] buffer)
        {
            using (var m = new MemoryStream())
            {
                m.Write(buffer, 0, buffer.Length);
                m.Position = 0;
                var bin = new BinaryFormatter();
                return (T)bin.Deserialize(m);
            }
        }

        /// <summary>
        ///     ProtolBuf序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static byte[] ProtolBufSerialize<T>(T instance)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, instance);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     ProtolBuf反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static T ProtolBufDeserialize<T>(byte[] buffer)
        {
            try
            {
                using (var ms = new MemoryStream(buffer))
                {
                    ms.Position = 0;
                    var t = Serializer.Deserialize<T>(ms);
                    Array.Clear(buffer, 0, buffer.Length);
                    return t;
                }
            }
            catch
            {
                Array.Clear(buffer, 0, buffer.Length);
                return default(T);
            }
        }
    }
}
