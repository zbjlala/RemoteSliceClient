/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Remote
* 类名称：ClientConfig
* 创建时间：2018/11/26 
* 创建人：zhangbaoj
* 创建说明：客户端配置信息类
*****************************************************************************************************/
using Remote_Client_fubao.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Remote
{
    /// <summary>
    ///     客户端配置类
    /// </summary>
    public class ClientConfig
    {
        /// <summary>
        ///     服务器监听IP
        /// </summary>
        public string IP { get; set; } = @"ws://127.0.0.1:8080/Remote/";

        /// <summary>
        ///     服务器监听端口
        /// </summary>
        public int Port { get; set; } = 31219;

        /// <summary>
        /// http服务器端口
        /// </summary>
        public int HttpPort { get; set; } = 8080; //http服务器端口


        public string FileUrl { get; set; }

        /// <summary>
        ///     解析命令初始缓存大小
        /// </summary>
        public int InitBufferSize { get; set; } = 1024 * 1; //解析命令初始缓存大小  

        /// <summary>
        ///     Socket超时设置为60秒
        /// </summary>
        public int SocketTimeOutMS { get; set; } = 60 * 1000; //Socket超时设置为60秒


        public static ClientConfig Instance()
        {
            var config = new ClientConfig();
            try
            {
                var json = File.ReadAllText(Environment.CurrentDirectory + "\\config.json");
                return SerializeHelper.JsonDeserialize<ClientConfig>(json);
            }
            catch (FileNotFoundException fex)
            {
                Save(config);
            }
            return config;
        }

        public static void Save(ClientConfig config)
        {
            var json = SerializeHelper.JsonSerialize(config);
            File.WriteAllText(Environment.CurrentDirectory + "\\config.json", json);
        }
    }
}
