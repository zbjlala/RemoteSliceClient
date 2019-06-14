using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Remote_Helper;
using System.IO.Compression;
using System.Diagnostics;
using System.Threading;
using Remote_Client_fubao.Remote.MS;
using Remote_Client_fubao.Remote;
using Remote_Client_fubao.Helper.Win32;
using Remote_Client_fubao.Helper;

namespace Remote_Client_fubao.Client
{
    /// <summary>
    /// 服务器接收到客户端远程协助请求控件
    /// 启动远程服务器功能（1.绘制屏幕2.收集并发送鼠标键盘命令）
    /// </summary>
    public partial class RemoteScreenUserControl : UserControl
    {
        public RemoteScreenUserControl()
        {
            InitializeComponent();
        }

        MessageClient _mClient = null;

        string _username;

        string _lastID;

        bool _isConnected;

        int VK_SHIFT = 16;

        int VK_CONTROL = 17;　 //Ctrl(或者另一个） 

        int VK_MENU = 18;　　　//Alt(或者另一个) 


        /// <summary>
        /// 服务器连接错误
        /// </summary>

        public delegate void OnConnectedFaildHandler();

        public OnConnectedFaildHandler OnConnectedFaild;

        public delegate void OnAcceptedHandler();

        public ReConnectFaildHanlder ReConnectFail;
        /// <summary>
        /// 服务器接收到客户端时
        /// </summary>
        public OnAcceptedHandler OnAccepted;

        /// <summary>
        /// 服务器接收到客户端时
        /// </summary>
        /// <param name="nHelper"></param>
        protected void RaiseOnAccepted()
        {
            OnAccepted?.Invoke();
        }

        MouseAndKeyHelper _MouseAndKeyHelper = new MouseAndKeyHelper();

        /// <summary>
        /// 是否接收到helpcommand
        /// </summary>
        public bool IsHelpered
        {
            get; private set;
        }

        /// <summary>
        /// 接收到远程图片比例
        /// </summary>
        public float ImageProportion
        {
            get;set;
        }
        public string Remote { get; set; }

        /// <summary>
        /// 启动远程服务器功能（1.绘制屏幕2.收集并发送鼠标键盘命令）
        /// </summary>
        /// <param name="userName"></param>
        public void Init(string lastID,string userName)
        {
            this._username = userName;
            _lastID = lastID;
            _mClient = new MessageClient(userName);
            _mClient.OnFile += _mClient_OnFile;
            _mClient.ReConnectFaild += _mClient_ReConnectFaild;
            _mClient.OnNotice += _mClient_OnNotice;
            _mClient.OnError += onError;
            _mClient.OnMsg += onMsg;
            _mClient.OnMessage += _mClient_OnMessage;
            _isConnected = _mClient.Connect();
            while (!_isConnected)
            {
                Thread.Sleep(10);

            }
            if (!_isConnected)
                OnConnectedFaild();
            else
                _isConnected = true;
            
        }

