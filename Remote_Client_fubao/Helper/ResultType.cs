using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Helper
{
    public enum ResultType
    {
        USER_INFO = 1, //登录返回用户信息
        RCONNECT_INFO = 2, //重连失败返回信息
        NETWORK_ABNORMAL= 3, //网络异常
        Timeout = 4,//心跳超时
        OTHER_LOGOUT = 5 //对方已登出

    }
}
