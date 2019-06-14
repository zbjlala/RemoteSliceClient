/*****************************************************************************************************
* 项目名称：Remote_Client_fubao
* 命名空间：Remote_Client_fubao.Remote.MS.Model
* 类名称：UserToken
* 创建时间：2018/11/26 
* 创建人：zhangbaoj
* 创建说明：  Server Socket操作类
*****************************************************************************************************/
using Remote_Client_fubao.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Remote_Client_fubao.Remote.MS.Model
{
    public class UserToken : IUserToken
    {
        private Socket connectSocket;

        /// <summary>
        ///     用户唯一ID
        /// </summary>
        public string UID { get; set; } = string.Empty;

        /// <summary>
        /// lastID
        /// </summary>
        public string LastID { get; set; }

        /// <summary>
        ///     用于接收的SocketAsyncEventArgs
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; } = new SocketAsyncEventArgs();

        public Socket ConnectSocket
        {
            get
            {
                return connectSocket;
            }
            set
            {
                connectSocket = value;
                ReceiveArgs.AcceptSocket = connectSocket;
            }
        }

        private volatile int offset, count = 0;

        private volatile byte[] _myBuffer;

        private volatile byte[] _myLenBuffer;

        public int MaxBufferSize
        {
            get; private set;
        }

        /// <summary>
        /// 会话验证码
        /// </summary>
        public int Auth
        {
            get; set;
        }

        public UserToken()
        {

        }

        public UserToken(int maxBufferSize)
        {
            this.MaxBufferSize = maxBufferSize;
        }

        public virtual void UnPackage(byte[] receiveData, Action<byte[]> action)
        {
            //当前包取内容的
            if (offset == 0)
            {
                var packageLength = 0;
                if (this._myLenBuffer != null) //长度不完整的（包头不完整的）
                {
                    //调整receiveData包内容
                    var nData = new byte[this._myLenBuffer.Length + receiveData.Length];
                    Buffer.BlockCopy(this._myLenBuffer, 0, nData, 0, this._myLenBuffer.Length);
                    Buffer.BlockCopy(receiveData, 0, nData, this._myLenBuffer.Length, receiveData.Length);
                    receiveData = nData;
                    nData = null;
                    this._myLenBuffer = null;
                }
                else //全新包（包头完整的）
                {
                    packageLength = TcpPackage.GetLength(receiveData);
                    if (packageLength == 0)
                        return;
                }
                if (packageLength < receiveData.Length)
                {
                    var package = TcpPackage.Parse(receiveData);
                    if (action != null && package != null)
                    {
                        action(package.Content);

                        var slen = TcpPackage.GetLength(receiveData, package.Length);
                        if (slen >= 9)
                        {
                            var next = new byte[receiveData.Length - package.Length];
                            Buffer.BlockCopy(receiveData, package.Length, next, 0, next.Length);
                            this.UnPackage(next, action);
                        }
                    }
                }
                else if (packageLength == receiveData.Length)
                {
                    var package = TcpPackage.Parse(receiveData);
                    if (action != null && package != null)
                    {
                        action(package.Content);
                    }
                }
                else if (packageLength > receiveData.Length)
                {
                    this.count = packageLength;
                    this._myBuffer = new byte[packageLength];
                    Buffer.BlockCopy(receiveData, 0, this._myBuffer, 0, receiveData.Length);
                    this.offset = receiveData.Length;
                }
                Array.Clear(receiveData, 0, receiveData.Length);
                receiveData = null;
            }
            else //跨包取内容的
            {
                if (receiveData.Length + offset < count) //包内容超出
                {
                    Buffer.BlockCopy(receiveData, 0, this._myBuffer, offset, receiveData.Length);
                    offset += receiveData.Length;
                }
                else if (receiveData.Length + offset >= count) //包内容短的
                {
                    var packageLast = count - offset;
                    Buffer.BlockCopy(receiveData, 0, this._myBuffer, offset, packageLast);
                    var package = TcpPackage.Parse(this._myBuffer);
                    if (action != null && package != null)
                    {
                        //action(this._myBuffer);
                        action(package.Content);
                    }
                    Array.Clear(this._myBuffer, 0, this._myBuffer.Length);
                    this._myBuffer = null;
                    count = offset = 0;
                    var receiveLast = receiveData.Length - packageLast;
                    if (receiveLast >= 4)//包含包头长度
                    {
                        var packageLength = TcpPackage.GetLength(receiveData, packageLast);
                        if (packageLength > 0)
                        {
                            if (receiveLast > packageLength)
                            {
                                var nextData = new byte[receiveLast];
                                Buffer.BlockCopy(receiveData, packageLast, nextData, 0, receiveLast);
                                this.UnPackage(nextData, action);
                            }
                            else
                            {
                                this._myBuffer = new byte[packageLength];
                                Buffer.BlockCopy(receiveData, packageLast, this._myBuffer, 0, receiveLast);
                                offset = receiveLast;
                                count = packageLength;
                            }
                        }
                        else
                        {
                            Array.Clear(this._myBuffer, 0, this._myBuffer.Length);
                            this._myBuffer = null;
                            count = offset = 0;
                            Array.Clear(this._myLenBuffer, 0, this._myLenBuffer.Length);
                            this._myLenBuffer = null;
                        }
                    }
                    else if (receiveLast > 0)//不包含包头长度
                    {
                        this._myLenBuffer = new byte[receiveLast];
                        Buffer.BlockCopy(receiveData, packageLast, this._myLenBuffer, 0, receiveLast);
                        if (TcpPackage.GetLength(this._myLenBuffer) == 0)
                        {
                            Array.Clear(this._myLenBuffer, 0, this._myLenBuffer.Length);
                            this._myLenBuffer = null;
                        }
                        Array.Clear(this._myBuffer, 0, this._myBuffer.Length);
                        this._myBuffer = null;
                        count = offset = 0;
                    }
                }
                receiveData = null;
            }
        }
        object obj = new object();
        /// <summary>
        /// 处理收取数据
        /// 解包
        /// </summary>
        /// <param name="receiveData"></param>
        /// <param name="action"></param>
        public virtual void UnPackage(byte[] receiveData, Action<TcpPackage> action)
        {
            //semaphore.WaitOne();
            //当前包取内容的
            if (offset == 0)
            {

                var packageLength = 0;

                if (this._myLenBuffer != null) //长度不完整的（包头不完整的）
                {
                    //调整receiveData包内容
                    var nData = new byte[this._myLenBuffer.Length + receiveData.Length];
                    Buffer.BlockCopy(this._myLenBuffer, 0, nData, 0, this._myLenBuffer.Length);
                    Buffer.BlockCopy(receiveData, 0, nData, this._myLenBuffer.Length, receiveData.Length);
                    receiveData = nData;
                    nData = null;
                    this._myLenBuffer = null;
                }
                else //全新包（包头完整的）
                {

                    packageLength = TcpPackage.GetLength(receiveData);

                    if (packageLength == 0)
                    {
                        action(null);
                        return;
                    }
                    if (packageLength < 0 || packageLength > 1000000)
                    {
                        Array.Clear(receiveData, 0, receiveData.Length);
                        receiveData = null;
                        return;
                    }
                }
                if (packageLength < receiveData.Length)
                {
                    var package = TcpPackage.Parse(receiveData);
                    if (action != null && package != null)
                    {
                        action(package);

                        var slen = TcpPackage.GetLength(receiveData, package.Length);
                        if (slen >= 9)
                        {
                            var next = new byte[receiveData.Length - package.Length];
                            Buffer.BlockCopy(receiveData, package.Length, next, 0, next.Length);
                            this.UnPackage(next, action);
                        }
                    }
                }
                else if (packageLength == receiveData.Length)
                {
                    var package = TcpPackage.Parse(receiveData);
                    if (action != null && package != null)
                    {
                        action(package);
                    }
                }
                else if (packageLength > receiveData.Length)
                {
                    this.count = packageLength;
                    this._myBuffer = new byte[packageLength];
                    Buffer.BlockCopy(receiveData, 0, this._myBuffer, 0, receiveData.Length);
                    this.offset = receiveData.Length;
                }
                Array.Clear(receiveData, 0, receiveData.Length);
                receiveData = null;
            }
            else //跨包取内容的
            {
                try
                {
                    if (receiveData.Length + offset < count) //包内容超出
                    {
                        Buffer.BlockCopy(receiveData, 0, this._myBuffer, offset, receiveData.Length);
                        offset += receiveData.Length;
                    }
                    else if (receiveData.Length + offset >= count) //包内容短的
                    {
                        var packageLast = count - offset;
                        Buffer.BlockCopy(receiveData, 0, this._myBuffer, offset, packageLast);
                        var package = TcpPackage.Parse(this._myBuffer);
                        if (action != null && package != null)
                        {
                            action(package);
                        }
                        Array.Clear(this._myBuffer, 0, this._myBuffer.Length);
                        this._myBuffer = null;
                        count = offset = 0;
                        var receiveLast = receiveData.Length - packageLast;
                        if (receiveLast >= 4)//包含包头长度
                        {
                            var packageLength = TcpPackage.GetLength(receiveData, packageLast);
                            if (packageLength > 0)
                            {
                                if (receiveLast > packageLength)
                                {
                                    var nextData = new byte[receiveLast];
                                    Buffer.BlockCopy(receiveData, packageLast, nextData, 0, receiveLast);
                                    this.UnPackage(nextData, action);
                                }
                                else
                                {
                                    this._myBuffer = new byte[packageLength];
                                    Buffer.BlockCopy(receiveData, packageLast, this._myBuffer, 0, receiveLast);
                                    offset = receiveLast;
                                    count = packageLength;
                                }
                            }
                            //else if(packageLength == 0)
                            //{
                            //    count = offset = 0;
                            //    this._myBuffer = null;
                            //    this._myLenBuffer = null;
                            //}
                            else
                            {
                                if(this._myBuffer != null)
                                Array.Clear(this._myBuffer, 0, this._myBuffer.Length);
                                this._myBuffer = new byte[0];
                                count = offset = 0;
                                if (this._myLenBuffer != null)
                                    Array.Clear(this._myLenBuffer, 0, this._myLenBuffer.Length);
                                this._myLenBuffer = null;
                            }
                        }
                        else if (receiveLast > 0)//不包含包头长度
                        {
                            this._myLenBuffer = new byte[receiveLast];
                            Buffer.BlockCopy(receiveData, packageLast, this._myLenBuffer, 0, receiveLast);
                            if (TcpPackage.GetLength(this._myLenBuffer) == 0)
                            {
                                if (this._myLenBuffer != null)
                                    Array.Clear(this._myLenBuffer, 0, this._myLenBuffer.Length);
                                this._myLenBuffer = null;
                            }
                            if(this._myBuffer != null)
                            Array.Clear(this._myBuffer, 0, this._myBuffer.Length);
                            this._myBuffer = null;
                            this._myBuffer = null;
                            count = offset = 0;
                        }
                    }
                    receiveData = null;
                }
                catch
                {
                    if (this._myBuffer != null)
                        Array.Clear(this._myBuffer, 0, this._myBuffer.Length);
                    this._myBuffer = null;
                    this._myBuffer = null;
                    count = offset = 0;
                }
            }
            // semaphore.Release();
        }

        public DateTime ConnectDateTime
        {
            get; set;
        }

        public DateTime ActiveDateTime
        {
            get; set;
        }
    }
}