        private void _mClient_OnMessage(object sender, Model.Entity.Message msg)
        {
            var transfer = Encoding.UTF8.GetString(msg.Data);

            if (transfer == "chanjetservice")
            {
                Remote = msg.Sender;
                RaiseOnAccepted();
            }
            _mClient.SendMessage(Remote, "chanjetservice");

        }
        static Bitmap lastBitmap;
        /// <summary>
        /// 接收远程桌面的图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <param name="type">1 整图 2 切片</param>
        private void _mClient_OnFile(object sender, byte[] file,string type)
        {
            try
            {
                if (file.Length != 0 && file != null)
                {
                    var deDates = CompressHelper.Decompress(file);
                    if (type == "1")
                    {
                        var datas = deDates;

                            this.InvokeAction(() =>
                            {
                                try
                                {
                                    this.pictureBox1.Image = ImageHelper.ToImage(datas);
                                    lastBitmap = (Bitmap)ImageHelper.ToImage(datas);
                                    ImageProportion = (float)this.pictureBox1.Image.Width / (float)this.pictureBox1.Image.Height;
                                    this.Invoke(new Action(() =>
                                    {
                                        this.pictureBox1.Refresh();
                                    }));

                                }
                                catch { }

                            });

                    }
                    if (type == "2")
                    {
                        Dictionary<Rectangle, Bitmap> dic = SerializeHelper.ByteDeserialize<Dictionary<Rectangle, Bitmap>>(deDates);
                        if(dic != null && dic.Count > 0)
                        {
                            foreach(var item in dic)
                            {
                                Bitmap bm = new Bitmap(item.Key.Width, item.Key.Height);
                                using (Graphics g = Graphics.FromImage(lastBitmap))
                                {
                                    try
                                    {
                                        g.DrawImage((Bitmap)item.Value.Clone(), item.Key);
                                        g.Save();
                                    }
                                    catch
                                    {

                                    }
                                    finally
                                    {
                                        g.Dispose();
                                    }
                                }
                            }
                        }
                        this.pictureBox1.Image = lastBitmap;
                        this.Invoke(new Action(()=> 
                        { this.pictureBox1.Refresh();
                        }));

                    }

                    // Array.Clear(file, 0, file.Length);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 服务器接收到消息错误时
        /// </summary>
        /// <param name="ex"></param>
        void transferHelper_OnServerReceivErred(Exception ex)
        {
            this.RaiseOnOffLined("服务器接收到消息异常：" + ex.Message);
            this.Stop();
        }

        public void _mClient_ReConnectFaild()
        {
            ReConnectFail();
        }
        /// <summary>
        /// 接收远程开始信息
        /// </summary>
        public event OnServiceNotice OnNotice;
        public void _mClient_OnNotice(Model.Entity.Message msg)
        {
            OnNotice?.Invoke(msg);
            //ServiceNotice serviceNotice = new ServiceNotice();
            //serviceNotice.StartPosition = FormStartPosition.CenterParent;
            //serviceNotice.Notice.Text = Encoding.UTF8.GetString(msg.Data);
            //serviceNotice.ShowDialog();


        }

        private void FormRemoteHelpDesktop_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this._mClient != null && this._isConnected)
            {
                try
                {
                    _mClient.SendMessage(Remote, "mousewheel:" + e.Delta.ToString());
                }
                catch
                {

                }
            }
        }

        private void RemoteScreenUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (this._mClient != null && this._isConnected)
            {
                try
                {
                    var transferStr = "mousedown";
                    if (e.Button == MouseButtons.Left)
                        transferStr += ":left";
                    else if (e.Button == MouseButtons.Middle)
                        transferStr += ":middle";
                    else if (e.Button == MouseButtons.Right)
                        transferStr += ":right";
                    _mClient.SendMessage(Remote, transferStr);
                }
                catch
                {

                }
            }
        }

        private void RemoteScreenUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            this.Focus();
            if (this._mClient != null && this._isConnected)
            {
                try
                {
                    var transferStr = "mousemove:" + e.X + "@" + e.Y + "@" + this.Height;
                    OnMsg?.Invoke(transferStr.ToString());
                    _mClient.SendMessage(Remote, transferStr);
                    Thread.Sleep(90);
                }
                catch
                {

                }
            }
        }

