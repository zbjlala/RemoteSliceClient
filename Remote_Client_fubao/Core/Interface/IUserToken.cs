/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Core.Interface
* 类名称：IUserToken
* 创建时间：2018/11/26
* 创建人：zhangbaoj
* 创建说明：UserToken 接口
*****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Remote_Client_fubao.Core.Interface
{
    public interface IUserToken
    {
        string UID
        {
            get; set;
        }

        string LastID
        {
            get; set;
        }
        SocketAsyncEventArgs ReceiveArgs
        {
            get; set;
        }

        Socket ConnectSocket
        {
            get;
            set;
        }

        int MaxBufferSize
        {
            get;
        }
        DateTime ConnectDateTime
        {
            get; set;
        }
        DateTime ActiveDateTime
        {
            get; set;
        }

        void UnPackage(byte[] receiveData, Action<byte[]> action);
    }
}
