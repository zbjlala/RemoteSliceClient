/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Core.Handler
* 类名称：Message
* 创建时间：2018/11/26 
* 创建人：zhangbaoj
* 创建说明：传输信息接口
*****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Model.Interface
{
    public interface IMessage
    {
        string Accepter { get; set; }

        byte Protocal { get; set; }

        byte[] Data { get; set; }

        string Sender { get; set; }

        long SendTick { get; set; }
    }
}