        public void RemoteScreenUserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (this._mClient != null && this._isConnected)
            {
                try
                {
                    if (e.KeyCode == Keys.Escape)
                    {
                        if (MessageBox.Show("需要关闭远程协助吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            var transferStr = "StopRemoteHelp:";
                            _mClient.SendMessage(Remote, transferStr);
                            this.RaiseOnEscUp();
                        }
                    }
                    else if (e.Control && e.KeyCode == Keys.Enter)
                    {
                        if (this.Height <= 450)
                        {
                            this.Height = Screen.PrimaryScreen.Bounds.Height;
                            this.Location = new Point(0, 0);
                            var transferStr = "FullScreen:";
                            _mClient.SendMessage(Remote, transferStr);
                        }
                        else
                        {
                            this.Height = 450;
                            var transferStr = "PartScreen:";
                            _mClient.SendMessage(Remote, transferStr);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public void RemoteScreenUserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (this._mClient != null && this._isConnected)
            {
                try
                {
                    if (e.Control && e.Shift && e.KeyValue >= 0)
                    {
                        var transferStr = "keypress:" + VK_CONTROL + "&" + VK_SHIFT + "&" + e.KeyValue.ToString();
                        _mClient.SendMessage(Remote, transferStr);
                    }
                    else if (e.Control && e.Alt && e.KeyValue >= 0)
                    {
                        var transferStr = "keypress:" + VK_CONTROL + "&" + VK_MENU + "&" + e.KeyValue.ToString();
                        _mClient.SendMessage(Remote, transferStr);
                    }
                    else if (e.Alt && e.Shift && e.KeyValue >= 0)
                    {
                        var transferStr = "keypress:" + VK_MENU + "&" + VK_SHIFT + "&" + e.KeyValue.ToString();
                        _mClient.SendMessage(Remote, transferStr);
                    }else if (e.Control && e.KeyValue >=0)
                    {
                        var transferStr = "keypress:" + VK_CONTROL +"&"+e.KeyValue.ToString();
                        _mClient.SendMessage(Remote, transferStr);
                    }
                    else  if (e.Shift && e.KeyValue >= 0)
                    {
                        var transferStr = "keypress:" + VK_SHIFT + "&" + e.KeyValue.ToString();
                        _mClient.SendMessage(Remote, transferStr);
                    }
                    else if (e.Alt && e.KeyValue >= 0)
                    {
                        var transferStr = "keypress:" + VK_MENU + "&" + e.KeyValue.ToString();
                        _mClient.SendMessage(Remote, transferStr);
                    }
                    else
                    {
                        var transferStr = "keypress:" + e.KeyValue.ToString();
                        _mClient.SendMessage(Remote, transferStr);
                    }
                }
                catch
                {

                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public delegate void OnEscUpHandler();
        /// <summary>
        /// 协助方按ESC键结束协助
        /// </summary>
        public event OnEscUpHandler OnEscUp;

        protected void RaiseOnEscUp()
        {
            if (this.OnEscUp != null)
            {

                _mClient = null;
                this.OnEscUp();
            }
        }

        public delegate void OnOffLinedHandler(string msg);
        /// <summary>
        /// 接受远程桌面图片对方掉线时事件
        /// </summary>
        public event OnOffLinedHandler OnOffLined;

        protected void RaiseOnOffLined(string msg)
        {
            if (this.OnOffLined != null)
            {
                this.OnOffLined(msg);
            }
        }

        /// <summary>
        /// 关闭远程协助
        /// </summary>
        public void Stop()
        {
            try
            {
                _mClient.SendLogout(Remote, _username + "退出远程");
                _mClient.Stop();
                _mClient.timer.Stop();
                _mClient.isCloseing = true;
                _mClient = null;
            }
            catch { }

        }

        private void RemoteScreenUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._mClient != null && this._isConnected)
            {
                try
                {
                    var transferStr = "mouseup:";
                    if (e.Button == MouseButtons.Left)
                        transferStr += "left";
                    else if (e.Button == MouseButtons.Middle)
                        transferStr += "middle";
                    else if (e.Button == MouseButtons.Right)
                        transferStr += "right";
                    _mClient.SendMessage(Remote, transferStr);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// 错误信息委托
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        public delegate void OnClientErrorHandler(Exception ex, string msg);
        /// <summary>
        /// 错误信息事件
        /// </summary>
        public event  OnClientErrorHandler OnError;
        /// <summary>
        /// 错误信息
        /// </summary>
        private void onError(Exception ex, string msg) {
            OnError?.Invoke(ex, msg);           
        }
        /// <summary>
        /// 日志消息委托
        /// </summary>
        /// <param name="msg"></param>
        public delegate void OnClientMsgHandler(string msg);

        /// <summary>
        /// 错误信息事件
        /// </summary>
        public event OnClientMsgHandler OnMsg;
        /// <summary>
        /// 日志消息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        private void onMsg( string msg)
        {
            OnMsg?.Invoke(msg);
        }

        /// <summary>
        /// 开始桌面共享、远程协助（发送图片）
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="isHelp"></param>
        /// <param name="qualityLevel"></param>
        /// <param name="fps"></param>
        public void StartSendCapture(string remote,string username, bool isHelp = true, QualityLevelEnum qualityLevel = QualityLevelEnum.Normal, int fps = 1)
        {
            //_isHelp = isHelp;
            //_qualityLevel = qualityLevel;
            //_fps = fps;
            // _isStartCapture = true;
            //_mClient.ConnectAsync();
           // Remote =  remote;
            //while (!_mClient.IsConnected)
            //{
            //    Thread.Sleep(10);
                
            //}
            //this._isConnected = true;
            this.SendRequest(username);
        }

        private void SendRequest(string username)
        {
            if (_mClient ==null)
            {
                Init(_lastID, _username);
            }
            _mClient.SendMessage(Remote, "chanjetservice" + "\\" + username);
        }

        private void RemoteScreenUserControl_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (this._mClient != null && this._isConnected)
            {
                try
                {
                    var transferStr = "KeyPress:" + e.KeyValue.ToString();
                    _mClient.SendMessage(Remote, transferStr);
                }
                catch
                {

                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this.Focus();
            if (this._mClient != null && this._isConnected)
            {
                try
                {
                    var transferStr = "mousemove:" + e.X + "@" + e.Y + "@" + this.Height;
                    OnMsg?.Invoke(transferStr.ToString());
                    _mClient.SendMessage(Remote, transferStr);
                }
                catch
                {

                }
            }
        }
    }
}
