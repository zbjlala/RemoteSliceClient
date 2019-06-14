/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Remote.MS
* 类名称：MessageClient
* 创建时间：2018/11/26 
* 创建人：zhangbaoj
* 创建说明：客户端类
*****************************************************************************************************/
using Remote_Client_fubao.Core.Handler;
using Remote_Client_fubao.Core.Tcp;
using Remote_Client_fubao.Helper;
using Remote_Client_fubao.Model.Entity;
using Remote_Client_fubao.Model.Enum;
using Remote_Client_fubao.Remote.MS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Remote_Client_fubao.Remote.MS
{
    /// <summary>
    ///     iocp客户端
    /// </summary>
    public class MessageClient 
    {
        private static readonly ClientConfig clientConfig = ClientConfig.Instance();

        public ReConnectFaildHanlder ReConnectFaild;

        string UID;

        public  bool isCloseing = false;

        public bool isConnecting = false;

        bool isLogin = false;

        public System.Timers.Timer timer;

        int reConnectedNum = 0;

        string _ip;
        /// <summary>
        /// 加密锁号
        /// </summary>
        private string Dgcode;

        WebSocket ws;


        /// <summary>
        ///     iocp客户端
        /// </summary>
        public MessageClient(string uid = "",string dgcode="")
        {
            string testip = clientConfig.IP;
            // _ip = "ws://10.11.72.40:8080/Remote/";
            // _ip = "ws://moni-remote.chanapp.chanjet.com/Remote";
            _ip = "ws://remote.chanapp.chanjet.com/Remote";
            // _ip = "ws://10.1.145.106:8080/Remote";
           //_ip = "ws://inte-remote.chanapp.chanjet.com/Remote";


            _ip = _ip + "?name="+uid.ToString();
            this.UID = uid;
            this.Dgcode = dgcode;
            timer = new System.Timers.Timer(5000);
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(CheckConnect);

        }

        public bool Connect()
        {
            ws = new WebSocket(_ip);
            WsInite();
            ws.Connect();
            if(ws.ReadyState == WebSocketState.Connecting || ws.ReadyState == WebSocketState.Open)
            {

                isConnecting = true;
                HeartAsync();
                timer.Start();
                return true;
            }
            return false;
        }

        public void CheckConnect(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ActiveTime.AddSeconds(20) <= DateTime.Now)
            {
                    ReConnect();
            }
        }

        public void WsInite()
        {
            ws.OnMessage += (sender, e) =>
             {
                 if (e.Data != null)
                 {
                     try
                     {
                         Message msg = SerializeHelper.Deserialize<Message>(e.Data);
                         if (msg != null)
                         {
                             switch(msg.Protocal)
                             {
                                 case (byte)MessageProtocalEnum.Heart:
                                     ActiveTime = DateTime.Now;
                                     //OnNotice?.Invoke(new Message
                                     //{
                                     //    Data = Encoding.UTF8.GetBytes("心跳："+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"))
                                     //});
                                     break;
                                 case (byte)MessageProtocalEnum.RLogin:
                                     this.OnLogined?.Invoke(this, Encoding.UTF8.GetString(msg.Data));
                                     isLogin = true;
                                     OnNotice?.Invoke(msg);
                                     break;
                                 case (byte)MessageProtocalEnum.RemoteConnect:
                                     //var aa = Encoding.UTF8.GetString(msg.Data);
                                     OnNotice?.Invoke(msg);
                                     break;
                                 default:
                                     OnMessage?.Invoke(this, msg);
                                     break;
                             }

                         }
                     }
                     catch
                     {

                     }
                 }
                 else
                 {
                     Message msgFile = SerializeHelper.ProtolBufDeserialize<Message>(e.RawData);
                     if (msgFile.Protocal == (byte)MessageProtocalEnum.File)
                     {
                         OnFile?.Invoke(this, msgFile.Data,"1");
                     }
                     if (msgFile.Protocal == (byte)MessageProtocalEnum.FileSlice)
                     {
                         OnFile?.Invoke(this, msgFile.Data, "2");
                     }
                     OnNotice?.Invoke(new Message {
                         Data = Encoding.UTF8.GetBytes("接收到图片")
                     });
                 }
             };

            ws.OnError += (sender, e) =>
            {
            };
            ws.OnClose += (sender, e) =>
            {
               
            };
           // ws.OnOpen += (sender, e) => ws.Send("name:Hi, there!");
        }
      

        #region properties

        public static DateTime ActiveTime
        {
            get;
            private set;
        } = DateTime.Now;

        public string Url
        {
            get; set;
        }


        #endregion

        #region event

        public event OnLoginedHandler OnLogined;

        public event OnSubedHandler OnSubed;

        public event OnClientErrorHandler OnError;

        public event OnCleintMessageReceivedHandler OnMessage;

        public event OnCleintFileReceivedHandler OnFile;

        public event OnServiceNotice OnNotice;
        /// <summary>
        /// 消息处理
        /// </summary>
        public event OnClientMsgHandler OnMsg;

        #endregion

        #region 私有方法       
        protected void HeartAsync()
        {
            ws.Send(SerializeHelper.Serialize(new Message {
                Accepter = this.UID,
                Protocal = (byte)MessageProtocalEnum.Login,
                Sender = this.UID
            }));
            Task.Factory.StartNew(() =>
            {
                while (isConnecting)
                {
                    if (ActiveTime.AddSeconds(5) <= DateTime.Now)
                    {

                            if (ws.ReadyState == WebSocketState.Open)
                            {
                                ws.Send(SerializeHelper.Serialize(new Message
                                {
                                    Sender = this.UID,
                                    Protocal = (byte)MessageProtocalEnum.Heart,
                                    Accepter = this.UID
                                }));
                            }
                            else
                            {
                                //ReConnect();
                            }

                     }
                    Thread.Sleep(100);
                }
                
            
            });
        }
        #endregion


    
        public void SendRemoteConnect(string sender, string accepter, bool remoteConnect)
        {
            string msg;
            if (remoteConnect)
            {
                msg = Dgcode;
            }
            else
            {
                msg = VerbalInfo.REFUSE_REMOTE;
            }
            Message message = new Message
            {
                Accepter = accepter,
                Protocal = (byte)MessageProtocalEnum.RemoteStart,
                Data = Encoding.UTF8.GetBytes(msg),
                Sender = sender
            };
            ws.Send(SerializeHelper.Serialize(message));
            //ActiveTime = DateTime.Now;
        }
        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="remoteID"></param>
        /// <param name="buffer"></param>

        public void SendFile(string remoteID, byte[] buffer)
        {

            try
            {
                if (ws.ReadyState == WebSocketState.Open)
                {
                    Message msg = new Message
                    {
                        Sender = this.UID,
                        Protocal = (byte)MessageProtocalEnum.File,
                        Data = buffer,
                        Accepter = remoteID
                    };
                    ws.Send(SerializeHelper.ProtolBufSerialize(msg));
                }
                else
                {
                    ReConnect();
                }
            }
            catch
            {

            }
            

        }
       /// <summary>
       /// 发送图片切片
       /// </summary>
       /// <param name="remoteID"></param>
       /// <param name="buffer"></param>
        public void SendFileSlice(string remoteID, byte[] buffer)
        {

            try
            {
                if (ws.ReadyState == WebSocketState.Open)
                {
                    Message msg = new Message
                    {
                        Sender = this.UID,
                        Protocal = (byte)MessageProtocalEnum.FileSlice,
                        Data = buffer,
                        Accepter = remoteID
                    };
                    ws.Send(SerializeHelper.ProtolBufSerialize(msg));
                }
                else
                {
                    ReConnect();
                }
            }
            catch
            {

            }


        }

        public void SendMessage(string remoteID, string text)
        {
            try
            {
                if (ws.ReadyState == WebSocketState.Open)
                {
                    Message msg = new Message
                    {
                        Sender = this.UID,
                        Protocal = (byte)MessageProtocalEnum.PrivateMsg,
                        Data = Encoding.UTF8.GetBytes(text),
                        Accepter = remoteID
                    };
                    ws.Send(SerializeHelper.Serialize(msg));
                }
                else
                {
                    ReConnect();
                }
            }
            catch
            {

            }
        }
        public void SendLogout(string remoteID, string text)
        {
            Message msg = new Message
            {
                Sender = this.UID,
                Protocal = (byte)MessageProtocalEnum.Logout,
                Data = Encoding.UTF8.GetBytes(text),
                Accepter = remoteID
            };
            ws.Send(SerializeHelper.Serialize(msg));
        }
        public void Stop()
        {
            ws.Close();
        }

        int _reConnectNum = 0;
        Semaphore samphore = new Semaphore(1,1);
        public void ReConnect()
        {
            try
            {
                if(ws != null)
                {

                        ws.Close();
                        ws = null;
                        this.Connect();

                }
                else
                {
                    this.Connect();
                }
            }
            catch
            {

            }
            //if (ws.ReadyState != WebSocketState.Open)
            //{
            //    if (ws != null)
            //    {
            //        ws.Close();
            //        ws = null;
            //    }
            //    this.Connect();
            //}
        
        }


        public string JsonResultCommon(ResultType type, string result)
        {
            IDictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("type", type.ToString());
            valuePairs.Add("result", result);
            return SerializeHelper.Serialize(valuePairs);
        }
    }
}
