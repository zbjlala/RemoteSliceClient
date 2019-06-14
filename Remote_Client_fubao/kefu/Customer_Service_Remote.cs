using DSkin.Forms;
using Newtonsoft.Json.Linq;
using Remote_Client_fubao.Helper;
using Remote_Client_fubao.Model.ClientEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Remote_Client_fubao.kefu
{
    public partial class Customer_Service : DSkinForm
    {
        /// <summary>
        /// LastID
        /// </summary>
        public string LastID { get; set; }
        /// <summary>
        /// userLongId  600开头
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 登录后JSON
        /// </summary>
        public JObject LoginJson { get; set; }

        public WorkOrderModel workOrderModel;

        /// <summary>
        /// 是否在远程
        /// </summary>
        private  bool isremote = false;
        public Customer_Service(string lastID, string loginJson)
        {
            InitializeComponent();
            LoginJson = JObject.Parse(loginJson);
            LastID = lastID;
            UserName = LoginJson["user"]["userLongId"].ToString(); 
            dSkinLabel3.Text = LoginJson["user"]["name"].ToString();
        }
        public Customer_Service(string lastID, string loginJson,string remote_id)
        {
            InitializeComponent();
            LoginJson = JObject.Parse(loginJson);
            LastID = lastID;
            dSkinTextBox1.Text = remote_id;
            UserName = LoginJson["user"]["userLongId"].ToString();
            dSkinLabel3.Text = LoginJson["user"]["name"].ToString();
            workOrderModel = new WorkOrderModel();
        }
        private void Customer_Service_Shown(object sender, EventArgs e)
        {
            remoteScreenUserControl1.OnConnectedFaild += remoteScreenUserControl1_OnConnectedFaild;
            remoteScreenUserControl1.ReConnectFail += remoteScreenUserControl1_ReConnectFail;
            remoteScreenUserControl1.OnNotice += remoteScreenUserControl1_OnNotice;
            //remoteScreenUserControl1.OnAccepted += RemoteScreenUserControl_OnAccepted;
            //remoteScreenUserControl1.OnOffLined += remoteScreenUserControl1_OnOffLined;
            //remoteScreenUserControl1.OnEscUp += remoteScreenUserControl1_OnEscUp;
            remoteScreenUserControl1.OnMsg += remoteScreenUserControl1_OnMsg;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(this.remoteScreenUserControl1.RemoteScreenUserControl_KeyDown);
            this.KeyUp += new KeyEventHandler(this.remoteScreenUserControl1.RemoteScreenUserControl_KeyUp);

            remoteScreenUserControl1.Init(LastID, UserName);

            if (dSkinTextBox1.Text.Length>0)
            {
                dSkinButton1_Click(this, e);
            }
        }
        /// <summary>
        /// 开始/结束远程按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dSkinButton1_Click(object sender, EventArgs e)
        {
            if (isremote)
            {
                remoteScreenUserControl1.Stop();
                remoteScreenUserControl1.Visible = false;
                dSkinButton1.Visible = true;
                dSkinLabel1.Visible = true;
                dSkinTextBox1.Enabled = true;
                isremote = false;
                //dSkinButton1.Visible = true;
                dSkinButton1.Text = "开始远程";
            }
            else
            {
                remoteScreenUserControl1.Visible = true;
                isremote = true;
               // Regex regex = new Regex(@"\d{3,15}");
               // if (regex.IsMatch(dSkinTextBox1.Text))
                // if (true)
               // {
                    this.InvokeAction(() =>
                    {
                        //remoteScreenUserControl1.Remote = regex.Match(dSkinTextBox1.Text).Value;
                        //remoteScreenUserControl1.StartSendCapture(regex.Match(dSkinTextBox1.Text).Value, this.dSkinLabel3.Text);
                        remoteScreenUserControl1.Remote =dSkinTextBox1.Text;
                        remoteScreenUserControl1.StartSendCapture(dSkinTextBox1.Text, this.dSkinLabel3.Text);
                    });
                    //dSkinLabel1.Visible = false;
                    dSkinTextBox1.Enabled = false;
                    dSkinButton1.Text = "结束远程";
                    dSkinButton1.Visible = true;
                    dSkinLabel1.Visible = true;
                    //dSkinButton1.Visible = false;
                    // RempteResize();
                //}
                //else
                //{
                //    MessageBox.Show("请输入正确ID");
                //}
            }
        }
        private void remoteScreenUserControl1_OnConnectedFaild()
        {
            MessageBox.Show("服务器连接错误，检查网络或服务器地址");
        }

        private void remoteScreenUserControl1_ReConnectFail()
        {
            MessageBox.Show("与服务器失去连接，重连失败请重新登录");
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        private void remoteScreenUserControl1_OnMsg(string msg)
        {

            if (IsDisposed)
            {
                return;
            }

            this.Invoke(new Action(() =>
            {
                if (msg.Contains("接收到数据3"))
                {
                    RempteResize();
                    dSkinLabel2.Text = "正在远程...";
                }
                else if (msg.Contains("失败"))
                {
                    dSkinLabel2.Text = msg;
                }
                textBox1.AppendText(msg + "\r\n");
            }));
        }

        int ReconnectInt = 0;
        /// <summary>
        /// 是否接收信息
        /// </summary>
        /// <param name="msg"></param>
        private void remoteScreenUserControl1_OnNotice(Model.Entity.Message  msg)
        {
            string json= Encoding.UTF8.GetString(msg.Data);
            StringBuilder stringBuilder = new StringBuilder();
            if (json == VerbalInfo.OTHER_LOGOUT)
            {
                this.Invoke(new Action(() =>
                {
                    dSkinLabel2.Text = "对方已登出";
                    textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\r\n" + "对方已登出" + "\r\n");
                }));
            }
            if (json== "登录成功")
            {
                ReconnectInt++;
                this.Invoke(new Action(() =>
                {
                    dSkinLabel2.Text = "登录成功";
                    textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\r\n" + "登录成功" + "\r\n");
                }));
            }
            else if (json == "接收到图片")
            {
                this.Invoke(new Action(() =>
                {
                    RempteResize();
                    dSkinLabel2.Text = "正在远程..."+ ((ReconnectInt==0?1: ReconnectInt )- 1).ToString();
                    textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\r\n" + "接收到图片" + "\r\n");
                }));
            }
            else
            {
                JObject msgjson = JObject.Parse(json);
                if (msgjson["type"].ToString() == ResultType.USER_INFO.ToString())
                {
                    JObject userjson = JObject.Parse(msgjson["result"].ToString());
                    try
                    {
                        stringBuilder.Append("在用产品：" + userjson["data"]["product"].ToString() + "\r\n" + "\r\n");
                        stringBuilder.Append("绑定模块：" + ArryToString(userjson["data"]["productmodulelist"].ToArray()) + "\r\n" + "\r\n");
                        stringBuilder.Append("服务商：" + userjson["data"]["service"].ToString() + "\r\n" + "\r\n");
                        stringBuilder.Append("公司名称：" + userjson["data"]["company"].ToString() + "\r\n" + "\r\n");
                        stringBuilder.Append("电话：" + userjson["data"]["userinfo"]["contact"]["mobile"].ToString() + "\r\n" + "\r\n");
                        stringBuilder.Append("邮箱：" + userjson["data"]["userinfo"]["email"].ToString() + "\r\n" + "\r\n");
                        stringBuilder.Append("客户IP:" + msgjson["ip"].ToString() + "\r\n" + "\r\n");
                        JObject ipAdress = JObject.Parse(msgjson["ip_Adress"].ToString());
                        stringBuilder.Append("客户区域:" + ipAdress["data"]["city"]+"-"+ ipAdress["data"]["province"]+"-"+ ipAdress["data"]["national"] + "\r\n" + "\r\n");
                        //加载头像
                        FaceLoad(userjson);
                    }
                    catch
                    {
                        stringBuilder.Append("信息加载异常" + "\r\n" + "\r\n");
                    }
                    this.Invoke(new Action(() =>
                        {
                            //dSkinTextBox2.Text = stringBuilder.ToString().Replace("\r\n\r\n", "\r\n");
                            dSkinHtmlLabel1.Text = "<a style=\"color:##393939;line-height:28px;font-size:14px;\">" + stringBuilder.ToString().Replace("\r\n\r\n","<br>") + "</a>"; 
                        }));

                    #region 填写工单默认值
                    workOrderModel.Service_ID = UserName;
                    workOrderModel.Product_From = userjson["data"]["product"].ToString();
                    workOrderModel.Customer_ID = userjson["data"]["userinfo"]["cop_user"].ToString();
                    workOrderModel.Customer_IP = msgjson["ip"].ToString();
                    workOrderModel.Order_From = userjson["data"]["company"].ToString();
                    workOrderModel.Begin_Time = DateTimeHelper.GetTimeSpan(DateTime.Now).ToString();
                    #endregion

                }
                else if (msgjson["type"].ToString() == ResultType.NETWORK_ABNORMAL.ToString())
                {
                    this.Invoke(new Action(() =>
                    {
                        dSkinLabel2.Text = VerbalInfo.NETWORK_ABNORMAL;
                        textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\r\n" + VerbalInfo.NETWORK_ABNORMAL + "\r\n");
                    }));
                }
                else if (msgjson["type"].ToString() == ResultType.RCONNECT_INFO.ToString()) {
                    this.Invoke(new Action(() =>
                    {
                        dSkinLabel2.Text = VerbalInfo.RECONNCED_INFO;
                        textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\r\n" + VerbalInfo.RECONNCED_INFO + "\r\n");
                    }));
                }
                else if (msgjson["type"].ToString() == ResultType.OTHER_LOGOUT.ToString())
                {
                    this.Invoke(new Action(() =>
                    {
                        dSkinLabel2.Text = "对方已登出";
                        textBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\r\n" + VerbalInfo.OTHER_LOGOUT+ "\r\n");
                    }));
                }
            }            
         

        }

        private void FaceLoad(JObject userjson)
        {
            try
            {
                this.Invoke(new EventHandler(delegate
                {
                    SetPictureBoxRegion();
                }));

                Image pic = Image.FromStream(WebRequest.Create(userjson["data"]["userinfo"]["avatar"].ToString()).GetResponse().GetResponseStream());
                this.dSkinPictureBox1.Image = pic;
            }
            catch
            {
                this.dSkinPictureBox1.Image = Image.FromFile(new DirectoryInfo("../../../") + "服宝机器人T3头像.png");
            }
        }
        /// <summary>
        /// 重新远程图像比例
        /// </summary>
        private void RempteResize()
        {
            float paneblProportion = (float)controlHost1.Width / (float)controlHost1.Height;
            float remoteScreenUserControlProportion = remoteScreenUserControl1.ImageProportion;
            //panel宽高比大于远程图片比例
            if (paneblProportion >= remoteScreenUserControlProportion)
            {
                remoteScreenUserControl1.Size = new Size((int)(controlHost1.Height * remoteScreenUserControlProportion), controlHost1.Height);
            }
            else
            {
                remoteScreenUserControl1.Size = new Size(controlHost1.Width, (int)(controlHost1.Width / remoteScreenUserControlProportion));
            }
            remoteScreenUserControl1.Location = new Point((int)((controlHost1.Width - remoteScreenUserControl1.Width) / 2), (int)((controlHost1.Height - remoteScreenUserControl1.Height) / 2));
        }

        private void remoteScreenUserControl1_MouseEnter(object sender, EventArgs e)
        {
            remoteScreenUserControl1.Focus();
        }

        private void Customer_Service_FormClosing(object sender, FormClosingEventArgs e)
        {
            remoteScreenUserControl1.Stop();
           // remoteScreenUserControl1.Dispose();           
        }

        private void controlHost1_Resize(object sender, EventArgs e)
        {
            RempteResize();
        }

        private void Customer_Service_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void Customer_Service_KeyDown(object sender, KeyEventArgs e)
        {
            //ctrl+I 显示调试信息
            if (e.Control && e.Alt && e.KeyCode == Keys.I) {
                textBox1.Visible = textBox1.Visible ? false : true;               
            }
        }

        private void SetPictureBoxRegion()
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddEllipse(this.dSkinPictureBox1.ClientRectangle);

            Region region = new Region(gp);

            this.dSkinPictureBox1.Region = region;

            gp.Dispose();

            region.Dispose();
        }


        private void dSkinButton4_Click(object sender, EventArgs e)
        {
            WorkOrder workOrder = new WorkOrder(workOrderModel);
            workOrder.ShowDialog();
        }

        public string ArryToString(Array array)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var item in array)
            {
                builder.Append(item + ",");

            }
            return builder.ToString();
        }
    }
}
