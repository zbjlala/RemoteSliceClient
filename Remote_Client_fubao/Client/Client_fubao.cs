using Remote_Client_fubao.Helper;
using Remote_Client_fubao.Helper.HttpHelper;
using Remote_Client_fubao.Helper.Win32;
using Remote_Client_fubao.Model.ClientEntity;
using Remote_Client_fubao.Model.Entity;
using Remote_Client_fubao.Remote;
using Remote_Client_fubao.Remote.MS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote_Client_fubao.Client
{
    public class Client_fubao
    {
        private static bool _isHelp = false;
        /// <summary>
        /// 本客户userid
        /// </summary>
        public string _userName;
        /// <summary>
        /// 登录身份信息
        /// </summary>
        private LoginInfo _logInfo;

        private HttpProxy httpProxy;
        /// <summary>
        /// 身份类型1客服2客户
        /// </summary>
        private string _type;

        public MessageClient _mCleint = null;

        private MouseAndKeyHelper _MouseAndKeyHelper = null;

        private QualityLevelEnum _qualityLevel = QualityLevelEnum.Low;

        public static volatile bool _isClose ;
        /// <summary>
        /// 远程方userid
        /// </summary>
        string _remote = string.Empty;

        private static System.Windows.Forms.Timer timer = null;

        private static bool _isSend = false;

        public  static int sendCount ;//防止多个Task发送图片

        private int sec = 200;//正常的传输

        private int secKeep = 7000;//每7秒发送一张整图

        /// <summary>
        /// 加密锁号
        /// </summary>
        private string Dgcode;
 

        static CancellationTokenSource cts ;


        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="loginInfo">登录身份信息</param>
        /// <param name="user_id">user_id信息</param>
        /// <param name="type">1、客服 2 客户</param>
        /// <param name="keydog">加密锁号</param>
        public Client_fubao(LoginInfo loginInfo, string user_id, string type = "2",string dgcode = "")
        {
            this._logInfo = loginInfo;

            this._userName = user_id;

            this._type = type;

            this.Dgcode = dgcode;

            sendCount = 0;

           
           // cts = new CancellationTokenSource();
        }
        /// <summary>
        /// 开启客户版客户端
        /// </summary>
        public void Start()
        {
            httpProxy = new HttpProxy();

            _MouseAndKeyHelper = new MouseAndKeyHelper();
          
            _mCleint = new MessageClient(this._userName,this.Dgcode);

            _mCleint.OnMessage += webCient_OnMessage;

            _mCleint.OnNotice += _mClient_OnNotice;

            var isConnected = _mCleint.Connect();

            _mCleint.isCloseing = false;
           
             cts = new CancellationTokenSource();

            if (timer == null)
            {
                timer = new System.Windows.Forms.Timer();
            }
            timer.Interval = 1000;//每秒查询一次鼠标和键盘休闲时间
            timer.Tick += new EventHandler(timer_Event);
        }

        private static void timer_Event(object sender, EventArgs e)
        {

            if (MouseAndKeyHelper.GetLastInputTime() >= 5)
            {
                _isSend = false;
            }
            else
            {
                _isSend = true;
            }
        }

        /// <summary>
        /// 接收到命令
        /// </summary>
        /// <param name="msg"></param>
        public void webCient_OnMessage(object sender, Remote_Client_fubao.Model.Entity.Message msg)
        { 

                try
                {
                //var transferStr = Encoding.UTF8.GetString(msg.Data).ToLower();
                var transferStr = Encoding.UTF8.GetString(msg.Data);
                var transferArr = transferStr.Split('\\');
                    if (transferArr.Count() > 0)
                    {
                    if (transferArr[0] == "chanjetservice")
                    {
                        TrueOrFalse trueOrFalse = new TrueOrFalse(transferArr[1]);
                        trueOrFalse.ShowDialog();
                        if (trueOrFalse.Accept)
                        {
                            _isHelp = true;
                            _isClose = false;
                            _remote = msg.Sender;
                            _mCleint.SendRemoteConnect(msg.Accepter, msg.Sender, true);
                            _isSend = true;
                            lastBitmap = null;
                            Interlocked.Increment(ref sendCount);
                            if (sendCount < 2)
                                SendImage();
                                SendImageKeep();
                            
                        }
                        else
                        {
                            _mCleint.SendRemoteConnect(msg.Accepter, msg.Sender, false);
                            return;
                        }
                    }
                    else
                    {
                        if (_isHelp)
                        { 

                        var transferArr2 = transferStr.Split(new string[] { ":" }, StringSplitOptions.None);
                        var command = transferArr2[0];
                        var dataStr = transferArr2[1];
                        switch (command)
                        {
                            case "mousemove"://鼠标移动
                                string[] localString = dataStr.Split('@');
                                int localX = int.Parse(localString[0]);
                                int localY = int.Parse(localString[1]);
                                int remoteFormHeight = int.Parse(localString[2]);
                                double rate = ((double)Screen.PrimaryScreen.Bounds.Height) / remoteFormHeight;
                                _MouseAndKeyHelper.SetCursorPosition((int)(localX * rate), (int)(localY * rate));
                                break;
                            case "mousedown"://鼠标键的按下
                                if (dataStr == "left")
                                    _MouseAndKeyHelper.MouseDown(MouseAndKeyHelper.ClickOnWhat.LeftMouse);
                                else if (dataStr == "middle")
                                    _MouseAndKeyHelper.MouseDown(MouseAndKeyHelper.ClickOnWhat.MiddleMouse);
                                else if (dataStr == "right")
                                    _MouseAndKeyHelper.MouseDown(MouseAndKeyHelper.ClickOnWhat.RightMouse);
                                break;
                            case "mouseup"://鼠标键的抬起
                                if (dataStr == "left")
                                    _MouseAndKeyHelper.MouseUp(MouseAndKeyHelper.ClickOnWhat.LeftMouse);
                                else if (dataStr == "middle")
                                    _MouseAndKeyHelper.MouseUp(MouseAndKeyHelper.ClickOnWhat.MiddleMouse);
                                else if (dataStr == "right")
                                    _MouseAndKeyHelper.MouseUp(MouseAndKeyHelper.ClickOnWhat.RightMouse);
                                break;
                            case "keypress"://键盘键的按下即抬起
                                    string[] arry = dataStr.Split('&');
                                    if (arry.Length <= 1)
                                    {
                                        int keyValue = int.Parse(dataStr);
                                        MouseAndKeyHelper.VirtualKeys virtualKey = (MouseAndKeyHelper.VirtualKeys)(Enum.ToObject(typeof(MouseAndKeyHelper.VirtualKeys), keyValue));
                                        _MouseAndKeyHelper.KeyPress(virtualKey);
                                    }
                                    else
                                    {
                                        List<MouseAndKeyHelper.VirtualKeys> list = new List<MouseAndKeyHelper.VirtualKeys>();
                                        foreach(var str in arry)
                                        {
                                            int Value = int.Parse(str);
                                            MouseAndKeyHelper.VirtualKeys virtualKey = (MouseAndKeyHelper.VirtualKeys)(Enum.ToObject(typeof(MouseAndKeyHelper.VirtualKeys), Value));
                                            list.Add(virtualKey);
                                        }
                                        _MouseAndKeyHelper.KeyPress(list);
                                    }
                                    break;
                            case "mousewheel"://鼠标滚轮的滚动
                                int delta = int.Parse(dataStr);
                                _MouseAndKeyHelper.MouseWheel(delta);
                                break;
                            default:
                                break;
                        }


                        if (transferStr == "helpcommand")
                        {

                        }
                        if (transferStr == "stopremotehelp")
                        {

                        }
                    }
                    }
                }

                }
                catch (Exception ex)
                {
                }

        }
        object o = new object();
        static Bitmap lastBitmap = null;
        static bool isBigChnage = false;
        /// <summary>
        /// 发送图像到远程
        /// </summary>
        private void SendImage()
        {
            Task.Factory.StartNew(() =>
           {
              
                   //Bitmap lastBitmap = null;
                   Rectangle rectangle = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
               while (!_isClose)
               {
                   try
                   {
                       if (true)
                       {
                           using (Bitmap desktopImage = new Bitmap(rectangle.Width, rectangle.Height))
                           {
                               using (Graphics g = Graphics.FromImage(desktopImage))
                               {
                                   try
                                   {
                                       g.CopyFromScreen(0, 0, 0, 0, desktopImage.Size);
                                   }
                                   catch(Exception ex)
                                   {
                                       continue;
                                   }
                                   MouseAndKeyHelper.DrawMouse(g);
                                   //比较此次截图与上一张截图的差异
                                   if (lastBitmap != null)
                                   {
                                       List<Rectangle> rects = ImageComparer.Compare(lastBitmap, desktopImage);
                                       lastBitmap = (Bitmap)desktopImage.Clone();
                                       if (rects.Count == 0)//无变化不发送
                                       {
                                           Thread.Sleep(sec);
                                       }
                                       if (rects.Count <= 480)//差异小于7块则分段传输
                                       {
                                           Dictionary<Rectangle, Bitmap> dic = new Dictionary<Rectangle, Bitmap>();
                                           foreach (var rect in rects)
                                           {
                                               Bitmap bmSmall = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppRgb);
                                               using (Graphics grSmall = Graphics.FromImage(bmSmall))
                                               {
                                                   grSmall.DrawImage(desktopImage, 0, 0,
                                                   rect, GraphicsUnit.Pixel);
                                                   grSmall.Save();
                                                   grSmall.Dispose();
                                               }
                                               dic.Add(rect, (Bitmap)bmSmall.Clone());
                                           }
                                           if (dic.Count > 0)
                                           {
                                               _mCleint.SendFileSlice(_remote, CompressHelper.Compress(SerializeHelper.ByteSerialize(dic)));
                                               isBigChnage = false;
                                               Thread.Sleep(sec);
                                           }

                                       }
                                       else
                                       {//发送完整的图片


                                           if (isBigChnage == false)
                                           {
                                               SendImageFile(desktopImage, g);
                                               lastBitmap = (Bitmap)desktopImage.Clone();//要在之前否则销毁掉了
                                               Thread.Sleep(sec);
                                           }
                                           else
                                               isBigChnage = false;
                                       }

                                   }
                                   else
                                   { //第一张图片
                                       lastBitmap = (Bitmap)desktopImage.Clone();//要在之前否则销毁掉了
                                       SendImageFile(desktopImage, g);
                                       Thread.Sleep(sec);
                                   }



                               }

                           }
                       }
                       else
                       {
                           Thread.Sleep(100);
                       }

                   }
                   catch(Exception ex)
                   {
                       continue;
                   }
                    }
 
               
            },cts.Token);
        }

        public void SendImageFile(Bitmap desktopImage,Graphics g)
        {
            using (MemoryStream ms = ImageHelper.GetLossyCompression(desktopImage, 15, "W", Screen.PrimaryScreen.Bounds.Width))
            {
                if (desktopImage != null)
                {
                    try
                    {
                        ms.Position = 0;
                        _mCleint.SendFile(_remote, CompressHelper.Compress(ms.GetBuffer()));
                        isBigChnage = true;
                        //if (_fps > 0)
                        //    sec = 1000 / _fps;
                        //else
                        //    sec = 2000;
                        //Thread.Sleep(sec);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        ms.Dispose();
                        g.Dispose();
                        desktopImage.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 每5秒发送一张整图防错
        /// </summary>
        public void SendImageKeep()
        {
            Task.Factory.StartNew(() => { 
            Rectangle rectangle = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            while (!_isClose)
            {
                try
                {
                    if (true)
                    {
                        using (Bitmap desktopImage = new Bitmap(rectangle.Width, rectangle.Height))
                        {
                            using (Graphics g = Graphics.FromImage(desktopImage))
                            {
                                try
                                {
                                    g.CopyFromScreen(0, 0, 0, 0, desktopImage.Size);
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                                MouseAndKeyHelper.DrawMouse(g);
                                SendImageFile(desktopImage, g);
                                Thread.Sleep(secKeep);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            }, cts.Token);
        }
        /// <summary>
        /// 接收远程开始信息
        /// </summary>
        public event OnServiceNotice OnNotice;
        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="msg"></param>
        public void _mClient_OnNotice(Model.Entity.Message msg)
        {
            OnNotice?.Invoke(msg);
            //ServiceNotice serviceNotice = new ServiceNotice();
            //serviceNotice.StartPosition = FormStartPosition.CenterParent;
            //serviceNotice.Notice.Text = Encoding.UTF8.GetString(msg.Data);
            //serviceNotice.ShowDialog();
        }
        /// <summary>
        /// 关闭远程组件
        /// </summary>
        public void ClientDispose()
        {
            try
            {
                _mCleint.SendLogout(_remote, _userName + "退出远程");
                cts.Cancel();
                timer.Stop();
                Thread.Sleep(2000);
                _mCleint.Stop();
                _mCleint.isCloseing = true;
                _isClose = true;
                sendCount = 0;
                _isHelp = false;
            }
            catch
            {

            }
        }

    }

    public enum QualityLevelEnum
    {
        High = 70,
        Normal = 35,
        Low = 10
    }
}
