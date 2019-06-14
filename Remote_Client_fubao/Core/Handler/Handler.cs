/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Core.Handler
* 类名称：Handler
* 创建时间：2018/11/26 
* 创建人：zhangbaoj
* 创建说明：委托类
*****************************************************************************************************/
using Remote_Client_fubao.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Core.Handler
{
    #region client

    public delegate void OnConnectedHandler(object sender);

    public delegate void OnCleintMessageReceivedHandler(object sender, Message msg);

    public delegate void OnCleintFileReceivedHandler(object sender,byte[] file,string type);

    public delegate void OnCleintDataReceivedHandler(object sender, byte[] data);

    public delegate void OnClientErrorHandler(Exception ex, string msg);
    /// <summary>
    /// 日志消息
    /// </summary>
    /// <param name="msg"></param>
    public delegate void OnClientMsgHandler(string msg);
    #endregion
}
