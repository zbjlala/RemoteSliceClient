/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Core.Tcp
* 类名称：ClientSocket
* 创建时间：2018/11/26
* 创建人：zhangbaoj
* 创建说明：  客户端socket操作
*****************************************************************************************************/
using Remote_Client_fubao.Core.Handler;
using Remote_Client_fubao.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;

namespace Remote_Client_fubao.Core.Tcp
{
    /// <summary>
    ///     客户端socket操作
    /// </summary>
    public class ClientSocket
    {
        private readonly int _bufferSize;

        private readonly int _timeOut;

        public Socket clientSocket;

        private object lockObj = new object();

        public SocketAsyncEventArgs ReceiveSocket;

        Type _userTokenType;

        public string IP;

        public int Port;

        //private System.Timers.Timer timer;

        /// <summary>
        /// 重连次数
        /// </summary>
        private int connectetimes = 0;


        public ClientSocket(Type userTokenType, int bufferSize, int timeOut, string lastID, string uid = "")
        {
            _userTokenType = userTokenType;
            _bufferSize = bufferSize * 100;
            _timeOut = timeOut;
            if (string.IsNullOrEmpty(uid))
                UID = Guid.NewGuid().ToString("N");
            else
                UID = uid;
            LastID = lastID;
            //timer = new System.Timers.Timer(1000);
            //timer.AutoReset = true;
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(ReConnected);
        }

        public string UID
        {
            get; set;
        }

        public string LastID
        {
            get; set;
        }

        public bool IsConnected
        {
            get;
            set;
        }

        public bool Connect(string ip = "127.0.0.1", int port = 6666)
        {
            if(clientSocket != null)
            {
                clientSocket.Close();
                clientSocket = null;
            }
            this.IP = ip;
            this.Port = port;
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Blocking = false;
            clientSocket.SendTimeout = clientSocket.ReceiveTimeout = _timeOut;
            IPAddress iPAddress;
            if (IsIP(ip))
            {
                iPAddress = IPAddress.Parse(ip);
            }
            else
            {
                IPHostEntry host = Dns.GetHostEntry(ip);
                iPAddress = host.AddressList[0];
            }
            var connectsocketAsync = new SocketAsyncEventArgs();
            connectsocketAsync.Completed += ConnectsocketAsync_Completed;
            connectsocketAsync.RemoteEndPoint = new IPEndPoint(iPAddress, port);
            if (!clientSocket.ConnectAsync(connectsocketAsync))
                Connected(connectsocketAsync);
            Thread.Sleep(2000);
            if (!IsConnected)
            {
                return false;
            }
            return true;
        }

        public void ReConnected(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.clientSocket = null;
            this.Connect(this.IP,this.Port);
            this.connectetimes++;
            RaiseOnMsg(String.Format("正在重新连接远程服务器，重连次数{0}",connectetimes));
        }

        private void ConnectsocketAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            Connected(e);
        }

        private void Connected(SocketAsyncEventArgs connectsocketAsync)
        {
            if (connectsocketAsync.SocketError == SocketError.Success)
            {
                ReceiveSocket = new SocketAsyncEventArgs();
                var buffer = new byte[_bufferSize];
                ReceiveSocket.SetBuffer(buffer, 0, _bufferSize);
                ReceiveSocket.Completed += SocketIO_Completed;
                ReceiveSocket.AcceptSocket = connectsocketAsync.ConnectSocket;

                var userToken = (IUserToken)Activator.CreateInstance(_userTokenType);
                userToken.ReceiveArgs = ReceiveSocket;
                userToken.UID = UID;
                userToken.LastID = LastID;
                userToken.ConnectSocket = ReceiveSocket.AcceptSocket;
                userToken.ActiveDateTime = DateTime.Now;
                userToken.ConnectDateTime = DateTime.Now;

                ReceiveSocket.UserToken = userToken;
                if (!connectsocketAsync.ConnectSocket.ReceiveAsync(ReceiveSocket))
                    Received(ReceiveSocket);
                IsConnected = true;
                RaiseOnConnected();
            }
        }

        public void DisConnect()
        {
            if (ReceiveSocket != null)
            {
                ReceiveSocket.Dispose();
            }
            if (clientSocket != null)
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            IsConnected = false;
        }

        private void SocketIO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // if (e.SocketError == SocketError.Success)
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    Received(e);
                    break;
                case SocketAsyncOperation.Send:
                    Sended(e);
                    break;
            }
        }

        private void Received(SocketAsyncEventArgs e)
        {
            try
            {
                if ((e.SocketError == SocketError.Success) && e.AcceptSocket.Connected)
                {
                    var userToken = (IUserToken)e.UserToken;
                    var buffer = new byte[e.BytesTransferred];
                    Buffer.BlockCopy(userToken.ReceiveArgs.Buffer, e.Offset, buffer, 0, buffer.Length);
                    OnData?.Invoke(this, buffer);
                    // OnData(this, buffer);
                    //Array.Clear(buffer, 0, buffer.Length);
                    buffer = null;
                    if (clientSocket.Connected)
                        if (!clientSocket.ReceiveAsync(ReceiveSocket))
                            Received(ReceiveSocket);
                }
                else
                {
                    RaiseOnError(new SocketException((int)SocketError.Shutdown), "已断开服务连接");
                    RaiseOnMsg("已断开服务连接166");
                }
            }
            catch (Exception ex)
            {
                RaiseOnError(ex, "ClientSocketOperation.Received");
                RaiseOnMsg("未将对象设置为引用172" + ex.ToString() + clientSocket.Connected);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        public void SendAsync(byte[] data)
        {
            if ((clientSocket != null) && clientSocket.Connected && (data != null))
            {
                var sendSocket = new SocketAsyncEventArgs();
                sendSocket.SetBuffer(data, 0, data.Length);
                if (!clientSocket.SendAsync(sendSocket))
                    Sended(sendSocket);
             //   Array.Clear(data, 0, data.Length);
            }
            else
            {
                RaiseOnError(new SocketException((int)SocketError.Shutdown), "发送消息失败");
                RaiseOnMsg("发送消息失败192" + clientSocket.Connected.ToString());
                this.IsConnected = false;
                //timer.Start();
                //while(clientSocket.Connected || connectetimes > 5)
                //{
                //    timer.Stop();
                //    connectetimes = 0;
                //}
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendAsync(string msg)
        {
            this.SendAsync(System.Text.Encoding.UTF8.GetBytes(msg));
        }


        private void Sended(SocketAsyncEventArgs sendSocket)
        {
            sendSocket.Dispose();
        }

        #region event

        public virtual event OnConnectedHandler OnConnected;

        protected virtual void RaiseOnConnected()
        {
            OnConnected?.Invoke(this);
        }

        public virtual event OnCleintDataReceivedHandler OnData;

        public event OnClientErrorHandler OnError;

        public void RaiseOnError(Exception ex, string msg, bool isConnected = true)
        {
            IsConnected = isConnected;
            if ((OnError != null) && !string.IsNullOrEmpty(msg))
                OnError(ex, msg + Environment.NewLine + ex.Message + Environment.NewLine + ex.Source + ex.StackTrace);
        }
        /// <summary>
        /// 消息处理
        /// </summary>
        public event OnClientMsgHandler OnMsg;

        public void RaiseOnMsg(string msg)
        {
            if ((OnMsg != null) && !string.IsNullOrEmpty(msg))
                OnMsg(DateTime.Now.ToLongTimeString() + "\r\n" + msg);
        }

        #endregion

        #region 检查是否为IP地址
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        #endregion
    }
}
