using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Helper.HttpHelper
{
    /// <summary>
    /// 接口URL
    /// </summary>
    public static class InterfaceUrl
    {
        //登录获取usertoken
        public static readonly string REMOTE_LOGIN = "https://service.chanjet.com/api/tools-login/tools-login";
        //获取lastID
        public static readonly string REMOTE_LASTID = "https://service.chanjet.com/robot/api/check/fubao-info";
        //客户端登录日志
        public static readonly string LOGIN_LOG = "https://inte-service.chanjet.com/robot/api/remote/remote-login";
        //工单提交
        public static readonly string SUBMIT_ORDER = "http://inte-service.chanjet.com/backend/api/remote/save-remote-order";
    }
}
