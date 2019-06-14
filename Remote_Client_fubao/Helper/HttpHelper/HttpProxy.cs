/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Helper.HttpHelper
* 类名称：HttpProxy
* 创建时间：2018/11/27 
* 创建人：zhangbaoj
* 创建说明：Http代理类
*****************************************************************************************************/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Remote_Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Helper.HttpHelper
{
    public class HttpProxy
    {
        HttpHelper1 httpHelper = new HttpHelper1();
        HttpItem httpItem;

        /// <summary>
        /// 通常接口访问类
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="parameter">参数键值对</param>
        /// <param name="isOrder">是否排序</param>
        /// <param name="method">请求方式（GET POST）</param>
        /// <param name="dataType">参数类型（DICTIONARY,JSON）</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public string GetRequestCommon(string url, IDictionary<string, string> parameter, bool isOrder = true,bool isAESC = false, string method = "POST", DataType dataType = DataType.DICTIONARY, string contentType = "application/x-www-form-urlencoded")
        {
            string test = SerializeHelper.Serialize(parameter);
            if (isAESC)
            {
                httpItem = new HttpItem()
                {
                    URL = url,
                    Method = method.ToUpper(),
                    Encoding = System.Text.Encoding.GetEncoding("utf-8"),
                    Accept = "*/*",
                    ContentType = ContentType.APP_LOGININFO,
                    Postdata = GetParam(LoginAES.AesEncrypt(SerializeHelper.Serialize(parameter), "ServiceWelcome88"))
                };
           }
            else
            {
                httpItem = new HttpItem()
                {
                    URL = url,
                    Method = method.ToUpper(),
                    Encoding = System.Text.Encoding.GetEncoding("utf-8"),
                    Accept = "*/*",
                    ContentType = ContentType.APP_LOGININFO,
                    Postdata = GetParameter(parameter, isOrder, dataType)
                };
            }
            HttpResult httpResult = httpHelper.GetHtml(httpItem);

            if (httpResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return "";
            }
            else
            {
                //JObject jsondata = JObject.Parse(httpResult.Html);
                //if (jsondata["success"].ToString() == "false")
                //{
                //    return "false";
                //}
            }
            return httpResult.Html;
        }
        public string GetRequestLogin(string url, IDictionary<string, string> parameter, bool isOrder = true, string method = "POST", DataType dataType = DataType.DICTIONARY, string contentType = "application/x-www-form-urlencoded")
        {
            string _postdata = GetParameter(parameter, isOrder, DataType.JSON);

            httpItem = new HttpItem()
            {
                URL = url,
                Encoding = System.Text.Encoding.GetEncoding("utf-8"),
                Method = method.ToUpper(),
                ContentType = ContentType.APP_TEXT,
                Accept = "*/*",
                Postdata =
                "{ \"toolsdata\":\"6tkUnMqRxrqTkpfCFAxM7BZ6GN96KqCnuhJ5s74qGkjaaBRb1r4CB0xmXR10QC1J/ZQuoyzSRdiCpSiMm9WUo7ylMRetVn+DummePfqGxvJLpx3cHJ3t6wJIuSookwCivotaEJhRVptGTK/iFnKToyAq6DcpGQJOjfwxpN+Zfx7aO9zCRj/wxLBMUGoJJqvqM+nRH50vqf1ubl+EzsgGxZm0LBedllWscvFerP/MbqDWpBaTskg6bqvw6A31M2Gi\"}"

                //Postdata = "{ \"toolsdata\":\"" + LoginAES.AesEncrypt(_postdata, "YbcLbhWelcome158") + "\"}"
            };
            HttpResult httpResult = httpHelper.GetHtml(httpItem);
            return httpResult.Html;
        }

        private string GetParameter(IDictionary<string, string> parameter, bool isOrder, DataType dataType)
        {
            string result;
            parameter = parameter.OrderBy(key => key.Key).ToDictionary(keyItem => keyItem.Key, valueItem => valueItem.Value);
            if (dataType == DataType.JSON)
            {
                result = JsonConvert.SerializeObject(parameter);
            }
            else
            {
                var sbPara = new StringBuilder(1024);
                foreach (var para in parameter.Where(para => !String.IsNullOrWhiteSpace(para.Value)))
                //para.Value.IsNullOrWhiteSpace()))
                {
                    sbPara.AppendFormat("{0}={1}&", para.Key, para.Value);
                }


                result = sbPara.ToString().TrimEnd('&');
            }
            return result;
        }

        private string GetParam(string aes)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("params",aes);
            return SerializeHelper.Serialize(param);
        }
    }

    public static class ContentType
    {
        public static readonly string APP_LOGIN = "application/x-www-form-urlencoded; charset=utf-8";
        public static readonly string APP_LOGININFO = "application/x-www-form-urlencoded";

        public static readonly string APP_TEXT = "text/html";

    }
    /// <summary>
    /// 接口传参数据类型
    /// </summary>
    public enum DataType
    {
        JSON = 1,
        DICTIONARY = 2
    }
    /// <summary>
    /// 登录信息加密
    /// </summary>
    public static class LoginAES
    {
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7,
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }


    }
    public static class NewtonJsonHelper
    {
        public static IDictionary<string, string> GetDictionByJson(string json)
        {
            IDictionary<string, string> keys = new Dictionary<string, string>();
            try
            {
                keys = JsonConvert.DeserializeObject<IDictionary<string, string>>(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return keys;
        }
    }
}
