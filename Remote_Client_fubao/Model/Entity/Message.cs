/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Core.Handler
* 类名称：Message
* 创建时间：2018/11/26 
* 创建人：zhangbaoj
* 创建说明：传输信息
*****************************************************************************************************/
using ProtoBuf;
using Remote_Client_fubao.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Remote_Client_fubao.Model.Entity
{
    [DataContract]
    [ProtoContract]
    public class Message : IMessage, IDisposable
    {
        /// <summary>
        ///     频道ID或私信ID
        /// </summary>
        [DataMember]
        [ProtoMember(1, IsRequired = false)]
        public string Accepter
        {
            get; set;
        }

        [DataMember]
        [ProtoMember(2, IsRequired = true)]
        public byte Protocal
        {
            get; set;
        }

        [DataMember]
        [ProtoMember(3, IsRequired = true)]
        public byte[] Data
        {
            get; set;
        }

        /// <summary>
        ///     发送者id
        /// </summary>
        [DataMember]
        [ProtoMember(4, IsRequired = true)]
        public string Sender
        {
            get; set;
        }

        /// <summary>
        ///     发送时间
        /// </summary>
        [DataMember]
        [ProtoMember(5, IsRequired = true)]
        public long SendTick
        {
            get; set;
        }

        [DataMember]
        [ProtoMember(6, IsRequired = true)]

        public string LastID
        {
            get; set;
        }

        public void Dispose()
        {
            this.Accepter = null;
            this.Protocal = 0;
            if (Data != null)
                Array.Clear(this.Data, 0, this.Data.Length);
            this.Sender = null;
            this.SendTick = 0;
        }
    }
}
