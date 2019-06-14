/*****************************************************************************************************
* 项目名称：CompressHelper
* 命名空间：Remote_Client_fubao.Helper
* 类名称：CompressHelper
* 创建时间：2018/11/27 
* 创建人：zhangbaoj
* 创建说明：压缩处理类
*****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Helper
{
    public static class CompressHelper
    {
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close(); //很重要，必须关闭，否则无法正确解压
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Decompress(byte[] inputBytes)
        {

            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }

            }
        }
    }
}
