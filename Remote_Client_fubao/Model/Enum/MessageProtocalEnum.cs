/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Model.Enum
* 类名称：Message
* 创建时间：2018/11/26 
* 创建人：zhangbaoj
* 创建说明：传输消息类型枚举
*****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Model.Enum
{
    public enum MessageProtocalEnum
    {
        Heart = 0,
        Login = 1,
        Logout = 2,
        Subscribe = 3,
        Unsubscribe = 4,
        PrivateMsg = 5,
        Message = 6,
        File = 7,
        RemoteConnect = 8,
        RemoteStart = 9,
        FileSlice = 10,
        RLogin = 101,
        RSubscribe = 103
    }
}
